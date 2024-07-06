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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Threads
{
    public class AsyncActivePeriodicObject : IActivePeriodicObject, IDisposable
    {
        public AsyncActivePeriodicObject(IActivePeriodicObjectContext context, ICustomThreadPool threadPool)
        {
            _context = context;
            _threadPool = threadPool;
            _cancellationToken = context.Token;

            context.AddChildActiveObject(this);
        }
        
        private readonly IActivePeriodicObjectContext _context;
        private readonly ICustomThreadPool _threadPool;
        private readonly CancellationToken _cancellationToken;

#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }
        
        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        private Value _taskValue = new NullValue();

        /// <inheritdoc/>
        public Value TaskValue
        {
            get
            {
                lock(_lockObj)
                {
                    return _taskValue;
                }
            }
        }

        /// <inheritdoc/>
        public Value Start()
        {
            lock(_lockObj)
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
                var cancellationToken = cancellationTokenSource.Token;

                var task = new ThreadTask(() => {
                    var autoResetEvent = _context.WaitEvent;

                    while (true)
                    {
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

                        if (_isExited)
                        {
                            return;
                        }

                        if(_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (!PeriodicMethod(cancellationToken))
                        {
                            return;
                        }
                    }
                }, _threadPool, cancellationToken);

                _taskValue = new TaskValue(task, cancellationTokenSource);

                task.Start();

                return _taskValue;
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
#if DEBUG
            _logger.Info("Stop()");
#endif

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

                _taskValue = new NullValue();
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

                _taskValue = new NullValue();
            }
        }
    }
}
