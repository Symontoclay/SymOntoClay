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

using SymOntoClay.ActiveObject.EventsCollections;
using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public class AsyncActivePeriodicObject : IActivePeriodicObject, IDisposable
    {
        public AsyncActivePeriodicObject(IActiveObjectContext context, ICustomThreadPool threadPool, IMonitorLogger logger)
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
        public PeriodicDelegate PeriodicMethod { get; set; }

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

                        while (true)
                        {
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

                            if (_isExited)
                            {
                                _onCompletedHandlersCollection.Emit();
                                return;
                            }

                            if (_cancellationToken.IsCancellationRequested)
                            {
                                _onCompletedHandlersCollection.Emit();
                                return;
                            }

                            if (!PeriodicMethod(linkedCancellationTokenSource.Token))
                            {
                                _onCompletedHandlersCollection.Emit();
                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error("7CA31B61-20CF-40E5-B275-E68213D00242", e);
                    }

                }, _threadPool, linkedCancellationTokenSource.Token);

                _task = task;

                task.Start();

                return _task;
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (_isExited)
                {
                    return;
                }

                _isExited = true;
                _isWaited = false;

                _task = null;
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

                _onCompletedHandlersCollection.Clear();

                _task = null;
            }
        }
    }
}
