/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            _logicalSearchResolver = dataResolversFactory.GetLogicalSearchResolver();
            _statesResolver = dataResolversFactory.GetStatesResolver();
            _annotationsResolver = dataResolversFactory.GetAnnotationsResolver();

            _converterFactToImperativeCode = context.ConvertersFactory.GetConverterFactToImperativeCode();
            _dateTimeProvider = context.DateTimeProvider;
        }

        private readonly IEngineContext _context;

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
        private readonly LogicalSearchResolver _logicalSearchResolver;
        private readonly StatesResolver _statesResolver;
        private readonly AnnotationsResolver _annotationsResolver;

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

        public Value ExternalReturn { get; private set; }

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame, bool setAsRunning = true)
        {
#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            var timeout = codeFrame.TargetDuration;

            if (timeout.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTiks;

#if DEBUG
                //Log($"currentTick = {currentTick}");
#endif

                var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

#if DEBUG
                //Log($"currentMilisecond = {currentMilisecond}");
#endif

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

#if DEBUG
            //_instancesStorage.PrintProcessesList();
#endif
        }

        public Value Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
#if DEBUG
            //Log("Dispose()");
#endif

            _activeObject.Dispose();
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
            try
            {
#if DEBUG
                //Log("Begin");
#endif

                if (_currentCodeFrame == null)
                {
#if DEBUG
                    //Log("_currentCodeFrame == null return false;");
#endif

                    return false;
                }

#if DEBUG
                //Log($"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");
#endif

                if (cancellationToken.IsCancellationRequested)
                {
                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

                if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing)
                {
#if DEBUG
                    //Log("_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing; return false;");
#endif

                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

#if DEBUG
                //Log($"_currentCodeFrame.ProcessInfo.Status = {_currentCodeFrame.ProcessInfo.Status}");
#endif

                if (_currentCodeFrame.ProcessInfo.Status == ProcessStatus.Canceled)
                {
#if DEBUG
                    //Log($"_currentCodeFrame.ProcessInfo.Status == ProcessStatus.Canceled");
#endif

                    GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                    return true;
                }

                var endOfTargetDuration = _currentCodeFrame.EndOfTargetDuration;

                if (endOfTargetDuration.HasValue)
                {
#if DEBUG
                    //Log($"endOfTargetDuration = {endOfTargetDuration}");
#endif

                    var currentTick = _dateTimeProvider.CurrentTiks;

#if DEBUG
                    //Log($"currentTick = {currentTick}");
#endif

                    var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

#if DEBUG
                    //Log($"currentMilisecond = {currentMilisecond}");
#endif

                    if (currentMilisecond >= endOfTargetDuration.Value)
                    {
#if DEBUG
                        //Log($"currentMilisecond >= endOfTargetDuration.Value");
#endif

                        GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                        return true;
                    }
                }

                var currentPosition = _currentCodeFrame.CurrentPosition;

#if DEBUG
                //Log($"currentPosition = {currentPosition}");
#endif

                var compiledFunctionBodyCommands = _currentCodeFrame.CompiledFunctionBody.Commands;

#if DEBUG
                //Log($"compiledFunctionBodyCommands.Count = {compiledFunctionBodyCommands.Count}");
#endif

                if (currentPosition >= compiledFunctionBodyCommands.Count)
                {
#if DEBUG
                    //Log("currentPosition >= compiledFunctionBodyCommands.Count return true;");
                    //Log($"_currentCodeFrame.ExecutionCoordinator?.ExecutionStatus = {_currentCodeFrame.ExecutionCoordinator?.ExecutionStatus}");
#endif

                    GoBackToPrevCodeFrame(ActionExecutionStatus.Complete);
                    return true;
                }

                if (!CheckReturnedInfo())
                {
#if DEBUG
                    //Log("!CheckReturnedInfo()");
#endif

                    return false;
                }

                var currentCommand = compiledFunctionBodyCommands[currentPosition];

#if DEBUG
                //Log($"currentCommand = {currentCommand}");
                Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                switch (currentCommand.OperationCode)
                {
                    case OperationCode.Nop:
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.ClearStack:
                        _currentCodeFrame.ValuesStack.Clear();
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.PushVal:
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

#if DEBUG
                            //Log($"value = {value?.ToHumanizedString()}");
#endif

                            _currentCodeFrame.ValuesStack.Push(value);
                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.VarDecl:
                        {
                            var valueStack = _currentCodeFrame.ValuesStack;

                            var annotation = valueStack.Pop();

#if DEBUG
                            //Log($"annotation = {annotation}");
#endif

                            var typesCount = currentCommand.CountParams;

#if DEBUG
                            //Log($"typesCount = {typesCount}");
#endif

                            var varPtr = new Var();

                            while (typesCount > 0)
                            {
                                var typeName = valueStack.Pop();

#if DEBUG
                                //Log($"typeName = {typeName}");
#endif

                                if (!typeName.IsStrongIdentifierValue)
                                {
                                    throw new Exception($"Typename should be StrongIdentifierValue.");
                                }

                                varPtr.TypesList.Add(typeName.AsStrongIdentifierValue);

                                typesCount--;
                            }

                            var varName = valueStack.Pop();

#if DEBUG
                            //Log($"varName = {varName}");
#endif

                            if (!varName.IsStrongIdentifierValue)
                            {
                                throw new Exception($"Varname should be StrongIdentifierValue.");
                            }

                            varPtr.Name = varName.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"varPtr = {varPtr}");
#endif

                            _currentVarStorage.Append(varPtr);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
                            _currentCodeFrame.ValuesStack.Push(varName);
                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CallBinOp:
                        {
                            var paramsList = TakePositionedParameters(3);

#if DEBUG
                            //Log($"paramsList = {paramsList.WriteListToString()}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var kindOfOperator = currentCommand.KindOfOperator;

                            if (kindOfOperator == KindOfOperator.IsNot)
                            {
                                kindOfOperator = KindOfOperator.Is;
                            }

#if DEBUG
                            //Log($"kindOfOperator = {kindOfOperator}");
#endif

                            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext);

#if DEBUG
                            //Log($"operatorInfo (1) = {operatorInfo}");
#endif

                            CallOperator(operatorInfo, paramsList);

#if DEBUG
                            //Log($"_currentCodeFrame (^) = {_currentCodeFrame.ToDbgString()}");
#endif

                            if (currentCommand.KindOfOperator == KindOfOperator.IsNot)
                            {
                                var result = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                                //Log($"result = {result}");
#endif

                                result = result.AsLogicalValue.Inverse();

#if DEBUG
                                //Log($"result (2) = {result}");
#endif

                                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                                //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
                            }

                            //_currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CallUnOp:
                        {
                            var paramsList = TakePositionedParameters(2);

#if DEBUG
                            //Log($"paramsList = {paramsList.WriteListToString()}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var kindOfOperator = currentCommand.KindOfOperator;

                            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext);

#if DEBUG
                            //Log($"operatorInfo (2)= {operatorInfo}");
#endif

                            CallOperator(operatorInfo, paramsList);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            //_currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Call:
                        CallFunction(KindOfFunctionParameters.NoParameters, currentCommand.CountParams, SyncOption.Sync);
                        break;

                    case OperationCode.Call_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.Sync);
                        break;

                    case OperationCode.Call_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.Sync);
                        break;

                    case OperationCode.AsyncCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, currentCommand.CountParams, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.IndependentAsync);
                        break;

                    case OperationCode.AsyncChildCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, currentCommand.CountParams, SyncOption.ChildAsync);
                        break;

                    case OperationCode.AsyncChildCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.ChildAsync);
                        break;

                    case OperationCode.AsyncChildCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.ChildAsync);
                        break;

                    case OperationCode.Exec:
                        {
                            var currentValue = TryResolveFromVar(_currentCodeFrame.ValuesStack.Pop());

#if DEBUG
                            //Log($"currentValue = {currentValue.ToHumanizedString()}");
#endif

                            var kindOfCurrentValue = currentValue.KindOfValue;

#if DEBUG
                            //Log($"kindOfCurrentValue = {kindOfCurrentValue}");
#endif

                            switch (kindOfCurrentValue)
                            {
                                case KindOfValue.RuleInstanceValue:
                                    ExecRuleInstanceValue(currentValue.AsRuleInstanceValue);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(kindOfCurrentValue), kindOfCurrentValue, null);
                            }
                        }
                        break;

                    case OperationCode.SetInheritance:
                        ProcessSetInheritance();
                        break;

                    case OperationCode.SetNotInheritance:
                        ProcessSetNotInheritance();
                        break;

                    case OperationCode.SetSEHGroup:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

                            var targetSEHGroup = _currentCodeFrame.CompiledFunctionBody.SEH[currentCommand.TargetPosition];

#if DEBUG
                            //Log($"targetSEHGroup = {targetSEHGroup}");
#endif

                            _currentCodeFrame.CurrentSEHGroup = targetSEHGroup;
                            _currentCodeFrame.SEHStack.Push(targetSEHGroup);

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.RemoveSEHGroup:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

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
                        break;

                    case OperationCode.JumpTo:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

                            _currentCodeFrame.CurrentPosition = currentCommand.TargetPosition;
                        }
                        break;

                    case OperationCode.JumpToIfTrue:
                        JumpToIf(1, currentCommand.TargetPosition);
                        break;

                    case OperationCode.JumpToIfFalse:
                        JumpToIf(0, currentCommand.TargetPosition);
                        break;

                    case OperationCode.Await:
                        {
                            var coordinator = _currentCodeFrame.ExecutionCoordinator;

#if DEBUG
                            //Log($"coordinator = {coordinator}");
#endif

                            if (coordinator == null)
                            {
                                _currentCodeFrame.CurrentPosition++;
                                break;
                            }

                            if (coordinator.ExecutionStatus == ActionExecutionStatus.Executing)
                            {
                                break;
                            }

                            if (coordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                            {
                                ProcessError(coordinator.RuleInstance);
                                break;
                            }

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Wait:
                        Wait(currentCommand);
                        break;

                    case OperationCode.CompleteAction:
                        {
                            _currentCodeFrame.ExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.BreakActionVal:
                        {
                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                            _executionCoordinator.RuleInstance = ruleInstance;

                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Error:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.Error");
#endif

#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                            ProcessError(ruleInstance);

#if DEBUG
                            //Log("End case OperationCode.Error");
#endif
                        }
                        break;

                    case OperationCode.Return:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.Return");
#endif

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

#if DEBUG
                            //_instancesStorage.PrintProcessesList();

                            //Log("End case OperationCode.Return");
#endif
                        }
                        break;

                    case OperationCode.ReturnVal:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.ReturnVal");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var currentValue = TryResolveFromVar(_currentCodeFrame.ValuesStack.Pop());

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

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

#if DEBUG
                            //_instancesStorage.PrintProcessesList();

                            //Log("End case OperationCode.Return");
#endif
                        }
                        break;

                    case OperationCode.SetState:
                        {
                            var currentValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            if (!currentValue.IsStrongIdentifierValue)
                            {
                                throw new Exception($"Unexpected value '{currentValue.ToSystemString()}'.");
                            }

                            var stateName = currentValue.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"stateName = {stateName}");
#endif

                            var state = _statesResolver.Resolve(stateName, _currentCodeFrame.LocalContext);

#if DEBUG
                            //Log($"state = {state}");
#endif

#if DEBUG
                            //Log($"(_stateExecutionCoordinator != null) = {_stateExecutionCoordinator != null}");
#endif

                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

                            _context.InstancesStorage.ActivateState(state);

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.SetDefaultState:
                        {
                            var currentValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            if (!currentValue.IsStrongIdentifierValue)
                            {
                                throw new Exception($"Unexpected value '{currentValue.ToSystemString()}'.");
                            }

                            var stateName = currentValue.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"stateName = {stateName}");
#endif

                            _globalStorage.StatesStorage.SetDefaultStateName(stateName);

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CompleteState:
                        {
                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

                            _context.InstancesStorage.TryActivateDefaultState();

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.BreakStateVal:
                        {
                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

                            _globalLogicalStorage.Append(ruleInstance);

                            _context.InstancesStorage.TryActivateDefaultState();

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.BreakState:
                        {
                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

                            _context.InstancesStorage.TryActivateDefaultState();

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Reject:
                        {
                            var localExecutionContext = _currentCodeFrame.LocalContext;

#if DEBUG
                            //Log($"localExecutionContext = {localExecutionContext}");
#endif

                            if (localExecutionContext.Kind != KindOfLocalCodeExecutionContext.AddingFact)
                            {
                                throw new NotSupportedException();
                            }

                            localExecutionContext.KindOfAddFactResult = KindOfAddFactOrRuleResult.Reject;

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Instantiate:
                        {
                            throw new NotImplementedException();
                        }
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

        private Value TryResolveFromVar(Value operand)
        {
#if DEBUG
            //Log($"operand = {operand}");
#endif

            return ValueResolvingHelper.TryResolveFromVar(operand, _currentCodeFrame.LocalContext, _varsResolver);
        }

        private void Wait(ScriptCommand currentCommand)
        {
#if DEBUG
            //Log($"_endOfTargetTimeout = {_endOfTargetTimeout}");
#endif

            if (_endOfTargetDuration.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTiks;

#if DEBUG
                //Log($"currentTick = {currentTick}");
#endif

                var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

#if DEBUG
                //Log($"currentMilisecond = {currentMilisecond}");
#endif

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

#if DEBUG
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            if (positionedParameters.Count == 1)
            {
                var firstParameter = positionedParameters[0];

#if DEBUG
                //Log($"firstParameter = {firstParameter}");
#endif
                if (firstParameter.KindOfValue != KindOfValue.TaskValue)
                {
#if DEBUG
                    //Log($"firstParameter = {firstParameter}");
#endif

                    var timeoutNumVal = _numberValueLinearResolver.Resolve(firstParameter, _currentCodeFrame.LocalContext);

#if DEBUG
                    //Log($"timeoutNumVal = {timeoutNumVal}");
#endif

                    var timeoutSystemVal = timeoutNumVal.SystemValue.Value;

#if DEBUG
                    //Log($"timeoutSystemVal = {timeoutSystemVal}");
#endif

                    var currentTick = _dateTimeProvider.CurrentTiks;

#if DEBUG
                    //Log($"currentTick = {currentTick}");
#endif

                    var currentMilisecond = currentTick * _dateTimeProvider.MillisecondsMultiplicator;

#if DEBUG
                    //Log($"currentMilisecond = {currentMilisecond}");
#endif

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
#if DEBUG
            //Log($"targetValue = {targetValue}");
#endif

            var currLogicValue = GetLogicalValueFromCurrentStackValue();

#if DEBUG
            //Log($"currLogicValue = {currLogicValue}");
#endif

            if (currLogicValue == targetValue)
            {
#if DEBUG
                //Log("currLogicValue == targetValue");
#endif

                _currentCodeFrame.CurrentPosition = targetPosition;
            }
            else
            {
#if DEBUG
                //Log("currLogicValue != targetValue");
#endif

                _currentCodeFrame.CurrentPosition++;
            }
        }

        private float? GetLogicalValueFromCurrentStackValue()
        {
            var currentValue = TryResolveFromVar(_currentCodeFrame.ValuesStack.Pop());

#if DEBUG
            //Log($"currentValue = {currentValue}");
#endif

            var kindOfValue = currentValue.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return currentValue.AsLogicalValue.SystemValue;

                case KindOfValue.NumberValue:
                    return ValueConverter.ConvertNumberValueToLogicalValue(currentValue.AsNumberValue, _context).SystemValue;

                case KindOfValue.StrongIdentifierValue:
                    return ValueConverter.ConvertStrongIdentifierValueToLogicalValue(currentValue.AsStrongIdentifierValue, _context).SystemValue;

                case KindOfValue.RuleInstanceValue:
                    {
                        var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                        var searchOptions = new LogicalSearchOptions();
                        searchOptions.QueryExpression = ruleInstance;
                        searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

                        var searchResult = _logicalSearchResolver.Run(searchOptions);

#if DEBUG
                        //Log($"searchResult = {searchResult}");
                        //Log($"result = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
#endif

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
#if DEBUG
                //Log("_currentSEHGroup != null");
#endif

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

#if DEBUG
            //Log($"errorValue = {errorValue}");
#endif

            _currentError = errorValue;

            _globalLogicalStorage.Append(ruleInstance);
        }

        private bool CheckSEH()
        {
            var ruleInstance = _currentError.RuleInstance;

#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
#endif

            var searchOptions = new LogicalSearchOptions();
            searchOptions.TargetStorage = ruleInstance;
            searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

            foreach (var sehItem in _currentCodeFrame.CurrentSEHGroup.Items)
            {
#if DEBUG
                //Log($"sehItem = {sehItem}");
#endif

                if (sehItem.Condition != null)
                {
                    searchOptions.QueryExpression = sehItem.Condition;

                    if (!_logicalSearchResolver.IsTruth(searchOptions))
                    {
                        continue;
                    }
                }

#if DEBUG
                //Log("NEXT");
#endif

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
#if DEBUG
            //Log($"_currentError = {_currentError}");
#endif

            if (_currentCodeFrame.CurrentSEHGroup == null)
            {
                return false;
            }

#if DEBUG
            //Log("_currentSEHGroup != null");
#endif

            return CheckSEH();
        }

        private void SetUpCurrentCodeFrame()
        {
            _executionCoordinator = _currentCodeFrame.ExecutionCoordinator;
            _currentInstance = _currentCodeFrame.Instance;
            _currentVarStorage = _currentCodeFrame.LocalContext.Storage.VarStorage;
        }

        private void GoBackToPrevCodeFrame()
        {
            GoBackToPrevCodeFrame(ActionExecutionStatus.None);
        }

        private void GoBackToPrevCodeFrame(ActionExecutionStatus targetActionExecutionStatus)
        {
            if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                var specialMark = _currentCodeFrame.SpecialMark;

#if DEBUG
                //Log($"targetActionExecutionStatus = {targetActionExecutionStatus}");
                //Log($"_currentCodeFrame.ProcessInfo.Status = {_currentCodeFrame.ProcessInfo.Status}");
                //Log($"specialMark = {specialMark}");
#endif

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

            if (currentProcessInfo.Status == ProcessStatus.Running)
            {
#if DEBUG
                //Log("currentProcessInfo.Status == ProcessStatus.Running");
#endif

                switch (targetActionExecutionStatus)
                {
                    case ActionExecutionStatus.Complete:
                        currentProcessInfo.Status = ProcessStatus.Completed;
                        break;

                    case ActionExecutionStatus.Broken:
                    case ActionExecutionStatus.Faulted:
                        currentProcessInfo.Status = ProcessStatus.Faulted;
                        break;

                    case ActionExecutionStatus.Canceled:
                    default:
                        currentProcessInfo.Status = ProcessStatus.Canceled;
                        break;
                }
            }

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

                SetUpCurrentCodeFrame();
            }
        }

        private List<Value> TakePositionedParameters(int count, bool resolveValueFromVar = false)
        {
#if DEBUG
            //Log($"count = {count}");
            //Log($"resolveValueFromVar = {resolveValueFromVar}");
#endif

            var result = new List<Value>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                if (resolveValueFromVar)
                {
                    result.Add(TryResolveFromVar(valueStack.Pop()));
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
                    value = TryResolveFromVar(value);
                }

                result[name] = value;
            }

            return result;
        }

        private enum SyncOption
        {
            Sync,
            IndependentAsync,
            ChildAsync
        }

        private void CallFunction(KindOfFunctionParameters kindOfParameters, int parametersCount, SyncOption syncOption)
        {
#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"parametersCount = {parametersCount}");
            //Log($"syncOption = {syncOption}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

#if DEBUG
            //Log($"annotation = {annotation}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var caller = valueStack.Pop();

#if DEBUG
            //Log($"caller = {caller}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

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

#if DEBUG
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
            //Log($"namedParameters = {namedParameters?.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"caller.IsPointRefValue = {caller.IsPointRefValue}");
            //Log($"caller.IsStrongIdentifierValue = {caller.IsStrongIdentifierValue}");
#endif

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

            throw new NotImplementedException();
        }

        private void CallPointRefValue(PointRefValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption)
        {
            var callerLeftOperand = caller.LeftOperand;
            var callerRightOperand = caller.RightOperand;

#if DEBUG
            //Log($"callerLeftOperand = {callerLeftOperand}");
#endif

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

#if DEBUG
            //Log($"methodName = {methodName}");
#endif

            var method = callerLeftOperand.GetMethod(methodName, kindOfParameters, namedParameters, positionedParameters);

#if DEBUG
            //Log($"method = {method}");
#endif

            CallExecutable(method, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
        }

        private void CallHost(StrongIdentifierValue methodName, 
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption)
        {
#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"annotation = {annotation}");
            //Log($"syncOption = {syncOption}");
#endif

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

#if DEBUG
            //Log($"command = {command}");
#endif

            //var packedSynonymsResolver = new PackedSynonymsResolver(_context.DataResolversFactory.GetSynonymsResolver(), _currentCodeFrame.LocalContext);

            var processCreatingResult = _hostListener.CreateProcess(command, _context, _currentCodeFrame.LocalContext);

#if DEBUG
            //Log($"processCreatingResult = {processCreatingResult}");
#endif

            if(processCreatingResult.IsSuccessful)
            {
                var processInfo = processCreatingResult.Process;                

                _instancesStorage.AppendAndTryStartProcessInfo(processInfo);

                var timeout = GetTimeoutFromAnnotation(annotation);

#if DEBUG
                //Log($"timeout = {timeout}");
#endif

                if (syncOption == SyncOption.Sync)
                {
                    processInfo.ParentProcessInfo = _currentCodeFrame.ProcessInfo;

                    List<IExecutionCoordinator> executionCoordinators = null;

                    if (_executionCoordinator != null)
                    {
                        executionCoordinators = new List<IExecutionCoordinator>() { _executionCoordinator };
                    }

                    ProcessInfoHelper.Wait(executionCoordinators, timeout, _dateTimeProvider, processInfo);

                    //Log($"localExecutionCoordinator = {localExecutionCoordinator}");

                    if(_executionCoordinator != null && _executionCoordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                    {
                        ProcessError(_executionCoordinator.RuleInstance);

                        return;
                    }

                    var status = processInfo.Status;

#if DEBUG
                    //Log($"status = {status}");
#endif

                    switch (status)
                    {
                        case ProcessStatus.Canceled:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Canceled;
                            _isCanceled = true;

                            GoBackToPrevCodeFrame(ActionExecutionStatus.Canceled);
                            return;

                        case ProcessStatus.Faulted:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

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
                }

                _currentCodeFrame.CurrentPosition++;

                return;
            }

#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"isSync = {isSync}");
            //Log($"command = {command}");
            //Log($"processCreatingResult = {processCreatingResult}");
#endif

            throw new NotImplementedException();
        }

        private void CallStrongIdentifierValue(StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            Value annotation, SyncOption syncOption, bool mayCallHost)
        {
#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"annotation = {annotation}");
            //Log($"syncOption = {syncOption}");
            //Log($"mayCallHost = {mayCallHost}");
#endif

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

#if DEBUG
            //Log($"method = {method}");
#endif

            if(method == null)
            {
                if(mayCallHost)
                {
                    CallHost(methodName, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
                    return;
                }

                throw new Exception($"Method '{methodName.NameValue}' is not found.");
            }

            CallExecutable(method, kindOfParameters, namedParameters, positionedParameters, annotation, syncOption);
        }

        private void ExecRuleInstanceValue(RuleInstanceValue ruleInstanceValue)
        {
#if DEBUG
            //Log($"ruleInstanceValue = {ruleInstanceValue.ToHumanizedString()}");
#endif

            var compiledCode = _converterFactToImperativeCode.Convert(ruleInstanceValue.RuleInstance, _currentCodeFrame.LocalContext);

#if DEBUG
            //Log($"compiledCode = {compiledCode.ToDbgString()}");
#endif

            var codeFrame = CodeFrameHelper.ConvertCompiledFunctionBodyToCodeFrame(compiledCode, _currentCodeFrame.LocalContext, _context);

            ExecuteCodeFrame(codeFrame, null, SyncOption.Sync);
        }

        private StrongIdentifierValue _timeoutName = NameHelper.CreateName("timeout");
        private StrongIdentifierValue _priorityName = NameHelper.CreateName("priority");

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
            CallExecutable(executable, KindOfFunctionParameters.PositionedParameters, null, positionedParameters, null, SyncOption.Sync);
        }

        private void CallExecutable(IExecutable executable, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, Value annotation, SyncOption syncOption)
        {
            if(executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var coordinator = executable.TryActivate(_context);

            var timeout = GetTimeoutFromAnnotation(annotation);
            var priority = GetPriorityFromAnnotation(annotation);
#if DEBUG
            //Log($"executable.IsSystemDefined = {executable.IsSystemDefined}");
            //Log($"coordinator != null = {coordinator != null}");
            //Log($"annotation = {annotation}");
            //Log($"timeout = {timeout}");
            //Log($"priority = {priority}");
            //Log($"syncOption = {syncOption}");
#endif

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch(kindOfParameters)
                {
                    case KindOfFunctionParameters.NoParameters:
                        result = executable.SystemHandler.Call(new List<Value>(), _currentCodeFrame.LocalContext);
                        break;

                    case KindOfFunctionParameters.PositionedParameters:
                        result = executable.SystemHandler.Call(positionedParameters, _currentCodeFrame.LocalContext);
                        break;

                    case KindOfFunctionParameters.NamedParameters:
                        result = executable.SystemHandler.Call(namedParameters.ToDictionary(p => p.Key.NameValue, p => p.Value), annotation, _currentCodeFrame.LocalContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }
               
#if DEBUG
                //Log($"result = {result}");
#endif

                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                //Log($"_currentCodeFrame (co) = {_currentCodeFrame.ToDbgString()}");
#endif

                _currentCodeFrame.CurrentPosition++;

                return;
            }
            else
            {
                ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null;

                if (timeout.HasValue || priority.HasValue)
                {
                    additionalSettings = new ConversionExecutableToCodeFrameAdditionalSettings()
                    {
                        Timeout = timeout,
                        Priority = priority
                    };
                }

                var newCodeFrame = CodeFrameHelper.ConvertExecutableToCodeFrame(executable, kindOfParameters, namedParameters, positionedParameters, _currentCodeFrame.LocalContext, _context, additionalSettings);

#if DEBUG
                //Log($"newCodeFrame = {newCodeFrame}");
#endif

                ExecuteCodeFrame(newCodeFrame, coordinator, syncOption);
            }
        }

        private void ExecuteCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator, SyncOption syncOption)
        {
#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            _context.InstancesStorage.AppendProcessInfo(codeFrame.ProcessInfo);

#if DEBUG
            //Log($"syncOption = {syncOption}");
#endif

            if (syncOption == SyncOption.Sync)
            {
                if (coordinator != null)
                {
                    var instance = coordinator.Instance;

#if DEBUG
                    //Log($"instance?.Name = {instance?.Name}");                        
                    //Log($"_currentInstance?.Name = {_currentInstance?.Name}");
                    //Log($"coordinator.KindOfInstance = {coordinator.KindOfInstance}");
#endif

                    if (_currentInstance != null && instance != null)
                    {
                        SetSpecialMarkOfCodeFrame(codeFrame, coordinator);

                        _currentInstance.AddChildInstance(instance);
                    }
                }

                _currentCodeFrame.CurrentPosition++;

                codeFrame.ExecutionCoordinator = coordinator;

                SetCodeFrame(codeFrame);
            }
            else
            {
                _currentCodeFrame.CurrentPosition++;

                if (syncOption == SyncOption.ChildAsync)
                {
#if DEBUG
                    //Log($"codeFrame = {codeFrame}");
                    //Log($"codeFrame.ProcessInfo.Id = {codeFrame.ProcessInfo.Id}");
                    //Log($"_currentCodeFrame = {_currentCodeFrame}");
#endif

                    _currentCodeFrame.ProcessInfo.AddChild(codeFrame.ProcessInfo);
                }

                var threadExecutor = new AsyncThreadExecutor(_context);
                threadExecutor.SetCodeFrame(codeFrame);

                var task = threadExecutor.Start();

                _currentCodeFrame.ValuesStack.Push(task);
            }
        }

        private void SetSpecialMarkOfCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator)
        {
            if(coordinator == null)
            {
                return;
            }

            var kindOfInstance = coordinator.KindOfInstance;

#if DEBUG
            //Log($"kindOfInstance = {kindOfInstance}");
#endif

            switch (kindOfInstance)
            {
                case KindOfInstance.ActionInstance:
                    codeFrame.SpecialMark = SpecialMarkOfCodeFrame.MainFrameOfActionInstance;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfInstance), kindOfInstance, null);
            }
        }

        private void ProcessSetInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = _strongIdentifierLinearResolver.Resolve(paramsList[0], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(paramsList[1], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = paramsList[2];//_logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true);

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetNotInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = _logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true).Inverse();

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }
    }
}
