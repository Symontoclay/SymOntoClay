using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances
{
    public class CompoundHtnTaskBackgroundTriggerInstance : BaseConditionalTriggerInstance,
        IOnChangedLogicConditionalTriggerObserverHandler,
        ILogicConditionalTriggerInstanceSerializedEventsHandler
    {
        public CompoundHtnTaskBackgroundTriggerInstance(CompoundHtnTaskBackground background, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(parent, context, parentStorage, parentCodeExecutionContext)
        {
            _background = background;

#if DEBUG
            Info("9B79EC73-F673-4A56-B79C-4DBCBBB2ABDF", $"background.CompiledFunctionBody = {background.CompiledFunctionBody?.ToDbgString()}");
            Info("D1D74832-CC30-45E2-95C1-1C1EE7216E90", $"background.Plan = {background.Plan?.ToDbgString()}");
            Info("6E9FC834-A3D8-4ECB-AFD3-3D938E41C1B2", $"background.Condition == null = {background.Condition == null}");
#endif

            var setBindingVariables = new BindingVariables();

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext, background.Condition, KindOfTriggerCondition.SetCondition, setBindingVariables, _localCodeExecutionContext);

            _setConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, background.Condition, KindOfTriggerCondition.SetCondition, _setConditionalTriggerExecutor.LocalCodeExecutionContext);
            _setConditionalTriggerObserver.AddOnChangedHandler(this);
        }

        private CompoundHtnTaskBackground _background;
        //private 
        private readonly LogicConditionalTriggerObserver _setConditionalTriggerObserver;
        private readonly LogicConditionalTriggerExecutor _setConditionalTriggerExecutor;

        void IOnChangedLogicConditionalTriggerObserverHandler.Invoke()
        {
            lock (_lockObj)
            {
                if (_needRun)
                {
                    return;
                }
            }

            LoggedFunctorWithoutResult<ILogicConditionalTriggerInstanceSerializedEventsHandler>.Run(Logger, "F5C7D84D-770E-40D0-AE92-48E497F75A82", this,
                (IMonitorLogger loggerValue, ILogicConditionalTriggerInstanceSerializedEventsHandler instanceValue) => {
                    instanceValue.NObserver_OnChanged();
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void ILogicConditionalTriggerInstanceSerializedEventsHandler.NObserver_OnChanged()
        {
#if DEBUG
            //Info("5AF67C00-E174-436C-9BB0-95889FCE46F9", $"Observer_OnChanged();{_trigger.ToHumanizedLabel()}");
#endif

            lock (_lockObj)
            {
                _needRun = true;
            }
        }

        /// <inheritdoc/>
        protected override void DoSearch(IMonitorLogger logger)
        {
#if DEBUG
            Info("A113FF1E-EF91-480A-BED9-A888FC27CC10", "Run DoSearch");
            //Info("0BC15273-C0D4-4103-A091-2A7D8260DBF1", $"_background.Holder = {_background.Holder}");
#endif

            var doTriggerSearchId = logger.DoTriggerSearch("D57D6399-7E14-4C43-B427-76382E660DBB", _parent.Name.ToHumanizedLabel(), _background.Holder.ToHumanizedLabel(), _background.ToLabel(logger));

            DoSearchWithNoResetCondition(logger, doTriggerSearchId);

            SetInitialTime();

            logger.EndDoTriggerSearch("8F28EDB4-22B8-48AE-BD20-1092CA72DD86", doTriggerSearchId);
        }

        private void DoSearchWithNoResetCondition(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunSetExprOfConditionalTrigger("9048A5A9-8FC7-4090-82FC-AD301169DB48", doTriggerSearchId, _background.Condition.ToLabel(logger));

            var setResult = _setConditionalTriggerExecutor.Run(out List<List<VarInstance>> setVarList);

            logger.EndRunSetExprOfConditionalTrigger("F2774531-0FD2-40F3-AE74-25E62317C0A1", doTriggerSearchId, _background.Condition.ToLabel(logger), setResult.IsSuccess, setResult.IsPeriodic, setVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(logger, doTriggerSearchId, setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(logger, doTriggerSearchId, setResult.IsPeriodic);
                }
            }
            else
            {
                SetIsOn(logger, "D810B8B5-FC3A-4273-95D4-17B4B90DE5FF", doTriggerSearchId, false);
            }
        }

        private void ProcessSetResultWithItems(IMonitorLogger logger, string doTriggerSearchId, List<List<VarInstance>> varList)
        {
            SetIsOn(logger, "F8218C91-0498-4F38-89CC-5D99E75D8D95", doTriggerSearchId, true);

            foreach (var targetVarList in varList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                var storage = new LocalStorage(localStorageSettings);
                localCodeExecutionContext.Storage = storage;
                localCodeExecutionContext.Holder = _parent.Name;
                localCodeExecutionContext.Instance = _parent;

                var varStorage = storage.VarStorage;

                foreach (var varItem in targetVarList)
                {
                    varStorage.Append(Logger, varItem);
                }

                RunSetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
            }
        }

        private void ProcessSetResultWithNoItems(IMonitorLogger logger, string doTriggerSearchId, bool isPeriodic)
        {
#if DEBUG
            Info("22A115BC-1646-4250-9D91-AF5D02A1FAE6", $"doTriggerSearchId = {doTriggerSearchId};isPeriodic = {isPeriodic};_triggerConditionNodeObserverContext.IsOn = {_triggerConditionNodeObserverContext.IsOn};{_background.ToHumanizedLabel()}");
#endif

            if (_triggerConditionNodeObserverContext.IsOn)
            {
                return;
            }

#if DEBUG
            Info("70DD5C0C-5DA9-46AD-AE3A-AF452F48E1CA", $"doTriggerSearchId = {doTriggerSearchId};isPeriodic = {isPeriodic};{_background.ToHumanizedLabel()}");
#endif

            if (!isPeriodic)
            {
                SetIsOn(logger, "C7F96729-A30A-4588-925A-11204240F59D", doTriggerSearchId, true);
            }
            else
            {
                _triggerConditionNodeObserverContext.InitialSetTime = _dateTimeProvider.CurrentTicks;
            }

#if DEBUG
            Info("2595428A-7F07-4B7A-8969-82A31A6821A1", $"doTriggerSearchId = {doTriggerSearchId};{_background.ToHumanizedLabel()}");
#endif

#if DEBUG
            Info("433C2DBE-5065-4B3A-A3C4-F29375389A81", $"doTriggerSearchId = {doTriggerSearchId};Run!!!!!!!!!!!!;{_background.ToHumanizedLabel()}");
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;
            localCodeExecutionContext.Instance = _parent;

            RunSetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
        }

        private void RunSetHandler(IMonitorLogger logger, string doTriggerSearchId, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _background.CompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _background;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);
        }
    }
}
