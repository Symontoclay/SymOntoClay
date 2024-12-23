using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class TasksStorage: BaseSpecificStorage, ITasksStorage
    {
        public TasksStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _compoundCommonTasksStorage = new CommonTasksStorage<CompoundTask>(realStorageContext.Logger);
            _primitiveCommonTasksStorage = new CommonTasksStorage<PrimitiveTask>(realStorageContext.Logger);
        }

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundTask compoundTask)
        {
            _compoundCommonTasksStorage.Append(logger, compoundTask);
        }

        private CommonTasksStorage<CompoundTask> _compoundCommonTasksStorage;
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveTask primitiveTask)
        {
            _primitiveCommonTasksStorage.Append(logger, primitiveTask);
        }

        private CommonTasksStorage<PrimitiveTask> _primitiveCommonTasksStorage;
        #endregion

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _compoundCommonTasksStorage.Dispose();
            _primitiveCommonTasksStorage.Dispose();
        }
    }
}
