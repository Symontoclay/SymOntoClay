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

        /// <inheritdoc/>
        public BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return GetCompoundTaskByName(logger, name);
        }

        /// <inheritdoc/>
        public BaseTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            var task = GetCompoundTaskByName(logger, name);

            if(task != null)
            {
                return task;
            }

            return GetPrimitiveTaskByName(logger, name);
        }

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundTask compoundTask)
        {
            _compoundCommonTasksStorage.Append(logger, compoundTask);
        }

        /// <inheritdoc/>
        public CompoundTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _compoundCommonTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<CompoundTask> _compoundCommonTasksStorage;
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveTask primitiveTask)
        {
            _primitiveCommonTasksStorage.Append(logger, primitiveTask);
        }

        /// <inheritdoc/>
        public PrimitiveTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _primitiveCommonTasksStorage.GetByName(logger, name);
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
