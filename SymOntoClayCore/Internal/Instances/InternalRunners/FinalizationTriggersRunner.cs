using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public class FinalizationTriggersRunner
    {
        public FinalizationTriggersRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IStorage storage)
        {
            _logger = logger;
            _context = context;
            _instance = instance;
            _holder = holder;
            _localCodeExecutionContext = localCodeExecutionContext;
            _storage = storage;
        }

        private bool _wasRun;
        private object _lockObj = new object();

        private readonly IMonitorLogger _logger;
        private readonly IEngineContext _context;
        private readonly TriggersResolver _triggersResolver;
        private readonly IInstance _instance;
        private readonly StrongIdentifierValue _holder;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IStorage _storage;

        private ExecutionCoordinator _finalizationExecutionCoordinator;
        private LeaveLifecycleTriggersRunner _leaveLifecycleTriggersRunner;

        public void RunAsync(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_wasRun)
                {
                    return;
                }

                _wasRun = true;
            }

            _finalizationExecutionCoordinator = new ExecutionCoordinator(_instance);
            _finalizationExecutionCoordinator.SetExecutionStatus(logger, "86865B32-9839-4442-8EEF-5522E6DAADB4", ActionExecutionStatus.Executing);

            _leaveLifecycleTriggersRunner = new LeaveLifecycleTriggersRunner(logger, _context, _instance, _holder, _localCodeExecutionContext, _finalizationExecutionCoordinator, _storage);
            _leaveLifecycleTriggersRunner.RunAsync(logger);
        }
    }
}
