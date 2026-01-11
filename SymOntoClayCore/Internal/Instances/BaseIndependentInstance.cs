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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseIndependentInstance : BaseInstance
    {
        protected BaseIndependentInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IStorageFactory storageFactory, List<VarInstance> varList)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, null, storageFactory, varList)
        {
            _idleActionsResolver = context.DataResolversFactory.GetIdleActionsResolver();
            _idleActionsRandom = new Random();
        }

        private readonly IdleActionsResolver _idleActionsResolver;
        private readonly Random _idleActionsRandom;

        /// <inheritdoc/>
        public override bool ActivateIdleAction(IMonitorLogger logger)
        {
            var idleActionsList = _idleActionsResolver.Resolve(logger, _localCodeExecutionContext);

            if (idleActionsList.IsNullOrEmpty())
            {
                return false;
            }

            if(idleActionsList.Count == 1)
            {
                ActivateIdleAction(logger, idleActionsList.First());

                return true;
            }

            var index = _idleActionsRandom.Next(0, idleActionsList.Count - 1);

            ActivateIdleAction(logger, idleActionsList[index]);

            return true;
        }

        private void ActivateIdleAction(IMonitorLogger logger, IdleActionItem idleActionItem)
        {
            logger.ActivateIdleAction("1833EDB9-9B58-4338-AFD2-795500BEB060", idleActionItem.ToLabel(logger));

#if DEBUG
            //logger.Info("DB893B55-F574-466A-BDE2-9326253534C2", $"idleActionItem = {idleActionItem.ToLabel(logger)}");
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Holder = Name;
            localCodeExecutionContext.Instance = this;

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = idleActionItem.GetCompiledFunctionBody(logger, _context, localCodeExecutionContext);
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = idleActionItem;
            processInitialInfo.Instance = this;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var taskValue = _context.CodeExecutor.ExecuteAsync(logger, processInitialInfo);

        }
    }
}
