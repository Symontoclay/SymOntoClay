using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class CommonTasksStorage<T>: BaseComponent
        where T: BaseHtnTask
    {
        public CommonTasksStorage(IMonitorLogger logger)
            : base(logger)
        {
        }

        public void Append(IMonitorLogger logger, T task)
        {
            _tasksDict[task.Name] = task;
            _tasksList = _tasksDict.Values.ToArray();
        }

        public T GetByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            if(_tasksDict.TryGetValue(name, out var task))
            {
                return task;
            }

            return null;
        }

        public IEnumerable<T> GetAllTasks(IMonitorLogger logger)
        {
            return _tasksList;
        }

        private T[] _tasksList = new T[0];
        private Dictionary<StrongIdentifierValue, T> _tasksDict = new Dictionary<StrongIdentifierValue, T>();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _tasksDict.Clear();
            _tasksList = new T[0];

            base.OnDisposed();
        }
    }
}
