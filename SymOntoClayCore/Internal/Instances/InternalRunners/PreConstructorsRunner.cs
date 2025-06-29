using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public class PreConstructorsRunner
    {
        public PreConstructorsRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            _logger = logger;
            _context = context;
            _instance = instance;
            _instanceName = instance.Name;
            _holder = holder;
            _localCodeExecutionContext = localCodeExecutionContext;
            _executionCoordinator = executionCoordinator;

            var dataResolversFactory = context.DataResolversFactory;
            _constructorsResolver = dataResolversFactory.GetConstructorsResolver();
        }

        private bool _wasRun;
        private object _lockObj = new object();

        private readonly IMonitorLogger _logger;
        private readonly IEngineContext _context;
        private readonly TriggersResolver _triggersResolver;
        private readonly IInstance _instance;
        private readonly StrongIdentifierValue _instanceName;
        private readonly StrongIdentifierValue _holder;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IExecutionCoordinator _executionCoordinator;

        private readonly ConstructorsResolver _constructorsResolver;

        private IThreadExecutor _threadExecutor;

        public void Run(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_wasRun)
                {
                    return;
                }

                _wasRun = true;
            }

            var preConstructors = _constructorsResolver.ResolvePreConstructors(logger, _instanceName, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (preConstructors.Any())
            {
                var superClassesStoragesDict = _constructorsResolver.GetSuperClassStoragesDict(logger, _localCodeExecutionContext.Storage, _instance);

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var preConstructor in preConstructors)
                {
                    var targetHolder = preConstructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
                    localCodeExecutionContext.Storage = targetStorage;
                    localCodeExecutionContext.Holder = targetHolder;
                    localCodeExecutionContext.Instance = _instance;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage = targetStorage;
                    localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.PreConstructor;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = preConstructor.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = preConstructor;
                    processInitialInfo.Instance = _instance;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

                _threadExecutor = _context.CodeExecutor.ExecuteBatchSync(logger, processInitialInfoList);
            }
        }
    }
}
