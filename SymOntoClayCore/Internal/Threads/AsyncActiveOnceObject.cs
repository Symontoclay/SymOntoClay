using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class AsyncActiveOnceObject: IActiveOnceObject, IDisposable
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
                            return;
                        }

                        if (_context.IsNeedWating)
                        {
                            _isWaited = true;
                            autoResetEvent.WaitOne();
                            _isWaited = false;
                        }

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        OnceMethod(linkedCancellationTokenSource.Token);
                    }
                    catch(Exception e)
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

        private IThreadTask _task = null;

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

                        if (_context.IsNeedWating)
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
            }
        }
    }
}
