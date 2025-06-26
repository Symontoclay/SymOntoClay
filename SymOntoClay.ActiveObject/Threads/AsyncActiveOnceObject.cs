using SymOntoClay.ActiveObject.EventsCollections;
using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public class AsyncActiveOnceObject : IActiveOnceObject, IDisposable
    {
        public AsyncActiveOnceObject(IActiveObjectContext context, ICustomThreadPool threadPool, IMonitorLogger logger)
        {
            _context = context;
            _threadPool = threadPool;
            _cancellationToken = context.Token;
            _logger = logger;

            context.AddChildActiveObject(this);
        }

        private readonly IActiveObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly CancellationToken _cancellationToken;
        private readonly IMonitorLogger _logger;

        private readonly object _lockObj = new object();

        public OnceDelegate OnceMethod { get; set; }

        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        private ThreadTask _task = null;

        /// <inheritdoc/>
        public IThreadTask TaskValue
        {
            get
            {
                lock (_lockObj)
                {
                    return _task;
                }
            }
        }

        /// <inheritdoc/>
        public void AddOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
        {
            _onCompletedHandlersCollection.AddHandler(handler);
        }

        /// <inheritdoc/>
        public void RemoveOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
        {
            _onCompletedHandlersCollection.RemoveHandler(handler);
        }

        private OnCompletedActiveObjectHandlersCollection _onCompletedHandlersCollection = new OnCompletedActiveObjectHandlersCollection();

        /// <inheritdoc/>
        public IThreadTask Start()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return _task;
                }

                if (!_isExited)
                {
                    return _task;
                }

                _isExited = false;
                _isWaited = false;

                var cancellationTokenSource = new CancellationTokenSource();
                var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, _cancellationToken);

                var task = new ThreadTask(() => {
                    try
                    {
                        var autoResetEvent = _context.WaitEvent;

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            _onCompletedHandlersCollection.Emit();
                            return;
                        }

                        if (_context.IsNeedWaiting)
                        {
                            _isWaited = true;
                            autoResetEvent.WaitOne();
                            _isWaited = false;
                        }

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            _onCompletedHandlersCollection.Emit();
                            return;
                        }

                        OnceMethod(linkedCancellationTokenSource.Token);

                        _onCompletedHandlersCollection.Emit();
                    }
                    catch (Exception e)
                    {
                        _logger.Error("787E7D4C-3164-47D0-8CA7-A5496300B1E9", e);
                    }

                    _isExited = true;
                }, _threadPool, _cancellationToken);

                _task = task;

                task.Start();

                return _task;
            }
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                _isExited = true;

                _context.RemoveChildActiveObject(this);

                _task = null;

                _onCompletedHandlersCollection.Clear();
            }
        }
    }

    public class AsyncActiveOnceObject<TResult> : IActiveOnceObject, IDisposable
    {
        public AsyncActiveOnceObject(IActiveObjectContext context, ICustomThreadPool threadPool, IMonitorLogger logger)
        {
            _context = context;
            _threadPool = threadPool;
            _cancellationToken = context.Token;
            _logger = logger;

            context.AddChildActiveObject(this);
        }

        private readonly IActiveObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly CancellationToken _cancellationToken;
        private readonly IMonitorLogger _logger;

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public OnceDelegateWithResult<TResult> OnceMethod { get; set; }

        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        private ThreadTask<TResult> _task = null;

        public IThreadTask<TResult> TaskValueWithResult
        {
            get
            {
                lock (_lockObj)
                {
                    return _task;
                }
            }
        }

        /// <inheritdoc/>
        public IThreadTask TaskValue
        {
            get
            {
                lock (_lockObj)
                {
                    return _task;
                }
            }
        }

        public TResult Result
        {
            get
            {
                lock (_lockObj)
                {
                    return _task.Result;
                }
            }
        }

        public void AddOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
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

        public void RemoveOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
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
                foreach (var handler in _onCompletedHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onCompletedHandlersLockObj = new object();
        private List<IOnCompletedActiveObjectHandler> _onCompletedHandlers = new List<IOnCompletedActiveObjectHandler>();

        /// <inheritdoc/>
        public IThreadTask Start()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return _task;
                }

                if (!_isExited)
                {
                    return _task;
                }

                _isExited = false;
                _isWaited = false;

                var cancellationTokenSource = new CancellationTokenSource();
                var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, _cancellationToken);

                var task = new ThreadTask<TResult>(() => {
                    try
                    {
                        var autoResetEvent = _context.WaitEvent;

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return default;
                        }

                        if (_context.IsNeedWaiting)
                        {
                            _isWaited = true;
                            autoResetEvent.WaitOne();
                            _isWaited = false;
                        }

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return default;
                        }

                        _isExited = true;

                        return OnceMethod(linkedCancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("787E7D4C-3164-47D0-8CA7-A5496300B1E9", e);
                        _isExited = true;
                        return default;
                    }
                }, _threadPool, _cancellationToken);

                task.OnCompleted += EmitOnCompletedHandlers;//no need

                _task = task;

                task.Start();

                return _task;
            }
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                _isExited = true;

                _context.RemoveChildActiveObject(this);

                _task.OnCompleted -= EmitOnCompletedHandlers;
                _task = null;

                _onCompletedHandlers.Clear();
            }
        }
    }
}
