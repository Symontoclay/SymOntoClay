using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseIndependentInstance : BaseInstance
    {
        protected BaseIndependentInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, IStorageFactory storageFactory, List<Var> varList)
            : base(codeItem, context, parentStorage, new StateStorageFactory(), varList)
        {
            _idleActionsResolver = context.DataResolversFactory.GetIdleActionsResolver();
            _idleActionsRandom = new Random();
        }

        private readonly IdleActionsResolver _idleActionsResolver;
        private readonly Random _idleActionsRandom;

        /// <inheritdoc/>
        public override bool ActivateIdleAction()
        {
#if DEBUG
            Log("Begin");
#endif

            var idleActionsList = _idleActionsResolver.Resolve(_localCodeExecutionContext);

#if DEBUG
            Log($"idleActionsList.Count = {idleActionsList.Count}");
            //Log($"idleActionsList = {idleActionsList.WriteListToString()}");
#endif

            if (idleActionsList.IsNullOrEmpty())
            {
                return false;
            }

            if(idleActionsList.Count == 1)
            {
                ActivateIdleAction(idleActionsList.First());

                return true;
            }

            var index = _idleActionsRandom.Next(0, idleActionsList.Count - 1);

#if DEBUG
            Log($"index = {index}");
#endif

            ActivateIdleAction(idleActionsList[index]);

            return true;
        }

        private void ActivateIdleAction(IdleActionItem idleActionItem)
        {
#if DEBUG
            //Log($"idleActionItem = {idleActionItem}");
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext();

            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Holder = Name;

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = idleActionItem.CompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = idleActionItem;
            processInitialInfo.Instance = this;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var taskValue = _context.CodeExecutor.ExecuteAsync(processInitialInfo);

#if DEBUG
            //Log($"taskValue = {taskValue}");
#endif
        }
    }
}
