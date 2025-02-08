using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public class RootTaskInstance : BaseCompoundTaskInstance
    {
        public RootTaskInstance(RootTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, context.StorageFactories.RootTaskInstanceStorageFactory)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.RootTaskInstance;
    }
}
