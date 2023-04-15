/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using Newtonsoft.Json.Linq;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Services;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor : BaseLoggedComponent
    {
        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject)
            : base(context.Logger)
        {
            _context = context;
            _codeFrameService = context.ServicesFactory.GetCodeFrameService();

            _projectLoader = new ProjectLoader(context);

            _globalStorage = context.Storage.GlobalStorage;
            _globalLogicalStorage = _globalStorage.LogicalStorage;
            _hostListener = context.HostListener;

            _instancesStorage = _context.InstancesStorage;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _methodsResolver = dataResolversFactory.GetMethodsResolver();
            _constructorsResolver = dataResolversFactory.GetConstructorsResolver();
            _logicalSearchResolver = dataResolversFactory.GetLogicalSearchResolver();
            _statesResolver = dataResolversFactory.GetStatesResolver();
            _annotationsResolver = dataResolversFactory.GetAnnotationsResolver();
            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();

            _valueResolvingHelper = dataResolversFactory.GetValueResolvingHelper();

            _converterFactToImperativeCode = context.ConvertersFactory.GetConverterFactToImperativeCode();
            _dateTimeProvider = context.DateTimeProvider;

            var commonNamesStorage = _context.CommonNamesStorage;

            _defaultCtorName = commonNamesStorage.DefaultCtorName;
            _timeoutName = commonNamesStorage.TimeoutAttributeName;
            _priorityName = commonNamesStorage.PriorityAttributeName;
        }

        private readonly IEngineContext _context;
        private readonly ICodeFrameService _codeFrameService;

        private readonly ProjectLoader _projectLoader;

        private readonly IStorage _globalStorage;
        private readonly ILogicalStorage _globalLogicalStorage;
        private readonly IHostListener _hostListener;
        private readonly IInstancesStorageComponent _instancesStorage;

        private readonly OperatorsResolver _operatorsResolver;
        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private readonly VarsResolver _varsResolver;
        private readonly MethodsResolver _methodsResolver;
        private readonly ConstructorsResolver _constructorsResolver;
        private readonly LogicalSearchResolver _logicalSearchResolver;
        private readonly StatesResolver _statesResolver;
        private readonly AnnotationsResolver _annotationsResolver;
        private readonly InheritanceResolver _inheritanceResolver;

        private readonly ValueResolvingHelper _valueResolvingHelper;
        
        private readonly ConverterFactToImperativeCode _converterFactToImperativeCode;
        private readonly IDateTimeProvider _dateTimeProvider;

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        private IExecutionCoordinator _executionCoordinator;
        private IInstance _currentInstance;
        private IVarStorage _currentVarStorage;

        private ErrorValue _currentError;
        private bool _isCanceled;

        private long? _endOfTargetDuration;
        private List<Task> _waitedTasksList;
        private Task _pseudoSyncTask;

        private readonly StrongIdentifierValue _defaultCtorName;
        private readonly StrongIdentifierValue _timeoutName;
        private readonly StrongIdentifierValue _priorityName;

        public Value ExternalReturn { get; private set; }

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame, bool setAsRunning = true)
        {
            var timeout = codeFrame.TargetDuration;

            if (timeout.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTiks;

                var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

                codeFrame.EndOfTargetDuration = Convert.ToInt64(currentMilisecond + timeout.Value);
            }

            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;

            SetUpCurrentCodeFrame();

            if (setAsRunning)
            {
                _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;
            }
        }

        public void SetCodeFrames(List<CodeFrame> codeFramesList)
        {
            var reverseCodeFramesList = codeFramesList.ToList();
            reverseCodeFramesList.Reverse();

            foreach (var codeFrame in reverseCodeFramesList)
            {
                codeFrame.ProcessInfo.Status = ProcessStatus.WaitingToRun;

                SetCodeFrame(codeFrame, false);
            }

            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;

        }

        public Value Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _activeObject.Dispose();
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
            try
            {
                if (_currentCodeFrame == null)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

                if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

                if (_currentCodeFrame.ProcessInfo.Status == ProcessStatus.Canceled)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

                if (_currentCodeFrame.ProcessInfo.Status == ProcessStatus.WeakCanceled)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.WeakCanceled);
                    return true;
                }

                var endOfTargetDuration = _currentCodeFrame.EndOfTargetDuration;

                if (endOfTargetDuration.HasValue)
                {
                    var currentTick = _dateTimeProvider.CurrentTiks;

                    var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

                    if (currentMilisecond >= endOfTargetDuration.Value)
                    {
                        GoBackToPrevCodeFrame(ActionExecutionStatus.WeakCanceled);
                        return true;
                    }
                }

                var currentPosition = _currentCodeFrame.CurrentPosition;

                var compiledFunctionBodyCommands = _currentCodeFrame.CompiledFunctionBody.Commands;

                if (currentPosition >= compiledFunctionBodyCommands.Count)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.Complete);
                    return true;
                }

                if (!CheckReturnedInfo())
                {
                    return false;
                }

                var currentCommand = compiledFunctionBodyCommands[currentPosition];

#if DEBUG
                //Log($"_currentCodeFrame.LocalContext.Holder = {_currentCodeFrame.LocalContext.Holder}");
                //Log($"currentCommand = {currentCommand}");
                Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                switch (currentCommand.OperationCode)
                {
                    case OperationCode.Nop:
                        ProcessNop();                        
                        break;

                    case OperationCode.ClearStack:
                        ProcessClearStack();
                        break;

                    case OperationCode.PushVal:
                        ProcessPushVal(currentCommand);
                        break;

                    case OperationCode.VarDecl:
                        ProcessVarDecl(currentCommand);
                        break;

                    case OperationCode.CodeItemDecl:
                        ProcessCodeItemDecl(currentCommand);
                        break;

                    case OperationCode.CallBinOp:
                        ProcessCallBinOp(currentCommand);
                        break;

                    case OperationCode.CallUnOp:
                        ProcessCallUnOp(currentCommand);
                        break;

                    case OperationCode.Call:
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.Sync);
                        break;

                    case OperationCode.Call_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.Sync);
                        break;

                    case OperationCode.Call_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.Sync);
                        break;

                    case OperationCode.AsyncCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncChildCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.ChildAsync);
                        break;

                    case OperationCode.AsyncChildCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.ChildAsync);
                        break;

                    case OperationCode.AsyncChildCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.ChildAsync);
                        break;

                    case OperationCode.CallCtor:
                        CallConstructor(KindOfFunctionParameters.NoParameters, 0);
                        break;

                    case OperationCode.CallCtor_N:
                        CallConstructor(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams);
                        break;

                    case OperationCode.CallCtor_P:
                        CallConstructor(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams);
                        break;

                    case OperationCode.CallDefaultCtors:
                        CallDefaultCtors();
                        break;

                    case OperationCode.ExecCallEvent:
                        ProcessExecCallEvent();
                        break;

                    case OperationCode.Exec:
                        ProcessExec();
                        break;

                    case OperationCode.SetInheritance:
                        ProcessSetInheritance();
                        break;

                    case OperationCode.SetNotInheritance:
                        ProcessSetNotInheritance();
                        break;

                    case OperationCode.SetSEHGroup:
                        ProcessSetSEHGroup(currentCommand);
                        break;

                    case OperationCode.RemoveSEHGroup:
                        ProcessRemoveSEHGroup();
                        break;

                    case OperationCode.JumpTo:
                        ProcessJumpTo(currentCommand);
                        break;

                    case OperationCode.JumpToIfTrue:
                        JumpToIf(1, currentCommand.TargetPosition);
                        break;

                    case OperationCode.JumpToIfFalse:
                        JumpToIf(0, currentCommand.TargetPosition);
                        break;

                    case OperationCode.Await:
                        ProcessAwait();
                        break;

                    case OperationCode.Wait:
                        Wait(currentCommand);
                        break;

                    case OperationCode.CompleteAction:
                        ProcessCompleteAction();
                        break;

                    case OperationCode.BreakActionVal:
                        ProcessBreakActionVal();
                        break;

                    case OperationCode.Error:
                        ProcessError();
                        break;

                    case OperationCode.Return:
                        ProcessReturn();
                        break;

                    case OperationCode.ReturnVal:
                        ProcessReturnVal();
                        break;

                    case OperationCode.SetState:
                        ProcessSetState();
                        break;

                    case OperationCode.SetDefaultState:
                        ProcessSetDefaultState();
                        break;

                    case OperationCode.CompleteState:
                        ProcessCompleteState();
                        break;

                    case OperationCode.BreakStateVal:
                        ProcessBreakStateVal();
                        break;

                    case OperationCode.BreakState:
                        ProcessBreakState();
                        break;

                    case OperationCode.Reject:
                        ProcessReject();
                        break;

                    case OperationCode.Instantiate:
                        ProcessInstantiate(KindOfFunctionParameters.NoParameters, 0);
                        break;

                    case OperationCode.Instantiate_N:
                        ProcessInstantiate(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams);
                        break;

                    case OperationCode.Instantiate_P:
                        ProcessInstantiate(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentCommand.OperationCode), currentCommand.OperationCode, null);
                }

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Error(e);
#endif

                throw;
            }
        }

        private void ProcessInstantiate(KindOfFunctionParameters kindOfParameters, int parametersCount)
        {
            var valuesStack = _currentCodeFrame.ValuesStack;

            var annotationValue = valuesStack.Pop();

            var prototypeValue = valuesStack.Pop();

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParameters(parametersCount, true);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParameters(parametersCount, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            var instanceValue = CreateInstance(prototypeValue);

            var newInstance = instanceValue.AsInstanceValue.InstanceInfo;

            var superClassesStoragesDict = _inheritanceResolver.GetSuperClassStoragesDict(newInstance.LocalCodeExecutionContext.Storage, newInstance);

            var executionsList = new List<(CodeFrame, IExecutionCoordinator)>();

            IExecutable constructor = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    constructor = _constructorsResolver.ResolveWithSelfAndDirectInheritance(newInstance.Name, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    constructor = _constructorsResolver.ResolveWithSelfAndDirectInheritance(newInstance.Name, namedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    constructor = _constructorsResolver.ResolveWithSelfAndDirectInheritance(newInstance.Name, positionedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if (constructor != null)
            {
                var coordinator = ((IExecutable)constructor).GetCoordinator(_context, newInstance.LocalCodeExecutionContext);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(constructor, kindOfParameters, namedParameters, positionedParameters, newInstance.LocalCodeExecutionContext, null);

                executionsList.Add((newCodeFrame, coordinator));
            }

            var preConstructors = _constructorsResolver.ResolvePreConstructors(newInstance.Name, newInstance.LocalCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (preConstructors.Any())
            {
                foreach (var preConstructor in preConstructors)
                {
                    var targetHolder = preConstructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(newInstance.LocalCodeExecutionContext);
                    localCodeExecutionContext.Storage = targetStorage;
                    localCodeExecutionContext.Holder = targetHolder;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage = targetStorage;
                    localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.PreConstructor;

                    var coordinator = ((IExecutable)preConstructor).GetCoordinator(_context, newInstance.LocalCodeExecutionContext);

                    var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(preConstructor, KindOfFunctionParameters.NoParameters, null, null, localCodeExecutionContext, null, true);

                    executionsList.Add((newCodeFrame, coordinator));
                }
            }

            if(executionsList.Any())
            {
                _currentCodeFrame.PutToValueStackArterReturningBack = instanceValue;

                ExecuteCodeFramesBatch(executionsList);
            }
            else
            {

                _currentCodeFrame.ValuesStack.Push(instanceValue);

                _currentCodeFrame.CurrentPosition++;
            }

        }

        private Value CreateInstance(Value prototypeValue)
        {
            prototypeValue = TryResolveFromVarOrExpr(prototypeValue);

            var kindOfValue = prototypeValue.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.CodeItem:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(prototypeValue.AsCodeItem, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                case KindOfValue.StrongIdentifierValue:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(prototypeValue.AsStrongIdentifierValue, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                case KindOfValue.InstanceValue:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(prototypeValue.AsInstanceValue, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                default:
                    throw new Exception($"The vaule {prototypeValue.ToHumanizedString()} can not be instantiated.");
            }            
        }

        private void ProcessReject()
        {
            var localExecutionContext = _currentCodeFrame.LocalContext;

            if (localExecutionContext.Kind != KindOfLocalCodeExecutionContext.AddingFact)
            {
                throw new NotSupportedException();
            }

            localExecutionContext.KindOfAddFactResult = KindOfAddFactOrRuleResult.Reject;

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessBreakState()
        {
            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

            _context.InstancesStorage.TryActivateDefaultState();

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessBreakStateVal()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Peek();

            var ruleInstance = currentValue.AsRuleInstance;

            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

            _globalLogicalStorage.Append(ruleInstance);

            _context.InstancesStorage.TryActivateDefaultState();

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCompleteState()
        {
            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

            _context.InstancesStorage.TryActivateDefaultState();

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetDefaultState()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Pop();

            if (!currentValue.IsStrongIdentifierValue)
            {
                throw new Exception($"Unexpected value '{currentValue.ToSystemString()}'.");
            }

            var stateName = currentValue.AsStrongIdentifierValue;

            _globalStorage.StatesStorage.SetDefaultStateName(stateName);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetState()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Pop();

            if (!currentValue.IsStrongIdentifierValue)
            {
                throw new Exception($"Unexpected value '{currentValue.ToSystemString()}'.");
            }

            var stateName = currentValue.AsStrongIdentifierValue;

            var state = _statesResolver.Resolve(stateName, _currentCodeFrame.LocalContext);

            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

            _context.InstancesStorage.ActivateState(state);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessReturnVal()
        {
            var currentValue = TryResolveFromVarOrExpr(_currentCodeFrame.ValuesStack.Pop());

            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Completed;

            GoBackToPrevCodeFrame(ActionExecutionStatus.Complete);

            if (_currentCodeFrame == null)
            {
                ExternalReturn = currentValue;
            }
            else
            {
                _currentCodeFrame.ValuesStack.Push(currentValue);
            }
        }

        private void ProcessReturn()
        {
            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Completed;

            GoBackToPrevCodeFrame(ActionExecutionStatus.Complete);

            var currentValue = NullValue.Instance;

            if (_currentCodeFrame == null)
            {
                ExternalReturn = currentValue;
            }
            else
            {
                _currentCodeFrame.ValuesStack.Push(currentValue);
            }
        }

        private void ProcessBreakActionVal()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Peek();

            var ruleInstance = currentValue.AsRuleInstance;

            _executionCoordinator.RuleInstance = ruleInstance;

            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCompleteAction()
        {
            _currentCodeFrame.ExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessAwait()
        {
            var coordinator = _currentCodeFrame.ExecutionCoordinator;

            if (coordinator == null)
            {
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            if (coordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                return;
            }

            if (coordinator.ExecutionStatus == ActionExecutionStatus.Broken)
            {
                ProcessError(coordinator.RuleInstance);
                return;
            }

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessJumpTo(ScriptCommand currentCommand)
        {
            _currentCodeFrame.CurrentPosition = currentCommand.TargetPosition;
        }

        private void ProcessRemoveSEHGroup()
        {
            _currentCodeFrame.SEHStack.Pop();

            if (_currentCodeFrame.SEHStack.Count == 0)
            {
                _currentCodeFrame.CurrentSEHGroup = null;
            }
            else
            {
                _currentCodeFrame.CurrentSEHGroup = _currentCodeFrame.SEHStack.Peek();
            }

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetSEHGroup(ScriptCommand currentCommand)
        {
            var targetSEHGroup = _currentCodeFrame.CompiledFunctionBody.SEH[currentCommand.TargetPosition];

            _currentCodeFrame.CurrentSEHGroup = targetSEHGroup;
            _currentCodeFrame.SEHStack.Push(targetSEHGroup);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessExec()
        {
            var currentValue = TryResolveFromVarOrExpr(_currentCodeFrame.ValuesStack.Pop());

            var kindOfCurrentValue = currentValue.KindOfValue;

            switch (kindOfCurrentValue)
            {
                case KindOfValue.RuleInstance:
                    ExecRuleInstanceValue(currentValue.AsRuleInstance);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCurrentValue), kindOfCurrentValue, null);
            }
        }

        private void ProcessCallUnOp(ScriptCommand currentCommand)
        {
            var paramsList = TakePositionedParameters(2);

            var kindOfOperator = currentCommand.KindOfOperator;

            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext);

            CallOperator(operatorInfo, paramsList);

        }

        private void ProcessCallBinOp(ScriptCommand currentCommand)
        {
            var paramsList = TakePositionedParameters(3);

            var kindOfOperator = currentCommand.KindOfOperator;

            if (kindOfOperator == KindOfOperator.IsNot)
            {
                kindOfOperator = KindOfOperator.Is;
            }

            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext);

            CallOperator(operatorInfo, paramsList);

            if (currentCommand.KindOfOperator == KindOfOperator.IsNot)
            {
                var result = _currentCodeFrame.ValuesStack.Pop();

                result = result.AsLogicalValue.Inverse();

                _currentCodeFrame.ValuesStack.Push(result);

            }

        }

        private void ProcessVarDecl(ScriptCommand currentCommand)
        {
            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

            var typesCount = currentCommand.CountParams;

            var varPtr = new Var();

            var annotatioValue = annotation.AsAnnotationValue;
            var annotatedItem = annotatioValue.AnnotatedItem;

            if(annotatedItem is Field)
            {
                var field = (Field)annotatedItem;

                varPtr.Holder = field.Holder;
                varPtr.TypeOfAccess = field.TypeOfAccess;
            }

            while (typesCount > 0)
            {
                var typeName = valueStack.Pop();

                if (!typeName.IsStrongIdentifierValue)
                {
                    throw new Exception($"Typename should be StrongIdentifierValue.");
                }

                varPtr.TypesList.Add(typeName.AsStrongIdentifierValue);

                typesCount--;
            }

            var varName = valueStack.Pop();

            if (!varName.IsStrongIdentifierValue)
            {
                throw new Exception($"Varname should be StrongIdentifierValue.");
            }

            varPtr.Name = varName.AsStrongIdentifierValue;

            _currentVarStorage.Append(varPtr);

            _currentCodeFrame.ValuesStack.Push(varName);
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCodeItemDecl(ScriptCommand currentCommand)
        {
            var valuesStack = _currentCodeFrame.ValuesStack;

            var annotationValue = valuesStack.Pop();

            var prototypeValue = valuesStack.Pop();

            var codeItem = prototypeValue.AsCodeItem;

            _projectLoader.LoadCodeItem(codeItem, _currentCodeFrame.LocalContext.Storage);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessPushVal(ScriptCommand currentCommand)
        {
            var value = currentCommand.Value;

            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.WaypointSourceValue:
                    {
                        var waypointSourceValue = value.AsWaypointSourceValue;

                        value = waypointSourceValue.ConvertToWaypointValue(_context, _currentCodeFrame.LocalContext);
                    }
                    break;

                case KindOfValue.ConditionalEntitySourceValue:
                    {
                        var conditionalEntitySourceValue = value.AsConditionalEntitySourceValue;

                        value = conditionalEntitySourceValue.ConvertToConditionalEntityValue(_context, _currentCodeFrame.LocalContext);
                    }
                    break;

                default:
                    break;
            }

            _currentCodeFrame.ValuesStack.Push(value);
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessClearStack()
        {
            _currentCodeFrame.ValuesStack.Clear();
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessNop()
        {
            _currentCodeFrame.CurrentPosition++;
        }

        private Value TryResolveFromVarOrExpr(Value operand)
        {
            return _valueResolvingHelper.TryResolveFromVarOrExpr(operand, _currentCodeFrame.LocalContext);
        }

        private void Wait(ScriptCommand currentCommand)
        {
            if (_endOfTargetDuration.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTiks;

                var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

                if (currentMilisecond >= _endOfTargetDuration.Value)
                {
                    _endOfTargetDuration = null;
                    _currentCodeFrame.CurrentPosition++;
                    return;
                }

                return;
            }

            if (!_waitedTasksList.IsNullOrEmpty())
            {
                if (_waitedTasksList.Any(p => p.Status == TaskStatus.Running))
                {
                    return;
                }

                _waitedTasksList = null;
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            var annotation = _currentCodeFrame.ValuesStack.Pop();

            var positionedParameters = TakePositionedParameters(currentCommand.CountParams, true);

            if (positionedParameters.Count == 1)
            {
                var firstParameter = positionedParameters[0];

                if (firstParameter.KindOfValue != KindOfValue.TaskValue)
                {
                    var timeoutNumVal = _numberValueLinearResolver.Resolve(firstParameter, _currentCodeFrame.LocalContext);

                    var timeoutSystemVal = timeoutNumVal.SystemValue.Value;

                    var currentTick = _dateTimeProvider.CurrentTiks;

                    var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

                    _endOfTargetDuration = Convert.ToInt64(currentMilisecond + timeoutSystemVal);

                    return;
                }
            }

            if (positionedParameters.Any(p => p.KindOfValue == KindOfValue.TaskValue))
            {
                _waitedTasksList = positionedParameters.Where(p => p.IsTaskValue).Select(p => p.AsTaskValue.SystemTask).ToList();
                return;
            }

            throw new NotImplementedException();
        }

        private void JumpToIf(float targetValue, int targetPosition)
        {
            var currLogicValue = GetLogicalValueFromCurrentStackValue();

            if (currLogicValue == targetValue)
            {
                _currentCodeFrame.CurrentPosition = targetPosition;
            }
            else
            {
                _currentCodeFrame.CurrentPosition++;
            }
        }

        private float? GetLogicalValueFromCurrentStackValue()
        {
            var currentValue = TryResolveFromVarOrExpr(_currentCodeFrame.ValuesStack.Pop());

            var kindOfValue = currentValue.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return currentValue.AsLogicalValue.SystemValue;

                case KindOfValue.NumberValue:
                    return ValueConverter.ConvertNumberValueToLogicalValue(currentValue.AsNumberValue, _context).SystemValue;

                case KindOfValue.StrongIdentifierValue:
                    return ValueConverter.ConvertStrongIdentifierValueToLogicalValue(currentValue.AsStrongIdentifierValue, _context).SystemValue;

                case KindOfValue.RuleInstance:
                    {
                        var ruleInstance = currentValue.AsRuleInstance;

                        var searchOptions = new LogicalSearchOptions();
                        searchOptions.QueryExpression = ruleInstance;
                        searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

                        var searchResult = _logicalSearchResolver.Run(searchOptions);

                        if (searchResult.IsSuccess)
                        {
                            return 1;
                        }

                        return 0;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }

        private void ProcessError()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Peek();

            var ruleInstance = currentValue.AsRuleInstance;

            ProcessError(ruleInstance);

        }

        private void ProcessError(RuleInstance ruleInstance)
        {
            RegisterError(ruleInstance);

            if (_currentCodeFrame.CurrentSEHGroup == null)
            {
                _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                GoBackToPrevCodeFrame(ActionExecutionStatus.Faulted);
            }
            else
            {
                if (!CheckSEH())
                {
                    _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                    GoBackToPrevCodeFrame(ActionExecutionStatus.Faulted);
                }
            }
        }

        private void RegisterError(RuleInstance ruleInstance)
        {
            var errorValue = new ErrorValue(ruleInstance);
            errorValue.CheckDirty();

            _currentError = errorValue;

            _globalLogicalStorage.Append(ruleInstance);
        }

        private bool CheckSEH()
        {
            var ruleInstance = _currentError.RuleInstance;

            var searchOptions = new LogicalSearchOptions();
            searchOptions.TargetStorage = ruleInstance;
            searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

            foreach (var sehItem in _currentCodeFrame.CurrentSEHGroup.Items)
            {
                if (sehItem.Condition != null)
                {
                    searchOptions.QueryExpression = sehItem.Condition;

                    if (!_logicalSearchResolver.IsTruth(searchOptions))
                    {
                        continue;
                    }
                }

                if (sehItem.VariableName != null && !sehItem.VariableName.IsEmpty)
                {
                    _currentVarStorage.SetValue(sehItem.VariableName, _currentError);
                }

                _currentError = null;

                _currentCodeFrame.CurrentPosition = sehItem.TargetPosition;

                return true;
            }

            _currentError = null;

            _currentCodeFrame.CurrentPosition = _currentCodeFrame.CurrentSEHGroup.AfterPosition;

            return true;
        }

        private bool CheckReturnedInfo()
        {
            if (_currentError != null)
            {
                return CheckCurrentError();
            }

            return true;
        }

        private bool CheckCurrentError()
        {
            if (_currentCodeFrame.CurrentSEHGroup == null)
            {
                return false;
            }

            return CheckSEH();
        }

        private void SetUpCurrentCodeFrame(ProcessStatus? lastProcessStatus = null)
        {
            _executionCoordinator = _currentCodeFrame.ExecutionCoordinator;
            _currentInstance = _currentCodeFrame.Instance;
            _currentVarStorage = _currentCodeFrame.LocalContext.Storage.VarStorage;

            if(_currentCodeFrame.PutToValueStackArterReturningBack != null)
            {
                _currentCodeFrame.ValuesStack.Push(_currentCodeFrame.PutToValueStackArterReturningBack);
                _currentCodeFrame.PutToValueStackArterReturningBack = null;
            }

            if(_currentCodeFrame.NeedsExecCallEvent)
            {
                _currentCodeFrame.LastProcessStatus = lastProcessStatus;
            }
        }

        private void GoBackToPrevCodeFrame()
        {
            GoBackToPrevCodeFrame(ActionExecutionStatus.None);
        }

        private void GoBackToPrevCodeFrame(ActionExecutionStatus targetActionExecutionStatus)
        {
#if DEBUG
            //Log($"targetActionExecutionStatus = {targetActionExecutionStatus}");
#endif

            if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                var specialMark = _currentCodeFrame.SpecialMark;

                switch (specialMark)
                {
                    case SpecialMarkOfCodeFrame.None:
                        break;

                    case SpecialMarkOfCodeFrame.MainFrameOfActionInstance:
                        if (targetActionExecutionStatus == ActionExecutionStatus.None)
                        {
                            break;
                        }
                        _executionCoordinator.ExecutionStatus = targetActionExecutionStatus;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(specialMark), specialMark, null);
                }
            }

            var currentProcessInfo = _currentCodeFrame.ProcessInfo;

            ProcessStatus? lastProcessStatus = null;

            var currentProcessInfoStatus = currentProcessInfo.Status;

#if DEBUG
            //Log($"currentProcessInfoStatus = {currentProcessInfoStatus}");
#endif

            if (currentProcessInfoStatus == ProcessStatus.Running)
            {
                switch (targetActionExecutionStatus)
                {
                    case ActionExecutionStatus.Complete:
                        currentProcessInfo.Status = ProcessStatus.Completed;
                        break;

                    case ActionExecutionStatus.Broken:
                    case ActionExecutionStatus.Faulted:
                        currentProcessInfo.Status = ProcessStatus.Faulted;
                        break;

                    case ActionExecutionStatus.WeakCanceled:
                        currentProcessInfo.Status = ProcessStatus.WeakCanceled;
                        break;

                    case ActionExecutionStatus.Canceled:
                        currentProcessInfo.Status = ProcessStatus.Canceled;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetActionExecutionStatus), targetActionExecutionStatus, null);
                }

                lastProcessStatus = currentProcessInfo.Status;
            }
            else
            {
                if(targetActionExecutionStatus == ActionExecutionStatus.WeakCanceled)
                {
                    currentProcessInfo.Status = ProcessStatus.WeakCanceled;
                }
            }

#if DEBUG
            //Log($"lastProcessStatus = {lastProcessStatus}");
#endif

            _codeFrames.Pop();

            if (_codeFrames.Count == 0)
            {
                _currentCodeFrame = null;
            }
            else
            {
                _currentCodeFrame = _codeFrames.Peek();

#if DEBUG
                //Log($"_isCanceled = {_isCanceled}");
#endif

                if (_isCanceled)
                {
                    _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Canceled;

                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return;
                }

                SetUpCurrentCodeFrame(lastProcessStatus);
            }
        }

        private List<Value> TakePositionedParameters(int count, bool resolveValueFromVar = false)
        {
            var result = new List<Value>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                if (resolveValueFromVar)
                {
                    result.Add(TryResolveFromVarOrExpr(valueStack.Pop()));
                }
                else
                {
                    result.Add(valueStack.Pop());
                }
            }

            result.Reverse();

            return result;
        }

        private Dictionary<StrongIdentifierValue, Value> TakeNamedParameters(int count, bool resolveValueFromVar = false)
        {
            var rawParamsList = TakePositionedParameters(count * 2);

            var result = new Dictionary<StrongIdentifierValue, Value>();

            var enumerator = rawParamsList.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var name = enumerator.Current.AsStrongIdentifierValue;

                enumerator.MoveNext();

                var value = enumerator.Current;

                if (resolveValueFromVar)
                {
                    value = TryResolveFromVarOrExpr(value);
                }

                result[name] = value;
            }

            return result;
        }

        private void CallConstructor(KindOfFunctionParameters kindOfParameters, int parametersCount)
        {
            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

            var caller = TryResolveFromVarOrExpr(valueStack.Pop());

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParameters(parametersCount, true);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParameters(parametersCount, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            var constructorName = caller.AsStrongIdentifierValue;

            _currentCodeFrame.CalledCtorsList.Add(constructorName);

            if (constructorName == _defaultCtorName)
            {
                constructorName = _currentCodeFrame.LocalContext.Owner;

            }

            IExecutable constructor = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    constructor = _constructorsResolver.ResolveOnlyOwn(constructorName, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    constructor = _constructorsResolver.ResolveOnlyOwn(constructorName, namedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    constructor = _constructorsResolver.ResolveOnlyOwn(constructorName, positionedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            CallExecutable(constructor, null, kindOfParameters, namedParameters, positionedParameters, annotation, SyncOption.Ctor);
        }

        private void CallDefaultCtors()
        {
            var optionsForInheritanceResolver = new ResolverOptions();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance = true;

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(_currentCodeFrame.LocalContext.Owner, _currentCodeFrame.LocalContext, optionsForInheritanceResolver);

            if(!superClassesList.Any())
            {
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            var targetSuperClassesList = superClassesList.Except(_currentCodeFrame.CalledCtorsList);

            if (!targetSuperClassesList.Any())
            {
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            var executionsList = new List<(CodeFrame, IExecutionCoordinator)>();

            foreach(var targetSuperClass in targetSuperClassesList)
            {
                var constructor = _constructorsResolver.ResolveOnlyOwn(targetSuperClass, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

                if(constructor == null)
                {
                    continue;
                }

                var coordinator = ((IExecutable)constructor).GetCoordinator(_context, _currentCodeFrame.LocalContext);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(constructor, KindOfFunctionParameters.NoParameters, null, null, _currentCodeFrame.LocalContext, null);

                executionsList.Add((newCodeFrame, coordinator));
            }

            ExecuteCodeFramesBatch(executionsList);
        }

        private void ProcessExecCallEvent()
        {
            var currentCodeFrame = _currentCodeFrame;

            if (_pseudoSyncTask == null)
            {
                if (!currentCodeFrame.NeedsExecCallEvent)
                {
                    _currentCodeFrame.CurrentPosition++;
                    return;
                }

                var lastProcessStatus = currentCodeFrame.LastProcessStatus.Value;

#if DEBUG
                //Log($"lastProcessStatus = {lastProcessStatus}");
#endif

                switch (lastProcessStatus)
                {
                    case ProcessStatus.Completed:
                        {
                            var completeAnnotationSystemEvent = currentCodeFrame.CompleteAnnotationSystemEvent;

                            if (completeAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(completeAnnotationSystemEvent);
                            }
                        }
                        break;

                    case ProcessStatus.WeakCanceled:
                        {
                            var weakCancelAnnotationSystemEvent = currentCodeFrame.WeakCancelAnnotationSystemEvent;

                            if(weakCancelAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(weakCancelAnnotationSystemEvent);
                            }
                        }
                        break;

                    default:
                        break;
                }

                currentCodeFrame.NeedsExecCallEvent = false;
                currentCodeFrame.LastProcessStatus = null;
                currentCodeFrame.CompleteAnnotationSystemEvent = null;
                currentCodeFrame.CancelAnnotationSystemEvent = null;
                currentCodeFrame.WeakCancelAnnotationSystemEvent = null;
                currentCodeFrame.ErrorAnnotationSystemEvent = null;

                return;
            }

            if (_pseudoSyncTask.Status == TaskStatus.Running)
            {
                return;
            }

            _pseudoSyncTask = null;

            _currentCodeFrame.CurrentPosition++;
        }

        private void ExecCallEvent(AnnotationSystemEvent annotationSystemEvent)
        {
            var coordinator = ((IExecutable)annotationSystemEvent).GetCoordinator(_context, _currentCodeFrame.LocalContext);

            if (coordinator == null)
            {
                coordinator = _currentCodeFrame.ExecutionCoordinator;
            }

            var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(annotationSystemEvent, KindOfFunctionParameters.NoParameters, null, null, _currentCodeFrame.LocalContext, null, true);

            if (annotationSystemEvent.IsSync)
            {
                ExecuteCodeFrame(newCodeFrame, coordinator, SyncOption.PseudoSync);
            }
            else
            {
                ExecuteCodeFrame(newCodeFrame, coordinator, SyncOption.ChildAsync);
            }
        }

        private enum SyncOption
        {
            Sync,
            IndependentAsync,
            ChildAsync,
            Ctor,
            PseudoSync
        }

        private void CallFunction(KindOfFunctionParameters kindOfParameters, int parametersCount, SyncOption syncOption)
        {
#if DEBUG
            Log($"kindOfParameters = {kindOfParameters}");
            Log($"parametersCount = {parametersCount}");
            Log($"syncOption = {syncOption}");
            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

            var caller = TryResolveFromVarOrExpr(valueStack.Pop());

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParameters(parametersCount, true);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParameters(parametersCount, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if (caller.IsPointRefValue)
            {
                CallPointRefValue(caller.AsPointRefValue, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
                return;
            }

            if (caller.IsStrongIdentifierValue)
            {
                CallStrongIdentifierValue(caller.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption, true);
                return;
            }

            if(caller.IsInstanceValue)
            {
                CallInstanceValue(caller.AsInstanceValue, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
                return;
            }

            throw new NotImplementedException();
        }

        private void CallInstanceValue(InstanceValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption)
        {
            var executable = caller.GetExecutable(kindOfParameters, namedParameters, positionedParameters);

            CallExecutable(executable, executable.OwnLocalCodeExecutionContext, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
        }

        private void CallPointRefValue(PointRefValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption)
        {
            var callerLeftOperand = caller.LeftOperand;
            var callerRightOperand = caller.RightOperand;

            if (callerLeftOperand.IsHostValue)
            {
                CallHost(callerRightOperand.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
                return;
            }

            if(callerLeftOperand.IsStrongIdentifierValue)
            {
                throw new NotImplementedException();
            }

            var methodName = callerRightOperand.AsStrongIdentifierValue;

            var method = callerLeftOperand.GetMethod(methodName, kindOfParameters, namedParameters, positionedParameters);

            CallExecutable(method, null, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
        }

        private void CallHost(StrongIdentifierValue methodName, 
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption)
        {
            var command = new Command();
            command.Name = methodName;

            switch(kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    command.ParamsDict = namedParameters.ToDictionary(p => p.Key, p => p.Value);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    command.ParamsList = positionedParameters.ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }


            AnnotationSystemEvent completeAnnotationSystemEvent = null;
            AnnotationSystemEvent cancelAnnotationSystemEvent = null;
            AnnotationSystemEvent weakCancelAnnotationSystemEvent = null;
            AnnotationSystemEvent errorAnnotationSystemEvent = null;

            if (annotation != null)
            {
                var annotationSystemEventsDict = AnnotationsHelper.GetAnnotationSystemEventsDictFromAnnotaion(annotation);

                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Complete, out completeAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Cancel, out cancelAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.WeakCancel, out weakCancelAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Error, out errorAnnotationSystemEvent);
            }

            var processCreatingResult = _hostListener.CreateProcess(command, _context, _currentCodeFrame.LocalContext);

            if(processCreatingResult.IsSuccessful)
            {
                var processInfo = processCreatingResult.Process;

                _instancesStorage.AppendAndTryStartProcessInfo(processInfo);

                var timeout = GetTimeoutFromAnnotation(annotation);

                if (syncOption == SyncOption.Sync)
                {
                    processInfo.ParentProcessInfo = _currentCodeFrame.ProcessInfo;

                    List<IExecutionCoordinator> executionCoordinators = null;

                    if (_executionCoordinator != null)
                    {
                        executionCoordinators = new List<IExecutionCoordinator>() { _executionCoordinator };
                    }

                    ProcessInfoHelper.Wait(executionCoordinators, timeout, _dateTimeProvider, processInfo);

                    if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                    {
                        ProcessError(_executionCoordinator.RuleInstance);

                        return;
                    }

                    var status = processInfo.Status;

#if DEBUG
                    Log($"[{methodName.ToHumanizedString()}] status = {status}");
#endif

                    switch (status)
                    {
                        case ProcessStatus.Completed:
                            _currentCodeFrame.ValuesStack.Push(NullValue.Instance);

                            if(completeAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(completeAnnotationSystemEvent);
                            }
                            break;

                        case ProcessStatus.WeakCanceled:
                            _currentCodeFrame.ValuesStack.Push(NullValue.Instance);

                            if (weakCancelAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(weakCancelAnnotationSystemEvent);
                            }
                            break;

                        case ProcessStatus.Canceled:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Canceled;

                            if (cancelAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(cancelAnnotationSystemEvent);
                            }

                            _isCanceled = true;

                            GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                            return;

                        case ProcessStatus.Faulted:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                            if (errorAnnotationSystemEvent != null)
                            {
                                throw new NotImplementedException();
                            }

                            GoBackToPrevCodeFrame(ActionExecutionStatus.Faulted);
                            return;
                    }
                }
                else
                {
                    if(syncOption == SyncOption.ChildAsync)
                    {
                        processInfo.ParentProcessInfo = _currentCodeFrame.ProcessInfo;
                    }

                    if (completeAnnotationSystemEvent != null)
                    {
                        var currCodeFrame = _currentCodeFrame;

                        processInfo.OnComplete += (procI) =>
                        {
                            var completeAnnotationSystemEventCoordinator = ((IExecutable)completeAnnotationSystemEvent).GetCoordinator(_context, currCodeFrame.LocalContext);

                            var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(completeAnnotationSystemEvent, KindOfFunctionParameters.NoParameters, null, null, currCodeFrame.LocalContext, null, true);

                            ExecuteCodeFrame(newCodeFrame, currCodeFrame, completeAnnotationSystemEventCoordinator, SyncOption.ChildAsync);
                        };
                    }

                    if (cancelAnnotationSystemEvent != null)
                    {
                        throw new NotImplementedException();
                    }

                    if (weakCancelAnnotationSystemEvent != null)
                    {
                        throw new NotImplementedException();
                    }

                    if (errorAnnotationSystemEvent != null)
                    {
                        throw new NotImplementedException();
                    }
                }

                _currentCodeFrame.CurrentPosition++;

                return;
            }

            throw new NotImplementedException();
        }

        private void CallStrongIdentifierValue(StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption, bool mayCallHost)
        {
            IExecutable method = null;

            switch(kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    method = _methodsResolver.Resolve(methodName, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    method = _methodsResolver.Resolve(methodName, namedParameters, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    method = _methodsResolver.Resolve(methodName, positionedParameters, _currentCodeFrame.LocalContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if(method == null)
            {
                if(mayCallHost)
                {
                    CallHost(methodName, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
                    return;
                }

                throw new Exception($"Method '{methodName.NameValue}' is not found.");
            }

            CallExecutable(method, null, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
        }

        private void ExecRuleInstanceValue(RuleInstance ruleInstance)
        {
            var compiledCode = _converterFactToImperativeCode.Convert(ruleInstance, _currentCodeFrame.LocalContext);

            var codeFrame = _codeFrameService.ConvertCompiledFunctionBodyToCodeFrame(compiledCode, _currentCodeFrame.LocalContext);

            ExecuteCodeFrame(codeFrame, null, SyncOption.Sync);
        }

        private long? GetTimeoutFromAnnotation(Value annotation)
        {
            var numberValue = GetSettingsFromAnnotation(annotation, _timeoutName);

            if(numberValue == null)
            {
                return null;
            }

            return Convert.ToInt64(numberValue.SystemValue.Value);
        }

        private float? GetPriorityFromAnnotation(Value annotation)
        {
            var numberValue = GetSettingsFromAnnotation(annotation, _priorityName);

            if (numberValue == null)
            {
                return null;
            }

            return Convert.ToSingle(numberValue.SystemValue.Value);
        }

        private NumberValue GetSettingsFromAnnotation(Value annotation, StrongIdentifierValue settingName)
        {
            if (annotation == null)
            {
                return null;
            }

            var localContext = _currentCodeFrame.LocalContext;

            var initialValue = _annotationsResolver.GetSettings(annotation, settingName, localContext);

            if (initialValue == null || initialValue.KindOfValue == KindOfValue.NullValue)
            {
                return null;
            }

            var numberValue = _numberValueLinearResolver.Resolve(initialValue, localContext);

            if (numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue)
            {
                return null;
            }

            return numberValue;
        }

        private void CallOperator(Operator op, List<Value> positionedParameters)
        {
            CallExecutable(op, positionedParameters);
        }

        private void CallExecutable(IExecutable executable, List<Value> positionedParameters)
        {
            CallExecutable(executable, null, KindOfFunctionParameters.PositionedParameters, null, positionedParameters, null, SyncOption.Sync);
        }

        private void CallExecutable(IExecutable executable, ILocalCodeExecutionContext ownLocalCodeExecutionContext, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, Value annotation, SyncOption syncOption)
        {
            if(executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var targetLocalContext = ownLocalCodeExecutionContext == null? _currentCodeFrame.LocalContext : ownLocalCodeExecutionContext;

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch(kindOfParameters)
                {
                    case KindOfFunctionParameters.NoParameters:
                        result = executable.SystemHandler.Call(new List<Value>(), targetLocalContext);
                        break;

                    case KindOfFunctionParameters.PositionedParameters:
                        result = executable.SystemHandler.Call(positionedParameters, targetLocalContext);
                        break;

                    case KindOfFunctionParameters.NamedParameters:
                        result = executable.SystemHandler.Call(namedParameters.ToDictionary(p => p.Key.NameValue, p => p.Value), annotation, targetLocalContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }
                
                _currentCodeFrame.ValuesStack.Push(result);

                _currentCodeFrame.CurrentPosition++;

                return;
            }
            else
            {
                AnnotationSystemEvent completeAnnotationSystemEvent = null;
                AnnotationSystemEvent cancelAnnotationSystemEvent = null;
                AnnotationSystemEvent weakCancelAnnotationSystemEvent = null;
                AnnotationSystemEvent errorAnnotationSystemEvent = null;

                if (annotation != null)
                {
                    var annotationSystemEventsDict = AnnotationsHelper.GetAnnotationSystemEventsDictFromAnnotaion(annotation);

                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Complete, out completeAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Cancel, out cancelAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.WeakCancel, out weakCancelAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Error, out errorAnnotationSystemEvent);
                }

                if (executable.NeedActivation && !executable.IsActivated)
                {
                    executable = executable.Activate(_context, _currentCodeFrame.LocalContext);
                }

                var coordinator = executable.GetCoordinator(_context, _currentCodeFrame.LocalContext);

                if (executable.UsingLocalCodeExecutionContextPreferences == UsingLocalCodeExecutionContextPreferences.UseOwnAsParent || executable.UsingLocalCodeExecutionContextPreferences == UsingLocalCodeExecutionContextPreferences.UseBothOwnAndCallerAsParent)
                {
                    targetLocalContext = executable.OwnLocalCodeExecutionContext;
                }

                var additionalSettings = GetAdditionalSettingsFromAnnotation(annotation, ownLocalCodeExecutionContext);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(executable, kindOfParameters, namedParameters, positionedParameters, targetLocalContext, additionalSettings);

                ExecuteCodeFrame(newCodeFrame, coordinator, syncOption, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
            }
        }

        private void ExecuteCodeFramesBatch(List<(CodeFrame, IExecutionCoordinator)> executionsList)
        {
            if (!executionsList.Any())
            {
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            _currentCodeFrame.CurrentPosition++;

            foreach (var item in executionsList)
            {
                var codeFrame = item.Item1;
                var coordinator = item.Item2;

                _context.InstancesStorage.AppendProcessInfo(codeFrame.ProcessInfo);

                PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                SetCodeFrame(codeFrame);
            }
        }

        private ConversionExecutableToCodeFrameAdditionalSettings GetAdditionalSettingsFromAnnotation(Value annotation, ILocalCodeExecutionContext ownLocalCodeExecutionContext)
        {
            var timeout = GetTimeoutFromAnnotation(annotation);
            var priority = GetPriorityFromAnnotation(annotation);

            ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null;

            if (timeout.HasValue || priority.HasValue)
            {
                additionalSettings = new ConversionExecutableToCodeFrameAdditionalSettings()
                {
                    Timeout = timeout,
                    Priority = priority,
                    AllowParentLocalStorages = ownLocalCodeExecutionContext == null ? false : true
                };
            }
            else
            {
                if (ownLocalCodeExecutionContext != null)
                {
                    additionalSettings = new ConversionExecutableToCodeFrameAdditionalSettings()
                    {
                        AllowParentLocalStorages = true
                    };
                }
            }

            return additionalSettings;
        }

        private void ExecuteCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            ExecuteCodeFrame(codeFrame, null, coordinator, syncOption, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
        }

        private void ExecuteCodeFrame(CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            var targetCurrentCodeFrame = _currentCodeFrame;

            if(currentCodeFrame != null)
            {
                targetCurrentCodeFrame = currentCodeFrame;
            }

            _context.InstancesStorage.AppendProcessInfo(codeFrame.ProcessInfo);

            switch (syncOption)
            {
                case SyncOption.Sync:
                case SyncOption.Ctor:
                    PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                    targetCurrentCodeFrame.CurrentPosition++;

                    if (completeAnnotationSystemEvent != null)
                    {
                        targetCurrentCodeFrame.NeedsExecCallEvent = true;
                        targetCurrentCodeFrame.CompleteAnnotationSystemEvent = completeAnnotationSystemEvent;
                    }

                    if (cancelAnnotationSystemEvent != null)
                    {
                        targetCurrentCodeFrame.NeedsExecCallEvent = true;
                        targetCurrentCodeFrame.CancelAnnotationSystemEvent = cancelAnnotationSystemEvent;
                    }

                    if(weakCancelAnnotationSystemEvent != null)
                    {
                        targetCurrentCodeFrame.NeedsExecCallEvent = true;
                        targetCurrentCodeFrame.WeakCancelAnnotationSystemEvent = weakCancelAnnotationSystemEvent;
                    }

                    if(errorAnnotationSystemEvent != null)
                    {
                        targetCurrentCodeFrame.NeedsExecCallEvent = true;
                        targetCurrentCodeFrame.ErrorAnnotationSystemEvent = errorAnnotationSystemEvent;
                    }

                    SetCodeFrame(codeFrame);
                    break;

                case SyncOption.IndependentAsync:
                case SyncOption.ChildAsync:
                    {
                        targetCurrentCodeFrame.CurrentPosition++;

                        if (syncOption == SyncOption.ChildAsync)
                        {
                            targetCurrentCodeFrame.ProcessInfo.AddChild(codeFrame.ProcessInfo);
                        }

                        var threadExecutor = new AsyncThreadExecutor(_context);
                        threadExecutor.SetCodeFrame(codeFrame);

                        var task = threadExecutor.Start();

                        var taskValue = task.AsTaskValue;

                        if (completeAnnotationSystemEvent != null)
                        {
                            taskValue.OnComplete += () =>
                            {
                                var annotationSystemEventCoordinator = ((IExecutable)completeAnnotationSystemEvent).GetCoordinator(_context, targetCurrentCodeFrame.LocalContext);

                                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(completeAnnotationSystemEvent, KindOfFunctionParameters.NoParameters, null, null, targetCurrentCodeFrame.LocalContext, null, true);

                                ExecuteCodeFrame(newCodeFrame, targetCurrentCodeFrame, annotationSystemEventCoordinator, SyncOption.ChildAsync);
                            };
                        }

                        if (cancelAnnotationSystemEvent != null)
                        {
                            throw new NotImplementedException();
                        }

                        if (weakCancelAnnotationSystemEvent != null)
                        {
                            taskValue.OnComplete += () =>
                            {
                                var codeFrameProcessInfoStatus = codeFrame.ProcessInfo.Status;

#if DEBUG
                                //Log($"codeFrameProcessInfoStatus = {codeFrameProcessInfoStatus}");
                                //Log($"codeFrame.ProcessInfo.GetType().FullName = {codeFrame.ProcessInfo.GetType().FullName}");
#endif

                                if (codeFrameProcessInfoStatus != ProcessStatus.WeakCanceled)
                                {
                                    return;
                                }

                                var annotationSystemEventCoordinator = ((IExecutable)weakCancelAnnotationSystemEvent).GetCoordinator(_context, targetCurrentCodeFrame.LocalContext);

                                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(weakCancelAnnotationSystemEvent, KindOfFunctionParameters.NoParameters, null, null, targetCurrentCodeFrame.LocalContext, null, true);

                                ExecuteCodeFrame(newCodeFrame, targetCurrentCodeFrame, annotationSystemEventCoordinator, SyncOption.ChildAsync);
                            };
                        }

                        if (errorAnnotationSystemEvent != null)
                        {
                            throw new NotImplementedException();
                        }

                        targetCurrentCodeFrame.ValuesStack.Push(task);
                    }
                    break;

                case SyncOption.PseudoSync:
                    {
                        PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                        var threadExecutor = new AsyncThreadExecutor(_context);
                        threadExecutor.SetCodeFrame(codeFrame);

                        var task = threadExecutor.Start();

                        _pseudoSyncTask = task.AsTaskValue.SystemTask;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(syncOption), syncOption, null);
            }
        }

        private void PrepareCodeFrameToSyncExecution(CodeFrame codeFrame, IExecutionCoordinator coordinator)
        {
            if (coordinator != null)
            {
                var instance = coordinator.Instance;

                if (_currentInstance != null && instance != null)
                {
                    SetSpecialMarkOfCodeFrame(codeFrame, coordinator);

                    _currentInstance.AddChildInstance(instance);
                }
            }

            codeFrame.ExecutionCoordinator = coordinator;
        }

        private void SetSpecialMarkOfCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator)
        {
            if(coordinator == null)
            {
                return;
            }

            var kindOfInstance = coordinator.KindOfInstance;

            switch (kindOfInstance)
            {
                case KindOfInstance.ActionInstance:
                    codeFrame.SpecialMark = SpecialMarkOfCodeFrame.MainFrameOfActionInstance;
                    break;

                case KindOfInstance.AppInstance:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfInstance), kindOfInstance, null);
            }
        }

        private void ProcessSetInheritance()
        {
            var paramsList = TakePositionedParameters(4);

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = _strongIdentifierLinearResolver.Resolve(paramsList[0], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(paramsList[1], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var rank = paramsList[2];//_logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true);

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetNotInheritance()
        {
            var paramsList = TakePositionedParameters(4);

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

            var rank = _logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true).Inverse();

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }
    }
}
