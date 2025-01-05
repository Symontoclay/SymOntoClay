using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class TasksStorage: BaseSpecificStorage, ITasksStorage
    {
        public TasksStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _rootTasksStorage = new CommonTasksStorage<RootTask>(realStorageContext.Logger);
            _strategicTasksStorage = new CommonTasksStorage<StrategicTask>(realStorageContext.Logger);
            _tacticalTasksStorage = new CommonTasksStorage<TacticalTask>(realStorageContext.Logger);
            _compoundCommonTasksStorage = new CommonTasksStorage<CompoundTask>(realStorageContext.Logger);
            _primitiveCommonTasksStorage = new CommonTasksStorage<PrimitiveTask>(realStorageContext.Logger);
        }

        /// <inheritdoc/>
        public BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
#if DEBUG
            //logger.Info("BC884227-12CE-4881-A3D3-EC1551F1C0E1", $"name = {name}");
            //logger.Info("7DB2A28C-1044-4C71-B637-0A01D348FF20", $"requestingKindOfTask = {requestingKindOfTask}");
#endif

            BaseCompoundTask task = null;

            if(requestingKindOfTask == KindOfTask.Root)
            {
                task = GetRootTask(logger, name);

                if (task != null)
                {
                    return task;
                }
            }

            switch(requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                    task = GetStrategicTask(logger, name);

                    if (task != null)
                    {
                        return task;
                    }
                    break;
            }

            switch (requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                case KindOfTask.Tactical:
                    task = GetTacticalTask(logger, name);

                    if (task != null)
                    {
                        return task;
                    }
                    break;
            }

            switch (requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                case KindOfTask.Tactical:
                case KindOfTask.Compound:
                    task = GetCompoundTaskByName(logger, name);

                    if (task != null)
                    {
                        return task;
                    }
                    break;
            }

            return null;
        }

        /// <inheritdoc/>
        public BaseTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
#if DEBUG
            //logger.Info("141DA781-3FB0-453F-BE69-1FEF36607336", $"name = {name}");
            //logger.Info("8B88B83A-0495-4278-B03E-E8B993406B88", $"requestingKindOfTask = {requestingKindOfTask}");
#endif

            BaseTask task = null;

            if (requestingKindOfTask == KindOfTask.Root)
            {
                task = GetRootTask(logger, name);

                if (task != null)
                {
                    return task;
                }
            }

            switch (requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                    task = GetStrategicTask(logger, name);

                    if (task != null)
                    {
                        return task;
                    }
                    break;
            }

            switch (requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                case KindOfTask.Tactical:
                    task = GetTacticalTask(logger, name);

                    if (task != null)
                    {
                        return task;
                    }
                    break;
            }

            switch (requestingKindOfTask)
            {
                case KindOfTask.Root:
                case KindOfTask.Strategic:
                case KindOfTask.Tactical:
                case KindOfTask.Compound:
                    task = GetCompoundTaskByName(logger, name);

                    if (task != null)
                    {
                        return task;
                    }

                    return GetPrimitiveTaskByName(logger, name);
            }

            return null;            
        }

        #region RootTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RootTask rootTask)
        {
            _rootTasksStorage.Append(logger, rootTask);
        }

        /// <inheritdoc/>
        public RootTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _rootTasksStorage.GetByName(logger, name);
        }

        /// <inheritdoc/>
        public IEnumerable<RootTask> GetAllRootTasks(IMonitorLogger logger)
        {
            return _rootTasksStorage.GetAllTasks(logger);
        }

        private CommonTasksStorage<RootTask> _rootTasksStorage;
        #endregion

        #region StrategicTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, StrategicTask strategicTask)
        {
            _strategicTasksStorage.Append(logger, strategicTask);
        }

        /// <inheritdoc/>
        public StrategicTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _strategicTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<StrategicTask> _strategicTasksStorage;
        #endregion

        #region TacticalTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, TacticalTask tacticalTask)
        {
            _tacticalTasksStorage.Append(logger, tacticalTask);
        }

        /// <inheritdoc/>
        public TacticalTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _tacticalTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<TacticalTask> _tacticalTasksStorage;
        #endregion

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
            _rootTasksStorage.Dispose();
            _strategicTasksStorage.Dispose();
            _tacticalTasksStorage.Dispose();
            _compoundCommonTasksStorage.Dispose();
            _primitiveCommonTasksStorage.Dispose();
        }
    }
}
