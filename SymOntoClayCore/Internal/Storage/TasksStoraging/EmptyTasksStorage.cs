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

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundTask compoundTask)
        {
        }
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveTask primitiveTask)
        {
        }
        #endregion
    }
}
