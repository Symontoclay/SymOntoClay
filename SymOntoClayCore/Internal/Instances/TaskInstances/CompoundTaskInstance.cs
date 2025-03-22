using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public class CompoundTaskInstance: BaseCompoundTaskInstance
    {
        public CompoundTaskInstance(CompoundHtnTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, context.StorageFactories.CompoundTaskInstanceStorageFactory)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.CompoundTaskInstance;
    }
}
