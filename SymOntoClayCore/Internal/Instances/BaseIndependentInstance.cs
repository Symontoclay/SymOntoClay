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
        }

        private readonly IdleActionsResolver _idleActionsResolver;

        /// <inheritdoc/>
        public override bool ActivateIdleAction()
        {
#if DEBUG
            Log("Begin");
#endif

            var idleActionsList = _idleActionsResolver.Resolve(_localCodeExecutionContext);

#if DEBUG
            Log($"idleActionsList = {idleActionsList.WriteListToString()}");
#endif

            if(idleActionsList.IsNullOrEmpty())
            {
                return false;
            }

            if(idleActionsList.Count == 1)
            {
                ActivateIdleAction(idleActionsList.First());

                return true;
            }

            throw new NotImplementedException();
        }

        private void ActivateIdleAction(IdleActionItem idleActionItem)
        {
#if DEBUG
            Log($"idleActionItem = {idleActionItem}");
#endif

            throw new NotImplementedException();
        }
    }
}
