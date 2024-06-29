/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners.Handlers
{
    public class HandlersRegistry: IDisposable
    {
        public HandlersRegistry(CancellationToken cancellationToken, ICustomThreadPool threadPool)
        {
            _cancellationToken = cancellationToken;
            _threadPool = threadPool;
        }

        protected readonly CancellationToken _cancellationToken;
        protected readonly ICustomThreadPool _threadPool;

        public void SetLogger(IMonitorLogger logger)
        {
            _logger = logger;
        }

        protected IMonitorLogger _logger;

        public void AddSyncHandler(string methodName, Action handler)
        {
            lock(_lockObj)
            {
                if(_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                AddHandler(PrepareMethodName(methodName), handler, _syncHandlersDict);
            }
        }

        public void AddAsyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                AddHandler(PrepareMethodName(methodName), handler, _asyncHandlersDict);
            }
        }

        public void RemoveHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                methodName = PrepareMethodName(methodName);

                TryRemoveHandler(methodName, handler, _syncHandlersDict);
                TryRemoveHandler(methodName, handler, _asyncHandlersDict);
            }
        }

        private void AddHandler(string methodName, Action handler, Dictionary<string, List<Action>> handlersDict)
        {
            if(handlersDict.ContainsKey(methodName))
            {
                var list = handlersDict[methodName];

                if(!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
            else
            {
                handlersDict[methodName] = new List<Action>() { handler };
            }
        }

        private void TryRemoveHandler(string methodName, Action handler, Dictionary<string, List<Action>> handlersDict)
        {
            if(handlersDict.ContainsKey(methodName))
            {
                handlersDict[methodName].Remove(handler);
            }
        }

        public void Emit(string methodName)
        {
            lock(_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                methodName = PrepareMethodName(methodName);

                if(_asyncHandlersDict.ContainsKey(methodName))
                {
                    var list = _asyncHandlersDict[methodName];

                    foreach (var handler in list)
                    {
                        ThreadTask.Run(() =>
                        {
                            try
                            {
                                handler.Invoke();
                            }
                            catch (Exception e)
                            {
                                _logger.Error("94BA1C64-FE5C-418B-AE5A-B1CAF10D06F1", e);
                            }
                        }, _threadPool, _cancellationToken);
                    }
                }

                if(_syncHandlersDict.ContainsKey(methodName))
                {
                    var list = _syncHandlersDict[methodName];

                    foreach(var handler in list)
                    {
                        handler.Invoke();
                    }
                }
            }
        }

        private string PrepareMethodName(string methodName)
        {
            return methodName.ToLower().Trim();
        }

        private readonly Dictionary<string, List<Action>> _syncHandlersDict = new Dictionary<string, List<Action>>();
        private readonly Dictionary<string, List<Action>> _asyncHandlersDict = new Dictionary<string, List<Action>>();

        private readonly object _lockObj = new object();
        private bool _disposed = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock(_lockObj)
            {
                if(_disposed)
                {
                    return;
                }

                _disposed = true;

                _syncHandlersDict.Clear();
                _asyncHandlersDict.Clear();
            }
        }
    }
}
