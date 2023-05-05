using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners.Handlers
{
    public class HostListenerHandlersRegistry : IDisposable
    {
        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
            _endPointHandlersRegistry.SetLogger(logger);
            _methodImplHandlersRegistry.SetLogger(logger);
        }

        protected IEntityLogger _logger;

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

        private readonly HandlersRegistry _endPointHandlersRegistry = new HandlersRegistry();
        private readonly HandlersRegistry _methodImplHandlersRegistry = new HandlersRegistry();

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
