using SymOntoClay.ActiveObject.EventsCollections;
using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.Pointers;
using SymOntoClay.Common.Cancellation;
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
            _cancellationContext = context.CancellationContext;
            _logger = logger;

            context.AddChildActiveObject(this);
        }

        private readonly IActiveObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly ICancellationContext _cancellationContext;
        private readonly IMonitorLogger _logger;

        private readonly object _lockObj = new object();

        public IObjectWithOnceRunMethod ObjectWithOnceRunMethod { get; set; }

        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        private IThreadTaskPointer _task = new ThreadTaskPointer();

        /// <inheritdoc/>
        public IThreadTaskPointer TaskValue
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
        public IThreadTaskPointer Start()
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

                var task = new ThreadTask(() => {
                    try
                    {
                        var autoResetEvent = _context.WaitEvent;

                        if (_cancellationContext.IsCancellationRequested)
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

                        if (_cancellationContext.IsCancellationRequested)
                        {
                            _onCompletedHandlersCollection.Emit();
                            return;
                        }

                        ObjectWithOnceRunMethod.OnceRunHandler(_cancellationContext);

                        _onCompletedHandlersCollection.Emit();
                    }
                    catch (Exception e)
                    {
                        _logger.Error("787E7D4C-3164-47D0-8CA7-A5496300B1E9", e);
                    }

                    _isExited = true;
                }, _threadPool, _cancellationContext);

                _task.Task = task;

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

                _task.Task = null;

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
            _cancellationContext = context.CancellationContext;
            _logger = logger;

            context.AddChildActiveObject(this);
        }

        private readonly IActiveObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly ICancellationContext _cancellationContext;
        private readonly IMonitorLogger _logger;

        private readonly object _lockObj = new object();

        public IObjectWithOnceRunMethod<TResult> ObjectWithOnceRunMethod { get; set; }

        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        private IThreadTaskPointer<TResult> _task = new ThreadTaskPointer<TResult>();

        public IThreadTaskPointer<TResult> TaskValueWithResult
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
        public IThreadTaskPointer TaskValue
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
                    return _task.TaskWithResult.Result;
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
        public IThreadTaskPointer Start()
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

                var task = new ThreadTask<TResult>(() => {
                    try
                    {
                        var autoResetEvent = _context.WaitEvent;

                        if (_cancellationContext.IsCancellationRequested)
                        {
                            _onCompletedHandlersCollection.Emit();
                            return default;
                        }

                        if (_context.IsNeedWaiting)
                        {
                            _isWaited = true;
                            autoResetEvent.WaitOne();
                            _isWaited = false;
                        }

                        if (_cancellationContext.IsCancellationRequested)
                        {
                            _onCompletedHandlersCollection.Emit();
                            return default;
                        }

                        _isExited = true;

                        var result = ObjectWithOnceRunMethod.OnceRunHandler(_cancellationContext);

                        _onCompletedHandlersCollection.Emit();

                        return result;
                    }
                    catch (Exception e)
                    {
                        _logger.Error("787E7D4C-3164-47D0-8CA7-A5496300B1E9", e);
                        _isExited = true;
                        return default;
                    }
                }, _threadPool, _cancellationContext);

                _task.Task = task;

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

                _task.Task = null;

                _onCompletedHandlersCollection.Clear();
            }
        }
    }
}
