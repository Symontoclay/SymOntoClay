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

        private Dictionary<StrongIdentifierValue, T> _tasksDict = new Dictionary<StrongIdentifierValue, T>();
    }
}
