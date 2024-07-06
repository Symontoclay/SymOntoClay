using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class AsyncActiveOnceObject: IDisposable
    {
        public AsyncActiveOnceObject(IActiveObjectContext context, ICustomThreadPool threadPool, IMonitorLogger logger)
        {
            _context = context;
            _threadPool = threadPool;
            _cancellationToken = context.Token;
            _logger = logger;
        }

        private readonly IActiveObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly CancellationToken _cancellationToken;
        private readonly IMonitorLogger _logger;

        private readonly object _lockObj = new object();

        public OnceDelegate OnceMethod { get; set; }

        private volatile bool _isWaited;

        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        public bool IsActive => !_isExited && !_isWaited;

        private Value _taskValue = new NullValue();

        public Value TaskValue
        {
            get
            {
                lock (_lockObj)
                {
                    return _taskValue;
                }
            }
        }

        public Value Start()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return _taskValue;
                }

                if (!_isExited)
                {
                    return _taskValue;
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

                _taskValue = new TaskValue(task, linkedCancellationTokenSource);

                task.Start();

                return _taskValue;
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

                //_context.RemoveChildActiveObject(this);

                _taskValue = new NullValue();
            }
        }
    }
}
