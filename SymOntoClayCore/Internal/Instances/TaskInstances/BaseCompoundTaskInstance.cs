using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public abstract class BaseCompoundTaskInstance: BaseInstance
    {
        protected BaseCompoundTaskInstance(BaseCompoundHtnTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator, IStorageFactory storageFactory)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, storageFactory, null)
        {
            if(!codeItem.Backgrounds.IsNullOrEmpty())
            {
                throw new NotImplementedException("53FF3664-A55F-41CC-880F-804F41FEC037");
            }
        }

        /// <inheritdoc/>
        protected override void CreateConditionalTriggers(IMonitorLogger logger)
        {
            base.CreateConditionalTriggers(logger);


        }
    }
}
