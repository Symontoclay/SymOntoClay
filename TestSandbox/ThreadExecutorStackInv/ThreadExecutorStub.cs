using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Threading;
using System.Collections.Generic;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorStub: IThreadExecutor
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        public ThreadTaskStatus RunningStatus { get; set; } = ThreadTaskStatus.Created;

        public void Cancel()
        {

        }

        public void Wait()
        {

        }

        public void Run()
        {
            for (int i = 0; i < _maxIterations; i++)
            {
                _globalLogger.Info($"Run Iteration {i}");
            }

            NComplete();
        }

        private void NComplete()
        {
            _globalLogger.Info("NComplete");

            RunningStatus = ThreadTaskStatus.RanToCompletion;
            EmitOnCompletedHandlers();
        }

        private int _maxIterations = 10;

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
