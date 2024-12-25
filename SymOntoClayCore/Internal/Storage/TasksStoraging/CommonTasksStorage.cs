using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class CommonTasksStorage<T>: BaseComponent
        where T: BaseTask
    {
        public CommonTasksStorage(IMonitorLogger logger)
            : base(logger)
        {
        }

        public void Append(IMonitorLogger logger, T task)
        {
            _tasksDict[task.Name] = task;
        }

        public T GetByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            if(_tasksDict.TryGetValue(name, out var task))
            {
                return task;
            }

            return null;
        }

        private Dictionary<StrongIdentifierValue, T> _tasksDict = new Dictionary<StrongIdentifierValue, T>();
    }
}
