using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Threading;
using System.Collections.Generic;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorStub: IThreadExecutor
    {
        public ThreadTaskStatus RunningStatus { get; set; } = ThreadTaskStatus.Created;

        public void Cancel()
        {

        }

        public void Wait()
        {

        }

        private object _onCompletedHandlersLockObj = new object();
        private List<IOnCompletedThreadExecutorHandler> _onCompletedHandlers = new List<IOnCompletedThreadExecutorHandler>();

        public void AddOnCompletedHandler(IOnCompletedThreadExecutorHandler handler)
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

        public void RemoveOnCompletedHandler(IOnCompletedThreadExecutorHandler handler)
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
                    handler.Invoke();
                }
            }
        }
    }
}
