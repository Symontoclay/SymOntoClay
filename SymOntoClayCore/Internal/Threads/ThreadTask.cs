using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ThreadTask
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static ThreadTask Run(Action action)
        {
            var task = new ThreadTask(action);
            task.Start();
            return task;
        }

        public static ThreadTask Run(Action action, CancellationToken cancellationToken)
        {
            var task = new ThreadTask(action, cancellationToken);
            task.Start();
            return task;
        }

        public ThreadTask(Action action, CancellationToken cancellationToken)
        {
            _action = action;
        }

        public ThreadTask(Action action)
        {
            _action = action;
        }

        public ThreadTaskStatus Status
        {
            get
            {
                lock(_lockObj)
                {
                    return _status;
                }
            }
        }

        public bool IsCanceled
        {
            get
            {
                lock (_lockObj)
                {
                    return _status == ThreadTaskStatus.Canceled;
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                lock (_lockObj)
                {
                    return _status == ThreadTaskStatus.RanToCompletion || _status == ThreadTaskStatus.Faulted || _status == ThreadTaskStatus.Canceled;
                }
            }
        }

        public bool IsCompletedSuccessfully
        {
            get
            {
                lock (_lockObj)
                {
                    return _status == ThreadTaskStatus.RanToCompletion;
                }
            }
        }

        public bool IsFaulted
        {
            get
            {
                lock (_lockObj)
                {
                    return _status == ThreadTaskStatus.Faulted;
                }
            }
        }

        public void Start()
        {
            lock(_lockObj)
            {
                if(_status == ThreadTaskStatus.Running)
                {
                    return;
                }

                _status = ThreadTaskStatus.Running;
            }

            if(_thread == null)
            {
                var threadDelegate = new ThreadStart(RunDelegate);
                _thread = new Thread(threadDelegate);
                _thread.IsBackground = true;
            }

            _thread.Start();
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public event Action OnStarted;
        public event Action OnCanceled;
        public event Action OnCompleted;
        public event Action OnCompletedSuccessfully;
        public event Action OnFaulted;

        private Action _action;
        private Thread _thread;
        private object _lockObj = new object();
        private ThreadTaskStatus _status = ThreadTaskStatus.Created;

        private void RunDelegate()
        {
            try
            {
                OnStarted?.Invoke();

                _action?.Invoke();

                lock(_lockObj)
                {
                    _status = ThreadTaskStatus.RanToCompletion;
                }

                OnCompletedSuccessfully?.Invoke();
            }
            catch (OperationCanceledException)
            {
                lock (_lockObj)
                {
                    _status = ThreadTaskStatus.Canceled;
                }

                OnCanceled?.Invoke();
            }
            catch (Exception)
            {
                lock (_lockObj)
                {
                    _status = ThreadTaskStatus.Faulted;
                }

                OnFaulted?.Invoke();
            }

            OnCompleted?.Invoke();
        }
    }
}
