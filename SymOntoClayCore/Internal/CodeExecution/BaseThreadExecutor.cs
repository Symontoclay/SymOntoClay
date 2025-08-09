/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using NLog;
using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Instances.TaskInstances;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor : BaseLoggedComponent, IThreadExecutor,
        IOnCompletedActiveObjectHandler
    {
        protected static (IMonitorLogger Logger, string ThreadId) CreateInitParams(IEngineContext context)
        {
            var monitorNode = context.MonitorNode;
            var threadId = monitorNode.CreateThreadId();
            return (monitorNode.CreateThreadLogger("71899838-D655-4B97-890D-0017F326F002", threadId), threadId);
        }

        protected static (IMonitorLogger Logger, string ThreadId) CreateInitParams(IEngineContext context, string parentThreadId)
        {
            var monitorNode = context.MonitorNode;
            var threadId = monitorNode.CreateThreadId();
            return (monitorNode.CreateThreadLogger(messagePointId: "BB6BCC54-A80C-4AE6-A2D5-338A35EF1AB3", threadId: threadId, parentThreadId: parentThreadId), threadId);
        }

        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject, (IMonitorLogger Logger, string ThreadId) initData)
            : this(context, activeObject, initData.Logger, initData.ThreadId)
        {
        }

        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject, IMonitorLogger logger, string threadId)
            : base(logger)
        {
            _threadId = threadId;

            _context = context;
            _codeFrameService = context.ServicesFactory.GetCodeFrameService();
            _codeFrameAsyncExecutor = new CodeFrameAsyncExecutor(context);

            _projectLoader = new ProjectLoader(context);

            _globalStorage = context.Storage.GlobalStorage;
            _globalLogicalStorage = _globalStorage.LogicalStorage;
            _hostListener = context.HostListener;

            _instancesStorage = _context.InstancesStorage;

            _activeObject = activeObject;
            activeObject.AddOnCompletedHandler(this);
            activeObject.PeriodicMethod = CommandLoop;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();
            _propertiesResolver = dataResolversFactory.GetPropertiesResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _methodsResolver = dataResolversFactory.GetMethodsResolver();
            _constructorsResolver = dataResolversFactory.GetConstructorsResolver();
            _logicalSearchResolver = dataResolversFactory.GetLogicalSearchResolver();
            _statesResolver = dataResolversFactory.GetStatesResolver();
            _annotationsResolver = dataResolversFactory.GetAnnotationsResolver();
            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _dateTimeResolver = dataResolversFactory.GetDateTimeResolver();
            _strongIdentifierExprValueResolver = dataResolversFactory.GetStrongIdentifierExprValueResolver();

            _valueResolvingHelper = dataResolversFactory.GetValueResolvingHelper();

            _converterFactToImperativeCode = context.ConvertersFactory.GetConverterFactToImperativeCode();
            _dateTimeProvider = context.DateTimeProvider;

            _typeConverter = context.TypeConverter;

            var commonNamesStorage = _context.CommonNamesStorage;

            _defaultCtorName = commonNamesStorage.DefaultCtorName;
            _timeoutName = commonNamesStorage.TimeoutAttributeName;
            _priorityName = commonNamesStorage.PriorityAttributeName;
        }

        private readonly IEngineContext _context;
        private readonly ICodeFrameService _codeFrameService;
        private readonly CodeFrameAsyncExecutor _codeFrameAsyncExecutor;

        private readonly string _threadId;

        public string ThreadId => _threadId;

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
        private readonly PropertiesResolver _propertiesResolver;
        private readonly MethodsResolver _methodsResolver;
        private readonly ConstructorsResolver _constructorsResolver;
        private readonly LogicalSearchResolver _logicalSearchResolver;
        private readonly StatesResolver _statesResolver;
        private readonly AnnotationsResolver _annotationsResolver;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly DateTimeResolver _dateTimeResolver;
        private readonly StrongIdentifierExprValueResolver _strongIdentifierExprValueResolver;

        private readonly ValueResolvingHelper _valueResolvingHelper;

        private readonly ITypeConverter _typeConverter;

        private readonly ConverterFactToImperativeCode _converterFactToImperativeCode;
        private readonly IDateTimeProvider _dateTimeProvider;

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        private IExecutionCoordinator _executionCoordinator;
        private IInstance _currentInstance;
        private IVarStorage _currentVarStorage;
        private IPropertyStorage _currentPropertyStorage;

        private ErrorValue _currentError;
        private bool _isCanceled;

        private ulong? _endOfTargetDuration;
        private List<IThreadExecutor> _waitedThreadExecutorsList;
        private List<IProcessInfo> _waitedProcessInfoList;

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
                var currentTick = _dateTimeProvider.CurrentTicks;

                codeFrame.EndOfTargetDuration = currentTick + timeout.Value;
            }

            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;

            SetUpCurrentCodeFrame();

            if (setAsRunning)
            {
                _currentCodeFrame.ProcessInfo.SetStatus(Logger, "FBF59E50-D35C-487E-9B49-8C7D5DBFAB7D", ProcessStatus.Running);
            }
        }

        public void SetCodeFrames(List<CodeFrame> codeFramesList)
        {
            var reverseCodeFramesList = codeFramesList.ToList();
            reverseCodeFramesList.Reverse();

            foreach (var codeFrame in reverseCodeFramesList)
            {
                codeFrame.ProcessInfo.SetStatus(Logger, "20582FBC-C298-4BAF-B969-B7B63A3B5754", ProcessStatus.WaitingToRun);

                SetCodeFrame(codeFrame, false);
            }

            _currentCodeFrame.ProcessInfo.SetStatus(Logger, "756FCA0C-6645-4CCA-B865-6A7220476D27", ProcessStatus.Running);
        }

        /// <inheritdoc/>
        public IThreadTask Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public ThreadTaskStatus RunningStatus => _activeObject.TaskValue?.Status ?? ThreadTaskStatus.Created;

        /// <inheritdoc/>
        public void Cancel()
        {
            _activeObject.TaskValue?.Cancel();
        }

        /// <inheritdoc/>
        public void Wait()
        {
            _activeObject.TaskValue?.Wait();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _activeObject.Dispose();

            _onCompletedHandlers.Clear();
        }

        /// <inheritdoc/>
        public void AddOnCompletedHandler(IOnCompletedThreadExecutorHandler handler)
        {
            lock (_onCompletedHandlersLockObj)
            {
                if (_onCompletedHandlers.Contains(handler))
                {
                    return;
                }

                _onCompletedHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnCompletedHandler(IOnCompletedThreadExecutorHandler handler)
        {
            lock (_onCompletedHandlersLockObj)
            {
                if (_onCompletedHandlers.Contains(handler))
                {
                    _onCompletedHandlers.Remove(handler);
                }
            }
        }

        void IOnCompletedActiveObjectHandler.Invoke()
        {
            EmitOnCompletedHandlers();
            _activeObject.RemoveOnCompletedHandler(this);
        }

        private void EmitOnCompletedHandlers()
        {
            lock (_onCompletedHandlersLockObj)
            {
                foreach (var handler in _onCompletedHandlers.ToArray())
                {
                    handler.Invoke();
                }
            }
        }

        private object _onCompletedHandlersLockObj = new object();
        private List<IOnCompletedThreadExecutorHandler> _onCompletedHandlers = new List<IOnCompletedThreadExecutorHandler>();

        private bool CommandLoop(CancellationToken cancellationToken)
        {
            try
            {
                var currentCodeFrame = _currentCodeFrame;

                if (currentCodeFrame == null)
                {
                    Logger.LeaveThreadExecutor("6173F3EA-CFD0-4603-9A6E-FF06C6866DAD");

                    return false;
                }

                if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing)
                {
                    GoBackToPrevCodeFrame("E6E7605D-E9C3-4FF6-A70B-4C28F77F9A64", _executionCoordinator.ExecutionStatus);
                    return true;
                }

                if (currentCodeFrame.ProcessInfo.Status == ProcessStatus.Canceled)
                {
                    GoBackToPrevCodeFrame("B691C44F-4168-4EDB-B113-D59E8E6A2A45", ActionExecutionStatus.Canceled);
                    return true;
                }

                if (currentCodeFrame.ProcessInfo.Status == ProcessStatus.WeakCanceled)
                {
                    GoBackToPrevCodeFrame("8C345CC8-A8EA-4241-8C93-17E6ADA6D33A", ActionExecutionStatus.WeakCanceled);
                    return true;
                }

                var endOfTargetDuration = currentCodeFrame.EndOfTargetDuration;

                if (endOfTargetDuration.HasValue)
                {
                    var currentTick = _dateTimeProvider.CurrentTicks;

                    if (currentTick >= endOfTargetDuration.Value)
                    {
                        var timeoutCancellationMode = currentCodeFrame.TimeoutCancellationMode;

                        switch(timeoutCancellationMode)
                        {
                            case TimeoutCancellationMode.WeakCancel:
                                GoBackToPrevCodeFrame("AD20A217-8A76-43C8-ADDC-44DE270229B1", ActionExecutionStatus.WeakCanceled);
                                break;

                            case TimeoutCancellationMode.Cancel:
                                GoBackToPrevCodeFrame("0D771BAF-D08B-4869-9A8E-CCE324382C83", ActionExecutionStatus.Canceled);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(timeoutCancellationMode), timeoutCancellationMode, null);
                        }

                        return true;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    GoBackToPrevCodeFrame("1F90CC1F-99B5-49FA-A661-754A81E0BC7B", ActionExecutionStatus.Canceled);
                    return true;
                }

                Logger.CodeFrame("C5B6E668-F7A6-4F76-915D-5472418CF697", currentCodeFrame.ToDbgString());

#if DEBUG
                Info("5D03A3F6-AF43-4D3A-8DED-A976A66603F2", $"currentCodeFrame = {currentCodeFrame.ToDbgString()}");
#endif

                var currentPosition = currentCodeFrame.CurrentPosition;

                var compiledFunctionBodyCommands = currentCodeFrame.CompiledFunctionBody.Commands;

                if (currentPosition >= compiledFunctionBodyCommands.Count)
                {
                    GoBackToPrevCodeFrame("9B3F7A86-FA93-4F76-A4C6-B0C0D54D2D0D", ActionExecutionStatus.Complete);
                    return true;
                }

                if (!CheckReturnedInfo())
                {
                    Logger.LeaveThreadExecutor("D2D3002E-9A1B-4847-9394-B845AA13559F");
                    return false;
                }

                var currentCommand = compiledFunctionBodyCommands[currentPosition];

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

                    case OperationCode.PropDecl:
                        ProcessPropDecl(currentCommand);
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
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.Sync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.Call_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.Sync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.Call_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.Sync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.IndependentAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.IndependentAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.IndependentAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncChildCall:
                        CallFunction(KindOfFunctionParameters.NoParameters, 0, SyncOption.ChildAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncChildCall_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, SyncOption.ChildAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.AsyncChildCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, SyncOption.ChildAsync, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.CallCtor:
                        CallConstructor(KindOfFunctionParameters.NoParameters, 0, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.CallCtor_N:
                        CallConstructor(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, currentCommand.AnnotatedItem);
                        break;

                    case OperationCode.CallCtor_P:
                        CallConstructor(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, currentCommand.AnnotatedItem);
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

                    case OperationCode.CancelAction:
                        ProcessCancelAction();
                        break;

                    case OperationCode.WeakCancelAction:
                        ProcessWeakCancelAction();
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

                    case OperationCode.AddLifeCycleEvent:
                        ProcessAddLifeCycleEvent();
                        break;

                    case OperationCode.BeginCompoundHtnTask:
                        ProcessBeginCompoundTask(currentCommand);
                        break;

                    case OperationCode.EndCompoundHtnTask:
                        ProcessEndCompoundTask(currentCommand);
                        break;

                    case OperationCode.BeginPrimitiveHtnTask:
                        ProcessBeginPrimitiveTask(currentCommand);
                        break;

                    case OperationCode.EndPrimitiveHtnTask:
                        ProcessEndPrimitiveTask(currentCommand);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentCommand.OperationCode), currentCommand.OperationCode, null);
                }

                return true;
            }
            catch (Exception e)
            {
                Error("55249ED6-2612-4CAE-8073-0164016F1C03", e);

#if DEBUG
                Info("AAAF2B02-51E1-469D-89DB-F9DA59C98984", $"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                throw;
            }
        }

        private IMonitorLogger GetTargetLogger(SyncOption syncOption)
        {
#if DEBUG
            //Info("E95DAA22-EB38-4D72-8C0B-D8A74DB6076D", $"syncOptions = {syncOption}");
#endif

            switch(syncOption)
            {
                case SyncOption.Sync:
                case SyncOption.Ctor:
                case SyncOption.PseudoSync:
                    return Logger;

                case SyncOption.IndependentAsync:
                case SyncOption.ChildAsync:
                    {
#if DEBUG
                        //Info("2F56F914-3967-4260-86A3-083281AD50AA", $"Logger.Id = {Logger.Id}");
#endif
                        var monitorNode = _context.MonitorNode;
                        return monitorNode.CreateThreadLogger("D855D49C-25C6-4A2B-8951-DEB4A2139E74", threadId: monitorNode.CreateThreadId(), parentThreadId: Logger.Id);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(syncOption), syncOption, null);
            }
        }

        private void ProcessBeginCompoundTask(ScriptCommand currentCommand)
        {
#if DEBUG
            //Info("D2F278BA-4BA0-4C8D-A424-F929BB816100", $"currentCommand = {currentCommand.ToDbgString()}");
#endif

            var currentCodeFrame = _currentCodeFrame;

            var compoundTask = currentCommand.CompoundTask;

            var baseCompoundTaskInstance = BaseCompoundTaskInstanceFactory(compoundTask);
            baseCompoundTaskInstance.Init(Logger);

            var codeFrameEvnPart = new CodeFrameEvnPart
            {
                LocalContext = currentCodeFrame.LocalContext,
                Metadata = currentCodeFrame.Metadata,
                Instance = currentCodeFrame.Instance,
                ExecutionCoordinator = currentCodeFrame.ExecutionCoordinator,
                CompoundTaskInstance = currentCodeFrame.CompoundTaskInstance
            };

#if DEBUG
            //Info("B5BDC6F0-3845-43AE-9820-405B99957D93", $"codeFrameEvnPart = {codeFrameEvnPart.ToDbgString()}");
#endif

            currentCodeFrame.CodeFrameEvnPartsStack.Push(codeFrameEvnPart);

            currentCodeFrame.LocalContext = baseCompoundTaskInstance.LocalCodeExecutionContext;
            currentCodeFrame.Metadata = compoundTask;
            currentCodeFrame.Instance = baseCompoundTaskInstance;
            currentCodeFrame.ExecutionCoordinator = baseCompoundTaskInstance.ExecutionCoordinator;
            currentCodeFrame.CompoundTaskInstance = baseCompoundTaskInstance;

#if DEBUG
            //Info("ADDBA7B6-2766-497C-9B4B-1E1A9220BAF9", $"currentCodeFrame = {currentCodeFrame.ToDbgString()}");
#endif

            _currentCodeFrame.CurrentPosition++;
        }

        private BaseCompoundTaskInstance BaseCompoundTaskInstanceFactory(BaseCompoundHtnTask compoundTask)
        {
            var currentCodeFrame = _currentCodeFrame;
            var localContext = currentCodeFrame.LocalContext;

            var kindOfTask = compoundTask.KindOfTask;

            switch(kindOfTask)
            {
                case KindOfTask.Root:
                    return new RootTaskInstance(compoundTask.AsRootHtnTask, _context, localContext.Storage, localContext, currentCodeFrame.ExecutionCoordinator);

                case KindOfTask.Strategic:
                    return new StrategicTaskInstance(compoundTask.AsStrategicHtnTask, _context, localContext.Storage, localContext, currentCodeFrame.ExecutionCoordinator);

                case KindOfTask.Tactical:
                    return new TacticalTaskInstance(compoundTask.AsTacticalHtnTask, _context, localContext.Storage, localContext, currentCodeFrame.ExecutionCoordinator);

                case KindOfTask.Compound:
                    return new CompoundTaskInstance(compoundTask.AsCompoundHtnTask, _context, localContext.Storage, localContext, currentCodeFrame.ExecutionCoordinator);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfTask), kindOfTask, null);
            }
        }

        private void ProcessEndCompoundTask(ScriptCommand currentCommand)
        {
            var currentCodeFrame = _currentCodeFrame;

            currentCodeFrame.CompoundTaskInstance.Dispose();

            var codeFrameEvnPart = currentCodeFrame.CodeFrameEvnPartsStack.Pop();

            currentCodeFrame.LocalContext = codeFrameEvnPart.LocalContext;
            currentCodeFrame.Metadata = codeFrameEvnPart.Metadata;
            currentCodeFrame.Instance = codeFrameEvnPart.Instance;
            currentCodeFrame.ExecutionCoordinator = codeFrameEvnPart.ExecutionCoordinator;
            currentCodeFrame.CompoundTaskInstance = codeFrameEvnPart.CompoundTaskInstance;

            //throw new NotImplementedException("41581C88-B848-442B-A307-EEDBACD88768");
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessBeginPrimitiveTask(ScriptCommand currentCommand)
        {
            //throw new NotImplementedException("4BD115D5-5D76-43F1-9FA6-717C3BFE4B0D");
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessEndPrimitiveTask(ScriptCommand currentCommand)
        {
            //throw new NotImplementedException("8C569C91-95D9-44FC-9AF3-CB11C9201884");
            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessAddLifeCycleEvent()
        {
            var valuesStack = _currentCodeFrame.ValuesStack;

            var handlerValue = valuesStack.Pop();

            var kindOfLifeCycleEventValue = valuesStack.Pop();

            var targetObjectValue = valuesStack.Pop();

            var kindOfLifeCycleEventName = kindOfLifeCycleEventValue.AsStrongIdentifierValue.NormalizedNameValue;

            var targetObject = targetObjectValue.AsProcessInfoValue;
            var handler = handlerValue.AsCodeItem.AsFunction;

            switch (kindOfLifeCycleEventName)
            {
                case "complete":
                case "completed":
                    targetObject.AddOnCompleteHandler(Logger, new ProcessInfoEventHandler(_context, Logger.Id, handler, _currentCodeFrame, true));
                    break;

                case "weak cancel":
                case "weak canceled":
                    targetObject.AddOnWeakCanceledHandler(Logger, new ProcessInfoEventHandler(_context, Logger.Id, handler, _currentCodeFrame, true));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLifeCycleEventName), kindOfLifeCycleEventName, null);
            }

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessInstantiate(KindOfFunctionParameters kindOfParameters, int parametersCount)
        {
            var valuesStack = _currentCodeFrame.ValuesStack;

            var prototypeValue = valuesStack.Pop();

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParametersOld(parametersCount);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParametersOld(parametersCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            var instanceValue = CreateInstance(prototypeValue);

            var newInstance = instanceValue.AsInstanceValue.InstanceInfo;

            var superClassesStoragesDict = _inheritanceResolver.GetSuperClassStoragesDict(Logger, newInstance.LocalCodeExecutionContext.Storage, newInstance);

            var executionsList = new List<(CodeFrame, IExecutionCoordinator)>();

            ConstructorResolvingResult constructorResolvingResult = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveWithSelfAndDirectInheritance(Logger, newInstance.Name, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveWithSelfAndDirectInheritance(Logger, newInstance.Name, namedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveWithSelfAndDirectInheritance(Logger, newInstance.Name, positionedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if (constructorResolvingResult != null)
            {
                if(constructorResolvingResult.NeedTypeConversion)
                {
                    throw new NotImplementedException("448090B6-3163-4BC0-8E33-B2284D7402EB");
                }

                IExecutable constructor = constructorResolvingResult.Constructor;

                var coordinator = ((IExecutable)constructor).GetCoordinator(Logger, _context, newInstance.LocalCodeExecutionContext);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(Logger, newInstance, constructor, kindOfParameters, namedParameters, positionedParameters, newInstance.LocalCodeExecutionContext, null);

                executionsList.Add((newCodeFrame, coordinator));
            }

            var preConstructors = _constructorsResolver.ResolvePreConstructors(Logger, newInstance.Name, newInstance.LocalCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (preConstructors.Any())
            {
                foreach (var preConstructor in preConstructors)
                {
                    var targetHolder = preConstructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(newInstance.LocalCodeExecutionContext);
                    localCodeExecutionContext.Storage = targetStorage;
                    localCodeExecutionContext.Holder = targetHolder;
                    localCodeExecutionContext.Instance = newInstance;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage = targetStorage;
                    localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.PreConstructor;

                    var coordinator = ((IExecutable)preConstructor).GetCoordinator(Logger, _context, newInstance.LocalCodeExecutionContext);

                    var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(Logger, newInstance, preConstructor, KindOfFunctionParameters.NoParameters, null, null, localCodeExecutionContext, null, true);

                    executionsList.Add((newCodeFrame, coordinator));
                }
            }

            if(executionsList.Any())
            {
                _currentCodeFrame.PutToValueStackAfterReturningBack = instanceValue;

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
            var kindOfValue = prototypeValue.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.CodeItem:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(Logger, prototypeValue.AsCodeItem, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                case KindOfValue.StrongIdentifierValue:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(Logger, prototypeValue.AsStrongIdentifierValue, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                case KindOfValue.InstanceValue:
                    {
                        var instanceValue = _context.InstancesStorage.CreateInstance(Logger, prototypeValue.AsInstanceValue, _currentCodeFrame.LocalContext);

                        return instanceValue;
                    }

                default:
                    throw new Exception($"The value {prototypeValue.ToHumanizedString()} can not be instantiated.");
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
            _executionCoordinator.SetExecutionStatus(Logger, "CF70D9D9-E756-48E8-A607-81908686F0FF", ActionExecutionStatus.Broken);

            _context.InstancesStorage.TryActivateDefaultState(Logger);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessBreakStateVal()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Peek();

            var ruleInstance = currentValue.AsRuleInstance;

            _executionCoordinator.SetExecutionStatus(Logger, "21485F40-AC82-40AE-AC79-505EDAFFB4CF", ActionExecutionStatus.Broken);

            _globalLogicalStorage.Append(Logger, ruleInstance);

            _context.InstancesStorage.TryActivateDefaultState(Logger);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCompleteState()
        {
            _executionCoordinator.SetExecutionStatus(Logger, "184BFA9F-BD34-4D55-BF25-B6AD83AA0722", ActionExecutionStatus.Complete);

            _context.InstancesStorage.TryActivateDefaultState(Logger);

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

            _globalStorage.StatesStorage.SetDefaultStateName(Logger, stateName);

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

            var state = _statesResolver.Resolve(Logger, stateName, _currentCodeFrame.LocalContext);

            _executionCoordinator.SetExecutionStatus(Logger, "59AD587D-5C99-4A38-817D-E826992E6D5C", ActionExecutionStatus.Complete);

            _context.InstancesStorage.ActivateState(Logger, state);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessReturnVal()
        {
            if (CodeFrameStateHelper.CanBeginCommandExecution(_currentCodeFrame.State))
            {
                _currentCodeFrame.State = CodeFrameState.BeginningCommandExecution;
            }

            if(CodeFrameStateHelper.ShouldCallTakeParameters(_currentCodeFrame.State))
            {
                var currentValueCallResult = TakeAndResolveCurrentValue();

#if DEBUG
                Info("E4C239A7-017E-4260-9326-AC0721172209", $"currentValueCallResult = {currentValueCallResult}");
#endif

                var currentValueCallResultKindOfResult = currentValueCallResult.KindOfResult;

#if DEBUG
                Info("E3F528B0-6F74-4A86-812E-241776E0CB6D", $"currentValueCallResultKindOfResult = {currentValueCallResultKindOfResult}");
#endif

                switch (currentValueCallResultKindOfResult)
                {
                    case KindOfCallResult.Value:
                        break;

                    case KindOfCallResult.ExecutingCodeInOtherCodeFrame:
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentValueCallResultKindOfResult), currentValueCallResultKindOfResult, null);
                }
            }

            if (_currentCodeFrame.State == CodeFrameState.ResolvedParameters)
            {
                _currentCodeFrame.State = CodeFrameState.CommandExecution;
            }

            if (_currentCodeFrame.State == CodeFrameState.CommandExecution)
            {
                var returnable = _currentCodeFrame.Metadata.AsIReturnable;

                if (returnable != null)
                {
#if DEBUG
                    Info("F91FFEA3-4B41-4B34-95C2-363FCC199ECC", $"returnable.TypesList? = {returnable.TypesList?.WriteListToString()}");
#endif

                    var conversionResult = _typeConverter.CheckAndTryConvert(Logger, _currentCodeFrame.ResolvedParameterValues.First(), returnable.TypesList, _currentCodeFrame.LocalContext);

#if DEBUG
                    Info("BB4EE87E-9C6D-4541-9836-F985B08E60D1", $"conversionResult = {conversionResult}");
#endif

                    var kindOfResult = conversionResult.KindOfResult;

                    switch (kindOfResult)
                    {
                        case KindOfCallResult.Value:
                            _currentCodeFrame.ResolvedParameterValues = new List<Value> { conversionResult.Value };
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
                    }
                }

                _currentCodeFrame.ProcessInfo.SetStatus(Logger, "17EFD6A4-C466-4A2E-AB3E-E7C90CC3547C", ProcessStatus.Completed);

                _currentCodeFrame.State = CodeFrameState.EndCommandExecution;
            }

            if(_currentCodeFrame.State == CodeFrameState.EndCommandExecution)
            {
                var currentValue = _currentCodeFrame.ResolvedParameterValues.First();

                var forParameterValueResolving = _currentCodeFrame.ForParameterValueResolving;

                GoBackToPrevCodeFrame("E2204170-6974-4F11-83F0-D078939C58C4", ActionExecutionStatus.Complete);

                if (_currentCodeFrame == null)
                {
                    ExternalReturn = currentValue;
                }
                else
                {
                    if (forParameterValueResolving)
                    {
                        _currentCodeFrame.CurrentResolvedParameterValue = currentValue;
                    }
                    else
                    {
                        _currentCodeFrame.ValuesStack.Push(currentValue);
                    }
                }
            }
        }

        private void ProcessReturn()
        {
            _currentCodeFrame.ProcessInfo.SetStatus(Logger, "F5B17665-B1A7-4B65-AF3A-F487CEC75F30", ProcessStatus.Completed);

            GoBackToPrevCodeFrame("7CA393A1-A3B8-48B5-BFF9-F7BA38048B13", ActionExecutionStatus.Complete);

            var currentValue = NullValue.Instance;

            if (_currentCodeFrame == null)
            {
                ExternalReturn = currentValue;
            }
            else
            {
                if (_currentCodeFrame.ForParameterValueResolving)
                {
                    _currentCodeFrame.CurrentResolvedParameterValue = currentValue;
                }
                else
                {
                    _currentCodeFrame.ValuesStack.Push(currentValue);
                }
            }
        }

        private void ProcessBreakActionVal()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Peek();

            var ruleInstance = currentValue.AsRuleInstance;

            _executionCoordinator.RuleInstance = ruleInstance;

            _executionCoordinator.SetExecutionStatus(Logger, "1A5CCCF7-2F52-45C7-B408-5E4B23662225", ActionExecutionStatus.Broken);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCompleteAction()
        {
#if DEBUG
            //Info("0296A5A0-3003-4813-A858-D0A55C68E98B", $"_currentCodeFrame.ExecutionCoordinator.Id = {_currentCodeFrame.ExecutionCoordinator.Id}");
            //Info("2586FC9C-3B5F-4653-B89C-005F58A26E5C", $"_currentCodeFrame.ProcessInfo.ToHumanizedLabel() = {_currentCodeFrame.ProcessInfo.ToHumanizedLabel()}");
#endif

            _currentCodeFrame.ExecutionCoordinator.SetExecutionStatus(Logger, "516B31E8-44B5-4B5A-8101-DDBF1D7DD8D0", ActionExecutionStatus.Complete);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessCancelAction()
        {
            _currentCodeFrame.ExecutionCoordinator.SetExecutionStatus(Logger, "74A41530-6EFB-42D4-A88B-52651BC84519", ActionExecutionStatus.Canceled);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessWeakCancelAction()
        {
            _currentCodeFrame.ExecutionCoordinator.SetExecutionStatus(Logger, "5F5779B1-4D20-455A-9756-4AF0009245DE", ActionExecutionStatus.WeakCanceled);

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
            var currentValue = _currentCodeFrame.ValuesStack.Pop();

            var kindOfCurrentValue = currentValue.KindOfValue;

            switch (kindOfCurrentValue)
            {
                case KindOfValue.CodeItem:
                    var codeItem = currentValue.AsCodeItem;
                    var codeItemKind = codeItem.Kind;
                    switch (codeItemKind)
                    {
                        case KindOfCodeEntity.RuleOrFact:
                            ExecRuleInstanceValue(currentValue.AsRuleInstance);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(codeItemKind), codeItemKind, null);
                    }
                    break;

                case KindOfValue.RuleInstance:
                    ExecRuleInstanceValue(currentValue.AsRuleInstance);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCurrentValue), kindOfCurrentValue, null);
            }
        }

        private void ProcessCallUnOp(ScriptCommand currentCommand)
        {
#if DEBUG
            //Info("21DF4338-0B0E-4956-81DB-3EA5DFA255B5", $"^^^^ _currentCodeFrame.State = {_currentCodeFrame.State}");
#endif

            if (CodeFrameStateHelper.CanBeginCommandExecution(_currentCodeFrame.State))
            {
                _currentCodeFrame.State = CodeFrameState.BeginningCommandExecution;
                _currentCodeFrame.CurrentKindOfOperator = currentCommand.KindOfOperator;
            }

            if (CodeFrameStateHelper.ShouldCallTakeParameters(_currentCodeFrame.State))
            {
#if DEBUG
                //Info("127B8EB8-251D-4CCC-9C79-F16ED03BDE71", $"currentCommand.KindOfOperator = {currentCommand.KindOfOperator}");
#endif

                var callOpTakeParametersSettings = GetCallOpTakeParametersSettings(currentCommand.KindOfOperator);

#if DEBUG
                //Info("7962C449-72E8-4F20-BC92-9D1549BC1534", $"callOpTakeParametersSettings.NeedRevers = {callOpTakeParametersSettings.NeedRevers}");
                //Info("0CBD60A5-1EAF-422B-BFBC-D0D41890D41A", $"callOpTakeParametersSettings.LoadingMatrix = {callOpTakeParametersSettings.LoadingMatrix.WritePODListToString()}");
#endif

                var paramsListCallResult = TakePositionedParameters(1, callOpTakeParametersSettings.NeedRevers, callOpTakeParametersSettings.LoadingMatrix);

#if DEBUG
                //Info("27766105-A33C-4FFD-9194-F15F6FC3442A", $"paramsListCallResult = {paramsListCallResult}");
#endif

                var paramsListCallResultKindOfResult = paramsListCallResult.KindOfResult;

#if DEBUG
                //Info("2E10CEDE-A0A2-4C6B-B9D0-744BA6EC0DA5", $"paramsListCallResultKindOfResult = {paramsListCallResultKindOfResult}");
#endif

                switch (paramsListCallResultKindOfResult)
                {
                    case KindOfCallResult.Value:
                        break;

                    case KindOfCallResult.ExecutingCodeInOtherCodeFrame:
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(paramsListCallResultKindOfResult), paramsListCallResultKindOfResult, null);
                }

#if DEBUG
                //Info("2B32E913-7D27-4C7E-943A-EBF343CFA996", $"_currentCodeFrame.ResolvedParameterValues = {_currentCodeFrame.ResolvedParameterValues.WriteListToString()}");
#endif
            }

            if (_currentCodeFrame.State == CodeFrameState.ResolvedParameters)
            {
                _currentCodeFrame.State = CodeFrameState.CommandExecution;
            }

            if (_currentCodeFrame.State == CodeFrameState.CommandExecution)
            {
                var operatorInfo = _operatorsResolver.GetOperator(Logger, _currentCodeFrame.CurrentKindOfOperator, _currentCodeFrame.LocalContext);

                CallOperator(operatorInfo, _currentCodeFrame.ResolvedParameterValues);

                _currentCodeFrame.State = CodeFrameState.EndCommandExecution;
            }
        }

        private void ProcessCallBinOp(ScriptCommand currentCommand)
        {
#if DEBUG
            //Info("693D9520-6EB3-40A5-9497-E7772582C3C6", $"!!!!!! _currentCodeFrame.State = {_currentCodeFrame.State}");
#endif

            if (CodeFrameStateHelper.CanBeginCommandExecution(_currentCodeFrame.State))
            {
                _currentCodeFrame.State = CodeFrameState.BeginningCommandExecution;
                _currentCodeFrame.CurrentKindOfOperator = currentCommand.KindOfOperator;
            }

            if (CodeFrameStateHelper.ShouldCallTakeParameters(_currentCodeFrame.State))
            {
#if DEBUG
                //Info("DECE2D4F-FEE3-4B36-ADA1-43DEB1BC0060", $"currentCommand.KindOfOperator = {currentCommand.KindOfOperator}");
#endif

                var callOpTakeParametersSettings = GetCallOpTakeParametersSettings(currentCommand.KindOfOperator);

#if DEBUG
                //Info("9AC1FC46-2CD3-4086-ADF3-C2B52E8E3F81", $"callOpTakeParametersSettings.NeedRevers = {callOpTakeParametersSettings.NeedRevers}");
                //Info("05E8E573-73FB-4145-AD0A-1112DE404B57", $"callOpTakeParametersSettings.LoadingMatrix = {callOpTakeParametersSettings.LoadingMatrix.WritePODListToString()}");
#endif

                var paramsListCallResult = TakePositionedParameters(2, callOpTakeParametersSettings.NeedRevers, callOpTakeParametersSettings.LoadingMatrix);

#if DEBUG
                //Info("57B18051-0C08-4AA8-AF84-8E84FFF78DE7", $"paramsListCallResult = {paramsListCallResult}");
#endif

                var paramsListCallResultKindOfResult = paramsListCallResult.KindOfResult;

#if DEBUG
                //Info("EE7E2DAA-E392-4B9F-B61C-395EADE19B11", $"paramsListCallResultKindOfResult = {paramsListCallResultKindOfResult}");
#endif

                switch (paramsListCallResultKindOfResult)
                {
                    case KindOfCallResult.Value:
                        break;

                    case KindOfCallResult.ExecutingCodeInOtherCodeFrame:
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(paramsListCallResultKindOfResult), paramsListCallResultKindOfResult, null);
                }

#if DEBUG
                //Info("A38F33A2-BF35-44E9-97EE-EC2A9AA9840B", $"_currentCodeFrame.ResolvedParameterValues = {_currentCodeFrame.ResolvedParameterValues.WriteListToString()}");
#endif

                if (_currentCodeFrame.CurrentKindOfOperator == KindOfOperator.IsNot)
                {
                    _currentCodeFrame.CurrentKindOfOperator = KindOfOperator.Is;
                }
            }

            if(_currentCodeFrame.State == CodeFrameState.ResolvedParameters)
            {
                _currentCodeFrame.State = CodeFrameState.CommandExecution;
            }

            if(_currentCodeFrame.State == CodeFrameState.CommandExecution)
            {
                var operatorInfo = _operatorsResolver.GetOperator(Logger, _currentCodeFrame.CurrentKindOfOperator, _currentCodeFrame.LocalContext);

                CallOperator(operatorInfo, _currentCodeFrame.ResolvedParameterValues);

                _currentCodeFrame.State = CodeFrameState.CommandPostExecution;
            }

            if(_currentCodeFrame.State == CodeFrameState.CommandPostExecution)
            {
                if (currentCommand.KindOfOperator == KindOfOperator.IsNot)
                {
                    var result = _currentCodeFrame.ValuesStack.Pop();

                    result = result.AsLogicalValue.Inverse();

                    _currentCodeFrame.ValuesStack.Push(result);
                }

                _currentCodeFrame.State = CodeFrameState.EndCommandExecution;
            }
        }

        private static bool[] _leftRightStreamBinOpTakeParametersSettings = [true, false];
        private static bool[] _pointBinOpTakeParametersSettings = [true, false];
        private static bool[] _assignBinOpTakeParametersSettings = [false, true];
        private static bool[] _usualBinOpTakeParametersSettings = [true, true];
        private static bool[] _usualUnOpTakeParametersSettings = [true];

        private (bool NeedRevers, bool[] LoadingMatrix) GetCallOpTakeParametersSettings(KindOfOperator kindOfOperator)
        {
#if DEBUG
            //Info("CA8916E7-8D93-4278-87ED-B86D34604206", $"kindOfOperator = {kindOfOperator}");
#endif

            switch(kindOfOperator)
            {
                case KindOfOperator.LeftRightStream:
                    return (true, _leftRightStreamBinOpTakeParametersSettings);

                case KindOfOperator.Point:
                    return (true, _pointBinOpTakeParametersSettings);

                case KindOfOperator.Assign:
                    return (false, _assignBinOpTakeParametersSettings);

                case KindOfOperator.Add:
                case KindOfOperator.Sub:
                case KindOfOperator.Mul:
                case KindOfOperator.Div:
                case KindOfOperator.More:
                case KindOfOperator.MoreOrEqual:
                case KindOfOperator.Less:
                case KindOfOperator.LessOrEqual:
                case KindOfOperator.Or:
                case KindOfOperator.And:
                case KindOfOperator.Is:
                case KindOfOperator.IsNot:
                    return (true, _usualBinOpTakeParametersSettings);

                case KindOfOperator.UnaryPlus:
                case KindOfOperator.UnaryMinus:
                case KindOfOperator.Not:
                    return (true, _usualUnOpTakeParametersSettings);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }
        }

        private void ProcessVarDecl(ScriptCommand currentCommand)
        {
            var valueStack = _currentCodeFrame.ValuesStack;

            var annotatedItem = currentCommand.AnnotatedItem;

#if DEBUG
            //Info("FAE862E7-9313-4F14-A950-D3F243046302", $"annotatedItem?.GetType().FullName = {annotatedItem?.GetType().FullName}");
#endif

            VarInstance varInstance = null;

            if(annotatedItem is Var)
            {
                varInstance = new VarInstance(annotatedItem as Var, _context);
            }
            else
            {
                if(annotatedItem is Field)
                {
                    varInstance = new VarInstance(annotatedItem as Field, _context);
                }
                else
                {
                    if(annotatedItem is VarDeclAstExpression)
                    {
                        var varDecl = annotatedItem as VarDeclAstExpression;

                        varInstance = new VarInstance(varDecl.Name, varDecl.TypesList, TypeOfAccess.Local, _context);
                    }
                    else
                    {
                        throw new NotSupportedException($"A95A0763-5F1D-4A72-BAD8-E11EE69506C1: Can not create {nameof(VarInstance)} by {annotatedItem?.GetType().FullName}.");
                    }                        
                }
            }

            _currentVarStorage.Append(Logger, varInstance);

            _currentCodeFrame.ValuesStack.Push(varInstance.Name);
            _currentCodeFrame.CurrentPosition++;

#if DEBUG
            //Info("2CA46DEF-5DAE-44AF-A4A8-2CC7C6F303B0", $"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
        }

        private void ProcessPropDecl(ScriptCommand currentCommand)
        {
#if DEBUG
            //Info("C3432985-5661-43EF-9126-8F2700484675", $"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var annotatedItem = currentCommand.AnnotatedItem;
            
#if DEBUG
            //Info("8A246ADA-0F77-48CA-AA29-EF5FC65A09B8", $"annotatedItem?.GetType().FullName = {annotatedItem?.GetType().FullName}");
#endif

            var property = annotatedItem as Property;

#if DEBUG
            //Info("84064116-5A17-4521-B574-E2EA5009907E", $"property = {property}");
            //Info("E1AB6C23-EB91-44B8-BC9C-BFF3EF8B45FF", $"_currentCodeFrame.Instance.Name = {_currentCodeFrame.Instance.Name}");
#endif

            var propertyInstance = new PropertyInstance(property, _currentCodeFrame.Instance, _context);

            _currentPropertyStorage.Append(Logger, propertyInstance);

            _currentCodeFrame.ValuesStack.Push(property.Name);
            _currentCodeFrame.CurrentPosition++;

            //throw new NotImplementedException("EEC6043C-D675-4E21-83A3-72AEB3C38F2F");
        }

        private void ProcessCodeItemDecl(ScriptCommand currentCommand)
        {
            var valuesStack = _currentCodeFrame.ValuesStack;

            var prototypeValue = valuesStack.Pop();

            var codeItem = prototypeValue.AsCodeItem;

            _projectLoader.LoadCodeItem(Logger, codeItem, _currentCodeFrame.LocalContext.Storage);

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

                        value = waypointSourceValue.ConvertToWaypointValue(Logger, _context, _currentCodeFrame.LocalContext);
                    }
                    break;

                case KindOfValue.ConditionalEntitySourceValue:
                    {
                        var conditionalEntitySourceValue = value.AsConditionalEntitySourceValue;

                        var conditionalEntityValue = conditionalEntitySourceValue.ConvertToConditionalEntityValue(Logger, _context, _currentCodeFrame.LocalContext);
                        conditionalEntityValue.Resolve(Logger);
                        value = conditionalEntityValue;
                    }
                    break;

                default:
                    break;
            }

            _currentCodeFrame.ValuesStack.Push(value);
            _currentCodeFrame.CurrentPosition++;

#if DEBUG
            //Info("10865BFD-1DD3-4FBE-AF2D-BB6E7C317921", $"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
        }

        [Obsolete]
        private void ProcessLoadFromVar()
        {
            /*var varName = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
            //Info("ABCC2FF2-9EB1-47A9-9B8D-14C98AC1C87F", $"currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
            //Info("F71F0BB0-E32C-455C-A996-86628BCBF465", $"varName = {varName}");
#endif

            var value = _varsResolver.GetVarValue(Logger, varName.AsStrongIdentifierValue, _currentCodeFrame.LocalContext);

#if DEBUG
            //Info("F6C7FB1C-BFA9-4871-9090-5EF56C8462AF", $"value = {value}");
#endif

            _currentCodeFrame.ValuesStack.Push(value);*/
            _currentCodeFrame.CurrentPosition++;
        }

        [Obsolete]
        private void ProcessTryLoadFromStrongIdentifier()
        {
            /*var conceptValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
            //Info("06DB4D55-8036-4404-AF04-8D4819E34024", $"currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
            //Info("70359123-3D0B-444B-B9E3-44F75BF3AB36", $"conceptValue = {conceptValue}");
#endif

            var propertyName = conceptValue.AsStrongIdentifierValue;

            //var oldCallResult = _propertiesResolver.GetPropertyValue(Logger, propertyName, _currentInstance, _currentCodeFrame.LocalContext);

#if DEBUG
            //Info("3DFD674F-EFA2-4607-B79E-7C5BE5179D43", $"oldCallResult = {oldCallResult}");
#endif

            var callResult = _strongIdentifierExprValueResolver.GetValue(Logger, propertyName, _currentInstance, _currentCodeFrame.LocalContext);

#if DEBUG
            //Info("F809CFFD-1E71-4CB0-A988-95A1D36FAD63", $"callResult = {callResult}");
#endif

            var kindOfResult = callResult.KindOfResult;

            switch(kindOfResult)
            {
                case KindOfCallResult.Value:
                    _currentCodeFrame.ValuesStack.Push(callResult.Value);
                    _currentCodeFrame.CurrentPosition++;
                    return;

                case KindOfCallResult.NeedExecuteGetProperty:
                case KindOfCallResult.NeedExecuteCode:
                    CallExecutable(callResult.Executable);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
            }*/
            _currentCodeFrame.CurrentPosition++;//tmp
            //throw new NotImplementedException("4EB61280-1522-422E-88F8-301A9ACA25AD");
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

        private void Wait(ScriptCommand currentCommand)
        {
            if (_endOfTargetDuration.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTicks;

                if (currentTick >= _endOfTargetDuration.Value)
                {
                    _endOfTargetDuration = null;
                    _currentCodeFrame.CurrentPosition++;
                    return;
                }

                return;
            }

            if (!_waitedThreadExecutorsList.IsNullOrEmpty() || !_waitedProcessInfoList.IsNullOrEmpty())
            {
                if ((_waitedThreadExecutorsList != null && _waitedThreadExecutorsList.Any(p => p.RunningStatus == ThreadTaskStatus.Running)) || 
                    (_waitedProcessInfoList != null && _waitedProcessInfoList.Any(p => p.Status == ProcessStatus.Running)))
                {
                    return;
                }

                _waitedThreadExecutorsList = null;
                _currentCodeFrame.CurrentPosition++;
                return;
            }

            var positionedParameters = TakePositionedParametersOld(currentCommand.CountParams);

            if (positionedParameters.Count == 1)
            {
                var firstParameter = positionedParameters[0];

                if (firstParameter.KindOfValue != KindOfValue.ThreadExecutorValue
                    && firstParameter.KindOfValue != KindOfValue.ProcessInfoValue)
                {
                    var timeoutSystemVal = _dateTimeResolver.ConvertTimeValueToTicks(Logger, firstParameter, DefaultTimeValues.TimeoutDefaultTimeValue, _currentCodeFrame.LocalContext);

                    var currentTick = _dateTimeProvider.CurrentTicks;

                    _endOfTargetDuration = currentTick + timeoutSystemVal;

                    return;
                }
            }

            if(positionedParameters.Any(p => p.KindOfValue == KindOfValue.ThreadExecutorValue) ||
                positionedParameters.Any(p => p.KindOfValue == KindOfValue.ProcessInfoValue))
            {
                if (positionedParameters.Any(p => p.KindOfValue == KindOfValue.ThreadExecutorValue))
                {
                    _waitedThreadExecutorsList = positionedParameters.Where(p => p.IsThreadExecutorValue).Select(p => p.AsThreadExecutorValue.ThreadExecutor).ToList();
                }

                if(positionedParameters.Any(p => p.KindOfValue == KindOfValue.ProcessInfoValue))
                {
                    _waitedProcessInfoList = positionedParameters.Where(p => p.IsProcessInfoValue).Select(p => p.AsProcessInfoValue.ProcessInfo).ToList();
                }

                return;
            }

            throw new NotImplementedException("E11ABE78-F83D-49A4-B968-BE9D19F3AA7B");
        }

        private void JumpToIf(float targetValue, int targetPosition)
        {
            _currentCodeFrame.State = CodeFrameState.BeginningCommandExecution;
            _currentCodeFrame.State = CodeFrameState.TakingParameters;

            var currLogicValue = GetLogicalValueFromCurrentStackValue();

            if (currLogicValue == targetValue)
            {
                _currentCodeFrame.CurrentPosition = targetPosition;
            }
            else
            {
                _currentCodeFrame.CurrentPosition++;
            }

            _currentCodeFrame.State = CodeFrameState.EndCommandExecution;
        }

        private float? GetLogicalValueFromCurrentStackValue()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
            //Info("2CC01662-FE1B-4963-9C26-271664A4B1FE", $"currentValue = {currentValue}");
#endif

            var conversionCallResult = TryResolveFromVarOrExpr(currentValue);

#if DEBUG
            //Info("094B79F8-D2B9-4104-8F67-B50E22D1B9DA", $"conversionCallResult = {conversionCallResult}");
#endif

            var conversionCallResultKindOfResult = conversionCallResult.KindOfResult;

#if DEBUG
            //Info("4AD601D7-4FD3-493C-9EBD-AE8DC7CF93E2", $"conversionCallResultKindOfResult = {conversionCallResultKindOfResult}");
#endif

            switch (conversionCallResultKindOfResult)
            {
                case KindOfCallResult.Value:
                    currentValue = conversionCallResult.Value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(conversionCallResultKindOfResult), conversionCallResultKindOfResult, null);
            }

            _currentCodeFrame.State = CodeFrameState.ResolvedParameters;

            var kindOfValue = currentValue.KindOfValue;

#if DEBUG
            //Info("6EEDFEAF-0B4A-4AF6-8725-CBB4C7BBE5AA", $"kindOfValue = {kindOfValue}");
#endif

            switch (kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return currentValue.AsLogicalValue.SystemValue;

                case KindOfValue.NumberValue:
                    return ValueConverter.ConvertNumberValueToLogicalValue(Logger, currentValue.AsNumberValue, _context).SystemValue;

                case KindOfValue.StrongIdentifierValue:
                    return ValueConverter.ConvertStrongIdentifierValueToLogicalValue(Logger, currentValue.AsStrongIdentifierValue, _context).SystemValue;

                case KindOfValue.RuleInstance:
                    {
                        var ruleInstance = currentValue.AsRuleInstance;

                        var searchOptions = new LogicalSearchOptions();
                        searchOptions.QueryExpression = ruleInstance;
                        searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;
                        searchOptions.ResolvingNotResultsStrategy = ResolvingNotResultsStrategy.Ignore;

                        var searchResult = _logicalSearchResolver.Run(Logger, searchOptions);

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
                _currentCodeFrame.ProcessInfo.SetStatus(Logger, "39056815-6732-4045-9CC1-A72D4B64DAE5", ProcessStatus.Faulted);

                GoBackToPrevCodeFrame("7521D54F-D408-4098-9B77-29BD014AF10C", ActionExecutionStatus.Faulted);
            }
            else
            {
                if (!CheckSEH())
                {
                    _currentCodeFrame.ProcessInfo.SetStatus(Logger, "2A75FFA8-314B-4408-A5AE-6EE13EFC6DFB", ProcessStatus.Faulted);

                    GoBackToPrevCodeFrame("1115613F-CEE0-4FA3-8E04-554B1C8B3E4E", ActionExecutionStatus.Faulted);
                }
            }
        }

        private void RegisterError(RuleInstance ruleInstance)
        {
            var errorValue = new ErrorValue(ruleInstance);
            errorValue.CheckDirty();

            _currentError = errorValue;

            _globalLogicalStorage.Append(Logger, ruleInstance);
        }

        private bool CheckSEH()
        {
            var ruleInstance = _currentError.RuleInstance;

            var localCodeExecutionContext = _currentCodeFrame.LocalContext;

            var searchOptions = new LogicalSearchOptions();
            searchOptions.TargetStorage = ruleInstance;
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

            foreach (var sehItem in _currentCodeFrame.CurrentSEHGroup.Items)
            {
                if (sehItem.Condition != null)
                {
                    searchOptions.QueryExpression = sehItem.Condition;

                    if (!_logicalSearchResolver.IsTruth(Logger, searchOptions))
                    {
                        continue;
                    }
                }

                if (sehItem.VariableName != null && !sehItem.VariableName.IsEmpty)
                {
                    _currentVarStorage.SetValue(Logger, sehItem.VariableName, _currentError, localCodeExecutionContext);
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
            var currentCodeFrame = _currentCodeFrame;
            var storage = currentCodeFrame.LocalContext.Storage;

            _executionCoordinator = currentCodeFrame.ExecutionCoordinator;
            _currentInstance = currentCodeFrame.Instance;
            _currentVarStorage = storage.VarStorage;
            _currentPropertyStorage = storage.PropertyStorage;

            if (currentCodeFrame.PutToValueStackAfterReturningBack != null)
            {
                currentCodeFrame.ValuesStack.Push(currentCodeFrame.PutToValueStackAfterReturningBack);
                currentCodeFrame.PutToValueStackAfterReturningBack = null;
            }

            if(currentCodeFrame.NeedsExecCallEvent)
            {
                currentCodeFrame.LastProcessStatus = lastProcessStatus;
            }
        }

        private void GoBackToPrevCodeFrame(string messagePointId, ActionExecutionStatus targetActionExecutionStatus)
        {
#if DEBUG
            //Info("A557578A-7C45-4D7E-B5F6-66CEE6E99D6F", $"messagePointId = {messagePointId}; targetActionExecutionStatus = {targetActionExecutionStatus}");
#endif

            Logger.GoBackToPrevCodeFrame(messagePointId, (int)targetActionExecutionStatus, targetActionExecutionStatus.ToString());

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
                        _executionCoordinator.SetExecutionStatus(Logger, "5289017C-43D5-4842-93FA-2369A1A039CA", targetActionExecutionStatus);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(specialMark), specialMark, null);
                }
            }

            var currentProcessInfo = _currentCodeFrame.ProcessInfo;

            var currentProcessInfoStatus = currentProcessInfo.Status;

#if DEBUG
            //Info("E03BAE1E-DD4F-4474-93B3-06CE775797FA", $"messagePointId = {messagePointId}; currentProcessInfoStatus = {currentProcessInfoStatus}");
#endif

            if (currentProcessInfoStatus == ProcessStatus.Running)
            {
                switch (targetActionExecutionStatus)
                {
                    case ActionExecutionStatus.Complete:
                        currentProcessInfo.SetStatus(Logger, "0215F946-3816-4B78-84EA-516296FE2002", ProcessStatus.Completed);
                        break;

                    case ActionExecutionStatus.Broken:
                    case ActionExecutionStatus.Faulted:
                        currentProcessInfo.SetStatus(Logger, "9CEF77A9-3A9A-4806-9898-C62353BC7AA9", ProcessStatus.Faulted);
                        break;

                    case ActionExecutionStatus.WeakCanceled:
                        currentProcessInfo.SetStatus(Logger, "BF6BB611-E243-4CB5-A71F-6211CC131B6F", ProcessStatus.WeakCanceled);
                        break;

                    case ActionExecutionStatus.Canceled:
                        currentProcessInfo.SetStatus(Logger, "14F8DA0A-7CE8-499F-B50A-9AEA95E9012E", ProcessStatus.Canceled);
                        _isCanceled = true;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetActionExecutionStatus), targetActionExecutionStatus, null);
                }
            }
            else
            {
                switch (targetActionExecutionStatus)
                {
                    case ActionExecutionStatus.WeakCanceled:
                        currentProcessInfo.SetStatus(Logger, "5C7F33A6-B618-4739-8027-0C24ED73A590", ProcessStatus.WeakCanceled);
                        break;

                    case ActionExecutionStatus.Canceled:
                        currentProcessInfo.SetStatus(Logger, "E7CE8CC3-2746-4B86-A5E5-538E46189311", ProcessStatus.Canceled);
                        _isCanceled = true;
                        break;
                }
            }

            var lastProcessStatus = currentProcessInfo.Status;

            _codeFrames.Pop();

            if (_codeFrames.Count == 0)
            {
                _currentCodeFrame = null;
            }
            else
            {
                _currentCodeFrame = _codeFrames.Peek();

                if (_isCanceled)
                {
                    _currentCodeFrame.ProcessInfo.SetStatus(Logger, "54F2D438-718E-43AB-BEDD-CC8C024FE4AD", ProcessStatus.Canceled);

                    GoBackToPrevCodeFrame("BCE9BB8B-31E5-4A1F-ADED-A9E6733A535C", ActionExecutionStatus.Canceled);
                    return;
                }

#if DEBUG
                //Info("454EF07F-92AC-4285-8438-17F27291BD65", $"messagePointId = {messagePointId}; lastProcessStatus = {lastProcessStatus}");
#endif

                SetUpCurrentCodeFrame(lastProcessStatus);
            }
        }

        private static bool[] _takeAndResolveCurrentValueLoadingMatrix = [true];

        private ValueListCallResult TakeAndResolveCurrentValue()
        {
            return TakePositionedParameters(1, false, _takeAndResolveCurrentValueLoadingMatrix);
        }

        /// <summary>
        /// Takes some amount of parameters from stack of current CodeFrame.
        /// Converts to values if needs.
        /// Ready made parameters returns as the result.
        /// Also It continues to be in the field ResolvedParameterValues of current CodeFrame.
        /// It needs for correct serializations.
        /// Please set the state CodeFrameState.BeginningCommandExecution to the current CodeFrame for starting working.
        /// Please expect the state CodeFrameState.ResolvedParameters in the current CodeFrame after finishing working.
        /// </summary>
        /// <param name="count">Target amount of parameters.</param>
        /// <param name="needRevers">Needs revers of not.</param>
        /// <param name="loadingMatrix">Settings which parameter needs conversion and which need not.</param>
        /// <returns>Returns ValueListCallResult, which describes which action should be done.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private ValueListCallResult TakePositionedParameters(int count, bool needRevers, bool[] loadingMatrix)
        {
#if DEBUG
            //Info("14B2B20F-631C-4713-AC8E-CD6B99C7A1AA", $"%%%% _currentCodeFrame.State = {_currentCodeFrame.State}");
#endif

            if(_currentCodeFrame.State == CodeFrameState.BeginningCommandExecution)
            {
                _currentCodeFrame.State = CodeFrameState.TakingParameters;
            }            

            if(_currentCodeFrame.State == CodeFrameState.TakingParameters)
            {
#if DEBUG
                //Info("16B68B6C-93AA-41F5-98FD-9B63853A776B", $"count = {count}");
                //Info("756E22C7-A85C-49A3-A26E-8C1E0A730AB7", $"needRevers = {needRevers}");
                //Info("FA9B4438-7B64-4411-A89B-00B7A5FDA691", $"loadingMatrix = {loadingMatrix?.WritePODListToString()}");
#endif

                BeginningClearResolvingParameter();

                var rawParamsList = NTakePositionedParameters(count, needRevers);

#if DEBUG
                //Info("6D0A80B5-D63B-4F20-B70E-069C53B1DE40", $"rawParamsList = {rawParamsList.WriteListToString()}");
#endif

                _currentCodeFrame.ResolvingParameterValues = rawParamsList;
                _currentCodeFrame.ResolvedParameterValues = new List<Value>();

                _currentCodeFrame.CurrentPositionOfResolvingParameter = -1;

                _currentCodeFrame.State = CodeFrameState.ResolvingParameters;
            }

            if(_currentCodeFrame.State == CodeFrameState.ResolvingParameters ||
                _currentCodeFrame.State == CodeFrameState.ResolvingParameterInCodeFrame)
            {
#if DEBUG
                //Info("C75CB66C-CF10-4032-8031-53D66898D3F7", $"##### _currentCodeFrame.CurrentPositionOfResolvingParameter = {_currentCodeFrame.CurrentPositionOfResolvingParameter}");
#endif

                var n = -1;

                foreach (var rawParam in _currentCodeFrame.ResolvingParameterValues)
                {
#if DEBUG
                    //Info("F603E818-0173-4497-AE25-07EFFCD458DA", $"n (before) = {n}");
                    //Info("994A65E5-F6F1-4166-B60E-CC74C858C14F", $"_currentCodeFrame.CurrentPositionOfResolvingParameter (before) = {_currentCodeFrame.CurrentPositionOfResolvingParameter}");
#endif

                    n++;

                    if(n > _currentCodeFrame.CurrentPositionOfResolvingParameter)
                    {
                        _currentCodeFrame.CurrentPositionOfResolvingParameter = n;
                    }
                    else
                    {
                        if(_currentCodeFrame.State == CodeFrameState.ResolvingParameterInCodeFrame &&
                            n == _currentCodeFrame.CurrentPositionOfResolvingParameter)
                        {
#if DEBUG
                            //Info("3DF206A5-EDC6-4112-B032-3434348D7BB8", $"_currentCodeFrame.CurrentResolvedParameterValue = {_currentCodeFrame.CurrentResolvedParameterValue}");
#endif

                            _currentCodeFrame.ResolvedParameterValues.Add(_currentCodeFrame.CurrentResolvedParameterValue);
                            _currentCodeFrame.State = CodeFrameState.ResolvingParameters;
                        }

                        continue;
                    }

#if DEBUG
                    //Info("A809E9E8-1910-4790-9BE4-DA01B5254129", $"n (after) = {n}");
                    //Info("E73B9D5B-1C10-4FF6-BADD-9BF0DE46D8BD", $"_currentCodeFrame.CurrentPositionOfResolvingParameter (after) = {_currentCodeFrame.CurrentPositionOfResolvingParameter}");
#endif

#if DEBUG
                    //Info("7D4F0934-7EAA-45CC-AE08-1194A58497D5", $"rawParam = {rawParam}");
                    //Info("C7DB006B-B05A-4D88-A5B5-81562472D174", $"_currentCodeFrame.CurrentPositionOfResolvedParameter = {_currentCodeFrame.CurrentPositionOfResolvingParameter}");
#endif

                    var loadingMatrixValue = loadingMatrix[_currentCodeFrame.CurrentPositionOfResolvingParameter];

#if DEBUG
                    //Info("A1C62064-EED1-46E1-BFF1-D57BF0657E62", $"loadingMatrixValue = {loadingMatrixValue}");
#endif

                    if (loadingMatrixValue == false)
                    {
                        _currentCodeFrame.ResolvedParameterValues.Add(rawParam);

                        continue;
                    }

                    var conversionCallResult = TryResolveFromVarOrExpr(rawParam);

                    var conversionCallResultKindOfResult = conversionCallResult.KindOfResult;

#if DEBUG
                    //Info("585CE384-A233-4069-B5A9-242DA909DA0B", $"conversionCallResultKindOfResult = {conversionCallResultKindOfResult}");
#endif

                    switch (conversionCallResultKindOfResult)
                    {
                        case KindOfCallResult.Value:
                            _currentCodeFrame.ResolvedParameterValues.Add(conversionCallResult.Value);
                            break;

                        case KindOfCallResult.ExecutingCodeInOtherCodeFrame:
                            return new ValueListCallResult() { KindOfResult = KindOfCallResult.ExecutingCodeInOtherCodeFrame };

                        default:
                            throw new ArgumentOutOfRangeException(nameof(conversionCallResultKindOfResult), conversionCallResultKindOfResult, null);
                    }
                }

                _currentCodeFrame.State = CodeFrameState.ResolvedParameters;

                var result = _currentCodeFrame.ResolvedParameterValues;

                EndClearResolvingParameter();

                return new ValueListCallResult(result);
            }

            throw new NotImplementedException("7ED884E5-4A5B-40DE-921D-FD93212D08EB");
        }

        private void BeginningClearResolvingParameter()
        {
            _currentCodeFrame.ResolvingParameterValues = null;
            _currentCodeFrame.ResolvedParameterValues = null;
            _currentCodeFrame.CurrentPositionOfResolvingParameter = -1;
            _currentCodeFrame.CurrentResolvedParameterValue = null;
        }

        private void EndClearResolvingParameter()
        {
            _currentCodeFrame.ResolvingParameterValues = null;
            _currentCodeFrame.CurrentPositionOfResolvingParameter = -1;
            _currentCodeFrame.CurrentResolvedParameterValue = null;
        }

        private static ValueCallResult _executingCodeInOtherCodeFrameCallResult = new ValueCallResult() { KindOfResult = KindOfCallResult.ExecutingCodeInOtherCodeFrame };

        private ValueCallResult TryResolveFromVarOrExpr(Value operand)
        {
#if DEBUG
            //Info("A58897AB-CC50-48C6-8CC9-6FC7949D7E16", $"operand = {operand}");
#endif

            var callResult = _valueResolvingHelper.TryResolveFromVarOrExpr(Logger, operand, _currentCodeFrame.LocalContext);

#if DEBUG
            //Info("6C51558E-F0C3-40AB-9CAD-DE253758EB89", $"callResult = {callResult}");
#endif

            var kindOfResult = callResult.KindOfResult;

            switch(kindOfResult)
            {
                case KindOfCallResult.Value:
                    return callResult;

                case KindOfCallResult.NeedExecuteGetProperty:
                    _currentCodeFrame.State = CodeFrameState.ResolvingParameterInCodeFrame;
                    CallExecutable(
                        executable: callResult.Executable,
                        forParameterValueResolving: true);
                    return _executingCodeInOtherCodeFrameCallResult;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
            }
        }

        private List<Value> NTakePositionedParameters(int count, bool needRevers)
        {
            var result = new List<Value>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                result.Add(valueStack.Pop());
            }

            if(needRevers)
            {
                result.Reverse();
            }            

            return result;
        }

        [Obsolete]
        private List<Value> TakePositionedParametersOld(int count)
        {
            throw new NotImplementedException("131EAE27-B27C-4E7A-B178-E1B7D2D92786");
            //return NTakePositionedParameters(count, true);
        }

        [Obsolete]
        private Dictionary<StrongIdentifierValue, Value> TakeNamedParametersOld(int count)
        {
            throw new NotImplementedException("135A0D8C-6DC5-460C-880F-0A570858520D");

            /*var rawParamsList = NTakePositionedParameters(count * 2, true);

            var result = new Dictionary<StrongIdentifierValue, Value>();

            var enumerator = rawParamsList.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var name = enumerator.Current.AsStrongIdentifierValue;

                enumerator.MoveNext();

                var value = enumerator.Current;

                result[name] = value;
            }

            return result;*/
        }

        private void CallConstructor(KindOfFunctionParameters kindOfParameters, int parametersCount, IAnnotatedItem annotatedItem)
        {
            var valueStack = _currentCodeFrame.ValuesStack;

            var caller = valueStack.Pop();

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParametersOld(parametersCount);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParametersOld(parametersCount);
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

            ConstructorResolvingResult constructorResolvingResult = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveOnlyOwn(Logger, constructorName, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveOnlyOwn(Logger, constructorName, namedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    constructorResolvingResult = _constructorsResolver.ResolveOnlyOwn(Logger, constructorName, positionedParameters, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if(constructorResolvingResult.NeedTypeConversion)
            {
                throw new NotImplementedException("E5818EA6-6D9F-42D7-84C8-67AEDE002D6A");
            }

            IExecutable constructor = constructorResolvingResult.Constructor;

            CallExecutable(constructor, null, kindOfParameters, namedParameters, positionedParameters, annotatedItem, SyncOption.Ctor, false);
        }

        private void CallDefaultCtors()
        {
            var optionsForInheritanceResolver = new ResolverOptions();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance = true;

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(Logger, _currentCodeFrame.LocalContext.Owner, _currentCodeFrame.LocalContext, optionsForInheritanceResolver);

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
                var constructorResolvingResult = _constructorsResolver.ResolveOnlyOwn(Logger, targetSuperClass, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

                if(constructorResolvingResult == null)
                {
                    continue;
                }

                if(constructorResolvingResult.NeedTypeConversion)
                {
                    throw new NotImplementedException("96995AA8-0685-4705-B42A-26240AC959E8");
                }

                var constructor = constructorResolvingResult.Constructor;

                var coordinator = ((IExecutable)constructor).GetCoordinator(Logger, _context, _currentCodeFrame.LocalContext);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(Logger, _currentCodeFrame.Instance, constructor, KindOfFunctionParameters.NoParameters, null, null, _currentCodeFrame.LocalContext, null);

                executionsList.Add((newCodeFrame, coordinator));
            }

            ExecuteCodeFramesBatch(executionsList);
        }

        private void ProcessExecCallEvent()
        {
            var currentCodeFrame = _currentCodeFrame;
            var pseudoSyncTask = currentCodeFrame.PseudoSyncTask;

#if DEBUG
            //Info("AD8D2320-FA82-4575-ACF6-638EF422A493", $"pseudoSyncTask == null = {pseudoSyncTask == null}");
#endif

            if (pseudoSyncTask == null)
            {
#if DEBUG
                //Info("5AEFA952-C546-4367-9F92-AEBB86D23A7D", $"currentCodeFrame.NeedsExecCallEvent = {currentCodeFrame.NeedsExecCallEvent}");
#endif

                if (!currentCodeFrame.NeedsExecCallEvent)
                {
                    _currentCodeFrame.CurrentPosition++;
                    return;
                }

                var lastProcessStatus = currentCodeFrame.LastProcessStatus;

#if DEBUG
                //Info("B3BCD764-486B-4DF0-90CA-3120249A00D7", $"lastProcessStatus = {lastProcessStatus}");
#endif

                if(lastProcessStatus.HasValue)
                {
                    var lastProcessStatusValue = lastProcessStatus.Value;

                    switch (lastProcessStatusValue)
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

                                if (weakCancelAnnotationSystemEvent != null)
                                {
                                    ExecCallEvent(weakCancelAnnotationSystemEvent);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

                currentCodeFrame.NeedsExecCallEvent = false;
                currentCodeFrame.LastProcessStatus = null;
                currentCodeFrame.CompleteAnnotationSystemEvent = null;
                currentCodeFrame.CancelAnnotationSystemEvent = null;
                currentCodeFrame.WeakCancelAnnotationSystemEvent = null;
                currentCodeFrame.ErrorAnnotationSystemEvent = null;

                return;
            }

#if DEBUG
            //Info("51153D21-72AF-488E-A475-F40A667117CC", $"pseudoSyncTask?.Status = {pseudoSyncTask?.Status}");
#endif

            if (pseudoSyncTask.RunningStatus == ThreadTaskStatus.Running)
            {
                return;
            }

            currentCodeFrame.PseudoSyncTask = null;

            _currentCodeFrame.CurrentPosition++;
        }

        private void ExecCallEvent(AnnotationSystemEvent annotationSystemEvent)
        {
            var coordinator = ((IExecutable)annotationSystemEvent).GetCoordinator(Logger, _context, _currentCodeFrame.LocalContext);

            if (coordinator == null)
            {
                coordinator = _currentCodeFrame.ExecutionCoordinator;
            }

            var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(Logger, _currentCodeFrame.Instance, annotationSystemEvent, KindOfFunctionParameters.NoParameters, null, null, _currentCodeFrame.LocalContext, null, true);

            if (annotationSystemEvent.IsSync)
            {
                ExecuteCodeFrame(newCodeFrame, coordinator, SyncOption.PseudoSync);
            }
            else
            {
                ExecuteCodeFrame(newCodeFrame, coordinator, SyncOption.ChildAsync, false);
            }
        }

        private void CallFunction(KindOfFunctionParameters kindOfParameters, int parametersCount, SyncOption syncOption, IAnnotatedItem annotatedItem)
        {
#if DEBUG
            //Info("7F3384D3-5741-41D8-89CD-4A0A515AA647", "Begin");
            //Info("7B518325-43A0-4457-BA92-BC77E99C96BE", $"kindOfParameters = {kindOfParameters}");
            //Info("D793791F-5F17-478E-8243-A5FA6F944D85", $"parametersCount = {parametersCount}");
            //Info("606CEBF8-80AE-4767-B341-BE1FDF2A26F6", $"syncOption = {syncOption}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var caller = valueStack.Pop();

#if DEBUG
            //Info("B480D9AB-70E4-4D5B-BFC0-AB9274AD0A64", $"caller = {caller}");
            //Info("B480D9AB-70E4-4D5B-BFC0-AB9274AD0A64", $"caller = {caller.ToHumanizedString()}");
            //Info("5D38AC5A-D24F-4CA3-9AA7-7D9A76DF0BA7", $"_currentCodeFrame.ProcessInfo.ToHumanizedLabel() = {_currentCodeFrame.ProcessInfo.ToHumanizedLabel()}");
            //Info("A513C3A8-C4EB-4398-9B59-3B929A7FDAFF", $"_currentCodeFrame.ProcessInfo.ToHumanizedString() = {_currentCodeFrame.ProcessInfo.ToHumanizedString()}");
            //Info("A513C3A8-C4EB-4398-9B59-3B929A7FDAFF", $"_currentCodeFrame.ProcessInfo.ToLabel(Logger) = {_currentCodeFrame.ProcessInfo.ToLabel(Logger)}");
#endif

            var chainOfProcessInfo = _currentCodeFrame.ProcessInfo.ToChainOfProcessInfoLabels(Logger);

#if DEBUG
            //Info("36B2ED7D-786B-4B1E-B46B-7CEF2728A45C", $"chainOfProcessInfo = {chainOfProcessInfo.WriteListToString()}");
#endif

            var callMethodId = Logger.CallMethod("7854EEB4-52A1-4B41-95BD-5417B983EB27", caller, chainOfProcessInfo, syncOption == SyncOption.Sync);

#if DEBUG
            //Info("06192378-BE2C-4F6E-9664-C6DFF60EBDDE", $"callMethodId = {callMethodId}");
#endif

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParametersOld(parametersCount);
                    foreach(var item in namedParameters)
                    {
                        Logger.Parameter("1DF0F0FD-D7CE-49E1-91A7-A62A5A75C4CD", callMethodId, item.Key, item.Value);
                    }
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParametersOld(parametersCount);
                    {
                        var n = 0;

                        foreach (var item in positionedParameters)
                        {
                            n++;

                            Logger.Parameter("44E75D5D-FF36-4696-9174-243ED77568AE", callMethodId, n.ToString(), item);
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            if (caller.IsPointRefValue)
            {
                CallPointRefValue(callMethodId, caller.AsPointRefValue, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption);

                Logger.EndCallMethod("A96D8714-A701-4367-844C-51B0F2AD95F5", callMethodId);

                return;
            }

            if(caller.IsHostMethodValue)
            {
                CallHost(callMethodId, caller.AsHostMethodValue.MethodName, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption);

                Logger.EndCallMethod("26A17EE4-DD14-42BD-B885-189BF8D28F89", callMethodId);

                return;
            }

            if (caller.IsStrongIdentifierValue)
            {
                CallStrongIdentifierValue(callMethodId, caller.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption, true);

                Logger.EndCallMethod("EAEBB22E-DAD3-4359-9EA6-B5B73EF65587", callMethodId);

                return;
            }

            if(caller.IsInstanceValue)
            {
                CallInstanceValue(callMethodId, caller.AsInstanceValue, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption);

                Logger.EndCallMethod("8AFEC685-DFF0-4C36-9942-A475C4313BEF", callMethodId);

                return;
            }

            throw new NotImplementedException("7FFA67FB-7766-4D73-9AAE-08E63138A383");
        }

        private void CallInstanceValue(string callMethodId, InstanceValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            IAnnotatedItem annotatedItem, SyncOption syncOption)
        {
            var executable = caller.GetExecutable(Logger, kindOfParameters, namedParameters, positionedParameters);

            CallExecutable(
                callMethodId: callMethodId,
                instance: caller.InstanceInfo,
                executable: executable,
                ownLocalCodeExecutionContext: executable.OwnLocalCodeExecutionContext,
                kindOfParameters:kindOfParameters,
                namedParameters:namedParameters,
                positionedParameters:positionedParameters,
                annotatedItem:annotatedItem,
                syncOption:syncOption,
                forParameterValueResolving: false);
        }
         
        private void CallPointRefValue(string callMethodId, PointRefValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            IAnnotatedItem annotatedItem, SyncOption syncOption)
        {
            throw new NotImplementedException("7A74E818-3F41-44F7-A16A-BE4AA4AD5075");

            /*var callerLeftOperand = caller.LeftOperand;
            var callerRightOperand = caller.RightOperand;

            if (callerLeftOperand.IsHostValue)
            {
                CallHost(callMethodId, callerRightOperand.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption);
                return;
            }

            if(callerLeftOperand.IsStrongIdentifierValue)
            {
                throw new NotImplementedException("1739B790-BBAE-40DB-88A5-93A64D965F59");
            }

            var methodName = callerRightOperand.AsStrongIdentifierValue;

            var method = callerLeftOperand.GetMethod(Logger, methodName, kindOfParameters, namedParameters, positionedParameters);

            CallExecutable(callMethodId, null, method, null, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption, false);*/
        }

        private void CallHost(string callMethodId, StrongIdentifierValue methodName, 
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            IAnnotatedItem annotatedItem, SyncOption syncOption)
        {
#if DEBUG
            //Log($"methodName = {methodName.ToHumanizedString()}");
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

            AnnotationSystemEvent completeAnnotationSystemEvent = null;
            AnnotationSystemEvent cancelAnnotationSystemEvent = null;
            AnnotationSystemEvent weakCancelAnnotationSystemEvent = null;
            AnnotationSystemEvent errorAnnotationSystemEvent = null;

            if (annotatedItem != null)
            {
                var annotationSystemEventsDict = AnnotationsHelper.GetAnnotationSystemEventsDict(annotatedItem);

                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Complete, out completeAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Cancel, out cancelAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.WeakCancel, out weakCancelAnnotationSystemEvent);
                annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Error, out errorAnnotationSystemEvent);
            }

            var processCreatingResult = _hostListener.CreateProcess(GetTargetLogger(syncOption), callMethodId, command, _context, _currentCodeFrame.LocalContext);

            Logger.SystemExpr("3D14A97D-41AB-4E21-82AF-2AC810BAE68F", callMethodId, "processCreatingResult.IsSuccessful", processCreatingResult.IsSuccessful);

#if DEBUG
            //Info("91939B48-DB56-416A-90BB-F13F4A613796", $"processCreatingResult = {processCreatingResult}");
#endif

            if (processCreatingResult.IsSuccessful)
            {
                var processInfo = processCreatingResult.Process;

                Logger.SystemExpr("8235EAFC-9BA3-427B-BF09-9509265D9B1E", callMethodId, "processInfo.EndPointName", processInfo.EndPointName);
                Logger.SystemExpr("C618CB67-0C92-492D-8708-A77311FDDCE3", callMethodId, "processInfo.Devices", processInfo.Devices.WritePODListToString());
                Logger.SystemExpr("ED733C9C-CBBB-487B-904B-E01D1AB75E81", callMethodId, "processInfo.Friends", processInfo.Friends.WritePODListToString());
                Logger.SystemExpr("C3AA40B1-CEB6-4B23-ADEB-C8EC56EC9898", callMethodId, "processInfo.Priority", processInfo.Priority);

                _instancesStorage.AppendAndTryStartProcessInfo(Logger, callMethodId, processInfo);

                var timeout = GetTimeoutFromAnnotation(annotatedItem);
                var timeoutCancellationMode = GetTimeoutCancellationModeFromAnnotation(annotatedItem);

                if (syncOption == SyncOption.Sync)
                {
                    var currentProcessInfo = _currentCodeFrame.ProcessInfo;

                    processInfo.ParentProcessInfo = currentProcessInfo;

                    List<IExecutionCoordinator> executionCoordinators = null;

                    if (_executionCoordinator != null)
                    {
                        executionCoordinators = new List<IExecutionCoordinator>() { _executionCoordinator };
                    }

#if DEBUG
                    //Info("20507B37-E3DE-460B-9E4E-40F079D3EFB1", $"Before ProcessInfoHelper.Wait");
#endif

                    ProcessInfoHelper.Wait(Logger, callMethodId, currentProcessInfo, executionCoordinators, timeout, timeoutCancellationMode, _dateTimeProvider, processInfo);

#if DEBUG
                    //Info("EEB44353-D9EF-4AA9-8535-0A10F686BA2B", $"After ProcessInfoHelper.Wait");
#endif

                    if (_executionCoordinator != null && _executionCoordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                    {
                        ProcessError(_executionCoordinator.RuleInstance);

                        return;
                    }

                    var status = processInfo.Status;

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
                            _currentCodeFrame.ProcessInfo.SetStatus(Logger, "60EE6E1D-1FCE-4E43-AF5F-2FDB574CCDE0", ProcessStatus.Canceled);

                            if (cancelAnnotationSystemEvent != null)
                            {
                                ExecCallEvent(cancelAnnotationSystemEvent);
                            }

                            _isCanceled = true;

                            GoBackToPrevCodeFrame("3D0329B2-6EAC-470C-AA88-D1DEC6E41374", ActionExecutionStatus.Canceled);
                            return;

                        case ProcessStatus.Faulted:
                            _currentCodeFrame.ProcessInfo.SetStatus(Logger, "CF28AF35-A9E0-4BBB-9079-9EB77CBC51A5", ProcessStatus.Faulted);

                            if (errorAnnotationSystemEvent != null)
                            {
                                throw new NotImplementedException("A37B281E-CE1A-4B49-AC0A-7266360C43B7");
                            }

                            GoBackToPrevCodeFrame("05D7C078-24B4-4189-81F2-59BDA492A1D2", ActionExecutionStatus.Faulted);
                            return;
                    }
                }
                else
                {
                    var currentProcessInfo = _currentCodeFrame.ProcessInfo;

                    var processInfoValue = new ProcessInfoValue(processInfo);

                    if (syncOption == SyncOption.ChildAsync)
                    {
                        processInfo.ParentProcessInfo = currentProcessInfo;
                    }

                    if(timeout.HasValue)
                    {
                        List<IExecutionCoordinator> executionCoordinators = null;

                        if (_executionCoordinator != null)
                        {
                            executionCoordinators = new List<IExecutionCoordinator>() { _executionCoordinator };
                        }

                        //This will be replaces in other way.
                        ThreadTask.Run(() => {
                            ProcessInfoHelper.Wait(Logger, callMethodId, currentProcessInfo, executionCoordinators, timeout, timeoutCancellationMode, _dateTimeProvider, processInfo);
                        }, _context.CodeExecutionThreadPool, _context.GetCancellationToken());
                    }

                    if (completeAnnotationSystemEvent != null)
                    {
                        processInfo.AddOnCompleteHandler(Logger, new ProcessInfoEventHandler(_context, Logger.Id, completeAnnotationSystemEvent, _currentCodeFrame, true));
                    }

                    if (cancelAnnotationSystemEvent != null)
                    {
                        throw new NotImplementedException("59854144-10BA-444C-82E3-4BCC0677743E");
                    }

                    if (weakCancelAnnotationSystemEvent != null)
                    {
                        processInfo.AddOnWeakCanceledHandler(Logger, new ProcessInfoEventHandler(_context, Logger.Id, weakCancelAnnotationSystemEvent, _currentCodeFrame, true));
                    }

                    if (errorAnnotationSystemEvent != null)
                    {
                        throw new NotImplementedException("ADB7D70D-29EF-4245-ABC5-3962823A9915");
                    }

                    _currentCodeFrame.ValuesStack.Push(processInfoValue);
                }

                _currentCodeFrame.CurrentPosition++;

                return;
            }

            throw new NotImplementedException("312E1089-2560-45BC-A31C-53A25D915395");
        }

        private void CallStrongIdentifierValue(string callMethodId, StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            IAnnotatedItem annotatedItem, SyncOption syncOption, bool mayCallHost)
        {
#if DEBUG
            //Info("03ED4F39-8D56-49C9-9E33-80B4C7674FEA", $"methodName = {methodName.ToHumanizedString()}");
#endif

            MethodResolvingResult methodResolvingResult = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    methodResolvingResult = _methodsResolver.Resolve(Logger, callMethodId, methodName, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    methodResolvingResult = _methodsResolver.Resolve(Logger, callMethodId, methodName, namedParameters, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    methodResolvingResult = _methodsResolver.Resolve(Logger, callMethodId, methodName, positionedParameters, _currentCodeFrame.LocalContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

#if DEBUG
            Logger.SystemExpr("010446DC-DBA6-43A8-B128-3B42625803C6", callMethodId, $"methodResolvingResult != null", methodResolvingResult != null);
#endif

            if (methodResolvingResult == null)
            {
                if(mayCallHost)
                {
                    CallHost(callMethodId, methodName, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption);
                    return;
                }

                throw new Exception($"Method '{methodName.NameValue}' is not found.");
            }

            if(methodResolvingResult.NeedTypeConversion)
            {
#if DEBUG
                //Info("749B55BB-87E6-4867-A6C9-014670811073", $"methodResolvingResult.ParametersRankMatrix = {methodResolvingResult.ParametersRankMatrix.WriteListToString()}");
#endif

                switch (kindOfParameters)
                {
                    case KindOfFunctionParameters.NoParameters:
                        break;

                    case KindOfFunctionParameters.NamedParameters:
                        namedParameters = _methodsResolver.PrepareParameters(Logger, namedParameters, methodResolvingResult.ParametersRankMatrix, _currentCodeFrame.LocalContext);
                        break;

                    case KindOfFunctionParameters.PositionedParameters:
                        positionedParameters = _methodsResolver.PrepareParameters(Logger, positionedParameters, methodResolvingResult.ParametersRankMatrix, _currentCodeFrame.LocalContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }

#if DEBUG
                //Info("FE20FB02-F9A5-4314-A9F3-88E465D0E40A", $"positionedParameters = {positionedParameters.WriteListToString()}");
                //Info("66B61BB0-7017-4AE8-8F15-82F142AB7721", $"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif
            }

            CallExecutable(callMethodId, methodResolvingResult.Instance, methodResolvingResult.Executable, null, kindOfParameters, namedParameters, positionedParameters, annotatedItem, syncOption, false);
        }

        private void ExecRuleInstanceValue(RuleInstance ruleInstance)
        {
            var compiledCode = _converterFactToImperativeCode.Convert(Logger, ruleInstance, _currentCodeFrame.LocalContext);

            var codeFrame = _codeFrameService.ConvertCompiledFunctionBodyToCodeFrame(Logger, _currentInstance, compiledCode, _currentCodeFrame.LocalContext);

            ExecuteCodeFrame(codeFrame, null, SyncOption.Sync);
        }

        private ulong? GetTimeoutFromAnnotation(IAnnotatedItem annotatedItem)
        {
            var initialValue = GetInitialSettingsValueFromAnnotation(annotatedItem, _timeoutName);

            if(initialValue == null)
            {
                return null;
            }

            return _dateTimeResolver.ConvertTimeValueToTicks(Logger, initialValue, DefaultTimeValues.TimeoutDefaultTimeValue, _currentCodeFrame.LocalContext);
        }

        private TimeoutCancellationMode _defaultTimeoutCancellationMode = TimeoutCancellationMode.WeakCancel;

        private TimeoutCancellationMode GetTimeoutCancellationModeFromAnnotation(IAnnotatedItem annotatedItem)
        {
            var meaningRoles = annotatedItem?.MeaningRolesList;

            if(meaningRoles.IsNullOrEmpty())
            {
                return _defaultTimeoutCancellationMode;
            }

            if(meaningRoles.Where(p => p.IsStrongIdentifierValue).Select(p => p.AsStrongIdentifierValue).Any(p => p.NormalizedNameValue == "cancel"))
            {
                return TimeoutCancellationMode.Cancel;
            }

            if(meaningRoles.Where(p => p.IsSequenceValue).Select(p => p.AsSequenceValue).Where(p => p.Values.Count == 2 && p.Values[0].AsStrongIdentifierValue?.NormalizedNameValue == "weak" && p.Values[1].AsStrongIdentifierValue?.NormalizedNameValue == "cancel").Any())
            {
                return TimeoutCancellationMode.WeakCancel;
            }

            return _defaultTimeoutCancellationMode;
        }

        private float? GetPriorityFromAnnotation(IAnnotatedItem annotatedItem)
        {
            var numberValue = GetSettingsFromAnnotation(annotatedItem, _priorityName);

            if (numberValue == null)
            {
                return null;
            }

            return Convert.ToSingle(numberValue.SystemValue.Value);
        }

        private NumberValue GetSettingsFromAnnotation(IAnnotatedItem annotatedItem, StrongIdentifierValue settingName)
        {
            var initialValue = GetInitialSettingsValueFromAnnotation(annotatedItem, settingName);

            if (initialValue == null)
            {
                return null;
            }

            var numberValue = _numberValueLinearResolver.Resolve(Logger, initialValue, _currentCodeFrame.LocalContext);

            if (numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue)
            {
                return null;
            }

            return numberValue;
        }

        private Value GetInitialSettingsValueFromAnnotation(IAnnotatedItem annotatedItem, StrongIdentifierValue settingName)
        {
            if (annotatedItem == null)
            {
                return null;
            }

            var initialValue = _annotationsResolver.GetSettings(Logger, annotatedItem, settingName, _currentCodeFrame.LocalContext);

            if (initialValue == null || initialValue.KindOfValue == KindOfValue.NullValue)
            {
                return null;
            }

            return initialValue;
        }


        private void CallOperator(Operator op, List<Value> positionedParameters)
        {
            CallExecutable(
                executable: op,
                positionedParameters:positionedParameters);
        }

        private void CallExecutable(IExecutable executable)
        {
            CallExecutable(
                executable: executable,
                forParameterValueResolving: false);
        }

        private void CallExecutable(IExecutable executable, bool forParameterValueResolving)
        {
            CallExecutable(
                executable: executable,
                ownLocalCodeExecutionContext: null,
                kindOfParameters: KindOfFunctionParameters.NoParameters,
                namedParameters: null,
                positionedParameters: null,
                annotatedItem: null,
                syncOption: SyncOption.Sync,
                forParameterValueResolving: forParameterValueResolving);
        }
        
        private void CallExecutable(IExecutable executable, List<Value> positionedParameters)
        {
            CallExecutable(
                executable: executable,
                ownLocalCodeExecutionContext: null,
                kindOfParameters: KindOfFunctionParameters.PositionedParameters,
                namedParameters: null,
                positionedParameters: positionedParameters,
                annotatedItem: null,
                syncOption: SyncOption.Sync,
                forParameterValueResolving: false);
        }

        private void CallExecutable(IExecutable executable, ILocalCodeExecutionContext ownLocalCodeExecutionContext, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, IAnnotatedItem annotatedItem, SyncOption syncOption, bool forParameterValueResolving)
        {
            CallExecutable(
                callMethodId: string.Empty,
                instance: null,
                executable: executable,
                ownLocalCodeExecutionContext: ownLocalCodeExecutionContext,
                kindOfParameters:kindOfParameters,
                namedParameters:namedParameters,
                positionedParameters:positionedParameters,
                annotatedItem: annotatedItem,
                syncOption: syncOption,
                forParameterValueResolving: forParameterValueResolving);
        }
        
        private void CallExecutable(string callMethodId, IInstance instance, IExecutable executable, ILocalCodeExecutionContext ownLocalCodeExecutionContext, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, IAnnotatedItem annotatedItem, SyncOption syncOption, bool forParameterValueResolving)
        {
#if DEBUG
            //Info("B39E497B-B02E-41DD-AC8F-A69910597590", $"Begin");
            //Info("8248ABAF-2A3B-44CB-A229-365F0FF8DC8B", $"executable == null = {executable == null}");
            //Info("93048AF5-9F86-417D-9C67-05707B8421EF", $"kindOfParameters = {kindOfParameters}");
            //Info("B235F9CD-642F-46C3-A1E8-52CA99572538", $"syncOption = {syncOption}");
#endif

            if (executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var targetLocalContext = ownLocalCodeExecutionContext == null? _currentCodeFrame.LocalContext : ownLocalCodeExecutionContext;

#if DEBUG
            //Info("4496A69E-77C4-4A6F-A2CF-D53E98F466A0", $"executable.IsSystemDefined = {executable.IsSystemDefined}");
#endif

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch(kindOfParameters)
                {
                    case KindOfFunctionParameters.NoParameters:
                        {
                            var callResult = executable.SystemHandler.Call(Logger, new List<Value>(), annotatedItem, targetLocalContext, _currentCodeFrame.CallMode);

                            switch(callResult.KindOfResult)
                            {
                                case KindOfCallResult.Value:
                                    result = callResult.Value;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(callResult.KindOfResult), callResult.KindOfResult, null);
                            }
                        }                        
                        break;

                    case KindOfFunctionParameters.PositionedParameters:
                        {
                            var callResult = executable.SystemHandler.Call(Logger, positionedParameters, annotatedItem, targetLocalContext, _currentCodeFrame.CallMode);

                            switch (callResult.KindOfResult)
                            {
                                case KindOfCallResult.Value:
                                    result = callResult.Value;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(callResult.KindOfResult), callResult.KindOfResult, null);
                            }
                        }
                        break;

                    case KindOfFunctionParameters.NamedParameters:
                        {
                            var callResult = executable.SystemHandler.Call(Logger, namedParameters.ToDictionary(p => p.Key.NameValue, p => p.Value), annotatedItem, targetLocalContext, _currentCodeFrame.CallMode);

                            switch (callResult.KindOfResult)
                            {
                                case KindOfCallResult.Value:
                                    result = callResult.Value;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(callResult.KindOfResult), callResult.KindOfResult, null);
                            }
                        }
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

                if (annotatedItem != null)
                {
                    var annotationSystemEventsDict = AnnotationsHelper.GetAnnotationSystemEventsDict(annotatedItem);

                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Complete, out completeAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Cancel, out cancelAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.WeakCancel, out weakCancelAnnotationSystemEvent);
                    annotationSystemEventsDict.TryGetValue(KindOfAnnotationSystemEvent.Error, out errorAnnotationSystemEvent);
                }

                if (executable.NeedActivation && !executable.IsActivated)
                {
                    executable = executable.Activate(Logger, _context, _currentCodeFrame.LocalContext, _executionCoordinator);
                }

                var coordinator = executable.GetCoordinator(Logger, _context, _currentCodeFrame.LocalContext);

                if (executable.UsingLocalCodeExecutionContextPreferences == UsingLocalCodeExecutionContextPreferences.UseOwnAsParent || executable.UsingLocalCodeExecutionContextPreferences == UsingLocalCodeExecutionContextPreferences.UseBothOwnAndCallerAsParent)
                {
                    targetLocalContext = executable.OwnLocalCodeExecutionContext;
                }

                var additionalSettings = GetAdditionalSettingsFromAnnotation(
                    annotatedItem: annotatedItem,
                    ownLocalCodeExecutionContext: ownLocalCodeExecutionContext,
                    forParameterValueResolving: forParameterValueResolving);

                var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(
                    logger: Logger,
                    callMethodId: callMethodId,
                    instance: instance,
                    function: executable,
                    kindOfParameters: kindOfParameters,
                    namedParameters:namedParameters,
                    positionedParameters:positionedParameters,
                    parentLocalCodeExecutionContext: targetLocalContext,
                    additionalSettings:additionalSettings);

                ExecuteCodeFrame(
                    codeFrame: newCodeFrame,
                    coordinator: coordinator,
                    syncOption: syncOption,
                    increaseCurrentFramePosition: forParameterValueResolving ? false: true,
                    completeAnnotationSystemEvent:completeAnnotationSystemEvent,
                    cancelAnnotationSystemEvent:cancelAnnotationSystemEvent,
                    weakCancelAnnotationSystemEvent:weakCancelAnnotationSystemEvent,
                    errorAnnotationSystemEvent:errorAnnotationSystemEvent);
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

                var processInfo = codeFrame.ProcessInfo;

                coordinator?.AddProcessInfo(processInfo);
                _context.InstancesStorage.AppendProcessInfo(Logger, processInfo);

                PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                SetCodeFrame(codeFrame);
            }
        }

        private ConversionExecutableToCodeFrameAdditionalSettings GetAdditionalSettingsFromAnnotation(IAnnotatedItem annotatedItem, ILocalCodeExecutionContext ownLocalCodeExecutionContext, bool forParameterValueResolving)
        {
            var timeout = GetTimeoutFromAnnotation(annotatedItem);
            var timeoutCancellationMode = GetTimeoutCancellationModeFromAnnotation(annotatedItem);
            var priority = GetPriorityFromAnnotation(annotatedItem);

            ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null;

            if (timeout.HasValue || priority.HasValue)
            {
                additionalSettings ??= new ConversionExecutableToCodeFrameAdditionalSettings();

                additionalSettings.Timeout = timeout;
                additionalSettings.TimeoutCancellationMode = timeoutCancellationMode;
            }

            if(priority.HasValue)
            {
                additionalSettings ??= new ConversionExecutableToCodeFrameAdditionalSettings();

                additionalSettings.Priority = priority;
            }

            if (ownLocalCodeExecutionContext != null)
            {
                additionalSettings ??= new ConversionExecutableToCodeFrameAdditionalSettings();

                additionalSettings.AllowParentLocalStorages = true;
            }

            if(forParameterValueResolving)
            {
                additionalSettings ??= new ConversionExecutableToCodeFrameAdditionalSettings();

                additionalSettings.ForParameterValueResolving = true;
            }

            return additionalSettings;
        }

        private void ExecuteCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaseCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            ExecuteCodeFrame(codeFrame, null, coordinator, syncOption, increaseCurrentFramePosition, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
        }

        private void ExecuteCodeFrame(CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaseCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            var targetCurrentCodeFrame = _currentCodeFrame;

            if(currentCodeFrame != null)
            {
                targetCurrentCodeFrame = currentCodeFrame;
            }

            var currentProcessInfo = codeFrame.ProcessInfo;

            coordinator?.AddProcessInfo(currentProcessInfo);

            _context.InstancesStorage.AppendProcessInfo(Logger, currentProcessInfo);

            switch (syncOption)
            {
                case SyncOption.Sync:
                case SyncOption.Ctor:
                    PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                    if(increaseCurrentFramePosition)
                    {
                        targetCurrentCodeFrame.CurrentPosition++;
                        targetCurrentCodeFrame.ProcessInfo.AddChild(Logger, currentProcessInfo);
                    }

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
                    _codeFrameAsyncExecutor.AsyncExecuteCodeFrame(Logger, Logger.Id, codeFrame, targetCurrentCodeFrame, coordinator, syncOption, increaseCurrentFramePosition, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
                    break;

                case SyncOption.PseudoSync:
                    {
                        PrepareCodeFrameToSyncExecution(codeFrame, coordinator);

                        var threadExecutor = new AsyncThreadExecutor(_context, _context.CodeExecutionThreadPool);
                        threadExecutor.SetCodeFrame(codeFrame);

                        threadExecutor.Start();

                        targetCurrentCodeFrame.PseudoSyncTask = threadExecutor;
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

                    _currentInstance.AddChildInstance(Logger, instance);
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
            var paramsList = TakePositionedParametersOld(4);

            var inheritanceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritanceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = _strongIdentifierLinearResolver.Resolve(Logger, paramsList[0], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(Logger, paramsList[1], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var rank = paramsList[2];//_logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true);

            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = rank;

            _globalStorage.InheritanceStorage.SetInheritance(Logger, inheritanceItem);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetNotInheritance()
        {
            var paramsList = TakePositionedParametersOld(4);

            var inheritanceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritanceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

            var rank = _logicalValueLinearResolver.Resolve(Logger, paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true).Inverse();

            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = rank;

            _globalStorage.InheritanceStorage.SetInheritance(Logger, inheritanceItem);

            _currentCodeFrame.CurrentPosition++;
        }
    }
}
