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
