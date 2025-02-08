using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public abstract class BaseCompoundTaskInstance: BaseInstance
    {
        protected BaseCompoundTaskInstance(BaseCompoundTask codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator, IStorageFactory storageFactory)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, storageFactory, null)
        {
        }
    }
}
