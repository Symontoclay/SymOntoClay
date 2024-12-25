using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class EmptyTasksStorage : BaseEmptySpecificStorage, ITasksStorage
    {
        public EmptyTasksStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundTask compoundTask)
        {
        }

        /// <inheritdoc/>
        public CompoundTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveTask primitiveTask)
        {
        }

        /// <inheritdoc/>
        public PrimitiveTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion
    }
}
