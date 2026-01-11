/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public class ConstructorsRunner
    {
        public ConstructorsRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator, IStorage storage)
        {
            _logger = logger;
            _context = context;
            _instance = instance;
            _instanceName = instance.Name;
            _holder = holder;
            _localCodeExecutionContext = localCodeExecutionContext;
            _executionCoordinator = executionCoordinator;
            _storage = storage;

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
        private readonly IStorage _storage;

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

            var constructorsResolvingResultList = _constructorsResolver.ResolveListWithSelfAndDirectInheritance(logger, _instanceName, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (constructorsResolvingResultList.Any())
            {
                var superClassesStoragesDict = _constructorsResolver.GetSuperClassStoragesDict(logger, _localCodeExecutionContext.Storage, _instance);

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var constructorResolvingResult in constructorsResolvingResultList)
                {
                    var constructor = constructorResolvingResult.Constructor;

                    var targetHolder = constructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);
                    localCodeExecutionContext.Holder = _instanceName;
                    localCodeExecutionContext.Instance = _instance;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage = targetStorage;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = constructor.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = constructor;
                    processInitialInfo.Instance = _instance;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

                _threadExecutor = _context.CodeExecutor.ExecuteBatchSync(logger, processInitialInfoList);

            }
        }
    }
}
