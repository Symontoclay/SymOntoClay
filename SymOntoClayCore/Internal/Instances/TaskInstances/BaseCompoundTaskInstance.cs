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
                //throw new NotImplementedException("53FF3664-A55F-41CC-880F-804F41FEC037");
            }
        }

        /// <inheritdoc/>
        protected override void CreateConditionalTriggers(IMonitorLogger logger)
        {
            base.CreateConditionalTriggers(logger);

#if DEBUG
            Info("AF9E476B-3E6B-4BAF-ACCE-9A7F9523A6AB", $"BaseCompoundTaskInstance CreateConditionalTriggers");
#endif

            //CompoundHtnTaskBackgroundTriggerInstance
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
#if DEBUG
            Info("129AC325-98AB-40F8-A65D-D85084488B67", $"BaseCompoundTaskInstance OnDisposed");
#endif

            base.OnDisposed();
        }
    }
}
