/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
            _rootTasksStorage = new CommonTasksStorage<RootHtnTask>(realStorageContext.Logger);
            _strategicTasksStorage = new CommonTasksStorage<StrategicHtnTask>(realStorageContext.Logger);
            _tacticalTasksStorage = new CommonTasksStorage<TacticalHtnTask>(realStorageContext.Logger);
            _compoundCommonTasksStorage = new CommonTasksStorage<CompoundHtnTask>(realStorageContext.Logger);
            _primitiveCommonTasksStorage = new CommonTasksStorage<PrimitiveHtnTask>(realStorageContext.Logger);
        }

        /// <inheritdoc/>
        public BaseCompoundHtnTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
#if DEBUG
            //logger.Info("BC884227-12CE-4881-A3D3-EC1551F1C0E1", $"name = {name}");
            //logger.Info("7DB2A28C-1044-4C71-B637-0A01D348FF20", $"requestingKindOfTask = {requestingKindOfTask}");
#endif

            BaseCompoundHtnTask task = null;

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
        public BaseHtnTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
#if DEBUG
            //logger.Info("141DA781-3FB0-453F-BE69-1FEF36607336", $"name = {name}");
            //logger.Info("8B88B83A-0495-4278-B03E-E8B993406B88", $"requestingKindOfTask = {requestingKindOfTask}");
#endif

            BaseHtnTask task = null;

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
        public void Append(IMonitorLogger logger, RootHtnTask rootTask)
        {
            _rootTasksStorage.Append(logger, rootTask);
        }

        /// <inheritdoc/>
        public RootHtnTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _rootTasksStorage.GetByName(logger, name);
        }

        /// <inheritdoc/>
        public IEnumerable<RootHtnTask> GetAllRootTasks(IMonitorLogger logger)
        {
            return _rootTasksStorage.GetAllTasks(logger);
        }

        private CommonTasksStorage<RootHtnTask> _rootTasksStorage;
        #endregion

        #region StrategicTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, StrategicHtnTask strategicTask)
        {
            _strategicTasksStorage.Append(logger, strategicTask);
        }

        /// <inheritdoc/>
        public StrategicHtnTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _strategicTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<StrategicHtnTask> _strategicTasksStorage;
        #endregion

        #region TacticalTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, TacticalHtnTask tacticalTask)
        {
            _tacticalTasksStorage.Append(logger, tacticalTask);
        }

        /// <inheritdoc/>
        public TacticalHtnTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _tacticalTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<TacticalHtnTask> _tacticalTasksStorage;
        #endregion

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundHtnTask compoundTask)
        {
            _compoundCommonTasksStorage.Append(logger, compoundTask);
        }

        /// <inheritdoc/>
        public CompoundHtnTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _compoundCommonTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<CompoundHtnTask> _compoundCommonTasksStorage;
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveHtnTask primitiveTask)
        {
            _primitiveCommonTasksStorage.Append(logger, primitiveTask);
        }

        /// <inheritdoc/>
        public PrimitiveHtnTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _primitiveCommonTasksStorage.GetByName(logger, name);
        }

        private CommonTasksStorage<PrimitiveHtnTask> _primitiveCommonTasksStorage;
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
