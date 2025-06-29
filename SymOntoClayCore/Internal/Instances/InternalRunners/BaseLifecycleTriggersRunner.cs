using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public abstract class BaseLifecycleTriggersRunner
    {
        protected BaseLifecycleTriggersRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator, IStorage storage, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, bool normalOrder, bool runOnce)
        {
            _logger = logger;
            _context = context;
            _instance = instance;
            _instanceName = instance.Name;
            _holder = holder;
            _localCodeExecutionContext = localCodeExecutionContext;
            _executionCoordinator = executionCoordinator;
            _storage = storage;
            _kindOfSystemEvent = kindOfSystemEvent;
            _normalOrder = normalOrder;
            _runOnce = runOnce;

            var dataResolversFactory = context.DataResolversFactory;

            _triggersResolver = dataResolversFactory.GetTriggersResolver();
        }

        private readonly IMonitorLogger _logger;
        private readonly IEngineContext _context;
        private readonly TriggersResolver _triggersResolver;
        private readonly IInstance _instance;
        private readonly StrongIdentifierValue _instanceName;
        private readonly StrongIdentifierValue _holder;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IStorage _storage;
        private readonly KindOfSystemEventOfInlineTrigger _kindOfSystemEvent;
        private readonly bool _normalOrder;
        private readonly bool _runOnce;
        private bool _wasRun;
        private IThreadExecutor _threadExecutor;

        private object _lockObj = new object();

        public void Run(IMonitorLogger logger)
        {
            lock(_lockObj)
            {
                if (_runOnce)
                {
                    if (_wasRun)
                    {
                        return;
                    }
                    else
                    {
                        _wasRun = true;
                    }
                }
            }

            var processInitialInfoList = BuildProcessInitialInfoList(logger);

            if(processInitialInfoList.Any())
            {
                _threadExecutor = _context.CodeExecutor.ExecuteBatchSync(logger, processInitialInfoList);
            }
        }

        public void RunAsync(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_runOnce)
                {
                    if (_wasRun)
                    {
                        return;
                    }
                    else
                    {
                        _wasRun = true;
                    }
                }
            }

            var processInitialInfoList = BuildProcessInitialInfoList(logger);

            if (processInitialInfoList.Any())
            {
                _threadExecutor = _context.CodeExecutor.ExecuteBatchAsync(logger, processInitialInfoList, logger.Id);
            }
        }

        private List<ProcessInitialInfo> BuildProcessInitialInfoList(IMonitorLogger logger)
        {
            var processInitialInfoList = new List<ProcessInitialInfo>();

            var targetSystemEventsTriggersList = _triggersResolver.ResolveSystemEventsTriggersList(logger, _kindOfSystemEvent, _holder, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (targetSystemEventsTriggersList.Any())
            {
                if (_normalOrder)
                {
                    targetSystemEventsTriggersList.Reverse();
                }

                logger.RunLifecycleTrigger("96DFB93E-105B-43C1-8FD3-A61C36120BF5", _instanceName?.ToHumanizedLabel(), _holder?.ToHumanizedLabel(), _kindOfSystemEvent);

                foreach (var targetTrigger in targetSystemEventsTriggersList)
                {
#if DEBUG
                    //logger.Info("89D5530C-F6F6-4C6A-A629-9E479A97658C", $"targetTrigger.KindOfInlineTrigger = {targetTrigger.KindOfInlineTrigger}");
                    //logger.Info("9E7AC0D4-9205-452B-BDAA-95442DD6887D", $"targetTrigger.KindOfSystemEvent = {targetTrigger.KindOfSystemEvent}");
                    //logger.Info("D354AC21-F24F-4133-9E0C-9EFF1A0F81C0", $"{nameof(targetTrigger)}.ToHumanizedLabel() = {targetTrigger.ToHumanizedLabel()}");
                    //logger.Info("DBBAA783-AC0E-4E87-A117-B0F4F703E756", $"{nameof(targetTrigger)}.ToHumanizedString() = {targetTrigger.ToHumanizedString()}");
                    //logger.Info("FD422325-65BB-4E70-BC77-CC9C21925C07", $"{nameof(targetTrigger)}.ToLabel(logger) = {targetTrigger.ToLabel(logger)}");
#endif
                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);
                    localCodeExecutionContext.Holder = _holder;
                    localCodeExecutionContext.Instance = _instance;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = targetTrigger.SetCompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = targetTrigger;
                    processInitialInfo.Instance = _instance;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }
            }

            return processInitialInfoList;
        }
    }
}
