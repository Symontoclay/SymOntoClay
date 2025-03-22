using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public class StrategicTaskInstance : BaseCompoundTaskInstance
    {
        public StrategicTaskInstance(StrategicHtnTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, context.StorageFactories.StrategicTaskInstanceStorageFactory)
        { 
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.StrategicTaskInstance;
    }
}
