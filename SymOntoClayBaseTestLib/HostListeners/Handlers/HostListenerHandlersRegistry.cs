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
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners.Handlers
{
    public class HostListenerHandlersRegistry : IDisposable
    {
        public HostListenerHandlersRegistry(CancellationToken cancellationToken, ICustomThreadPool threadPool) 
        {
            _endPointHandlersRegistry = new HandlersRegistry(cancellationToken, threadPool);
            _methodImplHandlersRegistry = new HandlersRegistry(cancellationToken, threadPool);
        }

        public void SetLogger(IMonitorLogger logger)
        {
            _logger = logger;
            _endPointHandlersRegistry.SetLogger(logger);
            _methodImplHandlersRegistry.SetLogger(logger);
        }

        protected IMonitorLogger _logger;

        public void AddSyncHandler(string methodName, Action handler)
        {
            lock(_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _endPointHandlersRegistry.AddSyncHandler(methodName, handler);
                _methodImplHandlersRegistry.AddSyncHandler(methodName, handler);
            }
        }

        public void AddEndPointSyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _endPointHandlersRegistry.AddSyncHandler(methodName, handler);
            }
        }

        public void AddMethodImplSyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _methodImplHandlersRegistry.AddSyncHandler(methodName, handler);
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

                _endPointHandlersRegistry.AddAsyncHandler(methodName, handler);
                _methodImplHandlersRegistry.AddAsyncHandler(methodName, handler);
            }
        }

        public void AddEndPointAsyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _endPointHandlersRegistry.AddAsyncHandler(methodName, handler);
            }
        }

        public void AddMethodImplAsyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _methodImplHandlersRegistry.AddAsyncHandler(methodName, handler);
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

                _endPointHandlersRegistry.RemoveHandler(methodName, handler);
                _methodImplHandlersRegistry.RemoveHandler(methodName, handler);
            }
        }

        public void Emit(string endPoint, string methodName)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                _endPointHandlersRegistry.Emit(endPoint);
                _methodImplHandlersRegistry.Emit(methodName);
            }
        }

        private readonly HandlersRegistry _endPointHandlersRegistry;
        private readonly HandlersRegistry _methodImplHandlersRegistry;

        private readonly object _lockObj = new object();
        private bool _disposed = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                _endPointHandlersRegistry.Dispose();
                _methodImplHandlersRegistry.Dispose();
            }
        }
    }
}
