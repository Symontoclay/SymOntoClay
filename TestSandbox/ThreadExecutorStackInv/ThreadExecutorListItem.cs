using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using System;
using System.Collections.Generic;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorListItem: IOnCompletedThreadExecutorHandler
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        public ThreadExecutorListItem(IThreadExecutor threadExecutor) 
        {
            _threadExecutor = threadExecutor;
            _threadExecutor.AddOnCompletedHandler(this);
        }

        private IThreadExecutor _threadExecutor;

        public IThreadExecutor Executor => _threadExecutor;

        void IOnCompletedThreadExecutorHandler.Invoke()
        {
            _threadExecutor = null;

            EmitOnCompletedHandlers();
        }

        private object _onCompletedHandlersLockObj = new object();
        private List<IOnCompletedThreadExecutorListItemHandler> _onCompletedHandlers = new List<IOnCompletedThreadExecutorListItemHandler>();

        public void AddOnCompletedHandler(IOnCompletedThreadExecutorListItemHandler handler)
        {
            lock (_onCompletedHandlersLockObj)
            {
                if (_onCompletedHandlers.Contains(handler))
                {
                    return;
                }

                _onCompletedHandlers.Add(handler);
            }
        }

        public void RemoveOnCompletedHandler(IOnCompletedThreadExecutorListItemHandler handler)
        {
            lock (_onCompletedHandlersLockObj)
            {
                if (_onCompletedHandlers.Contains(handler))
                {
                    _onCompletedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnCompletedHandlers()
        {
            lock (_onCompletedHandlersLockObj)
            {
                foreach (var handler in _onCompletedHandlers.ToArray())
                {
                    handler.Invoke(this);
                }
            }
        }
    }
}
