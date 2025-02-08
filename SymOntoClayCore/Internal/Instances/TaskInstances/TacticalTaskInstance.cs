using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public class TacticalTaskInstance : BaseCompoundTaskInstance
    {
        public TacticalTaskInstance(TacticalTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, context.StorageFactories.TacticalTaskInstanceStorageFactory)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.TacticalTaskInstance;
    }
}
