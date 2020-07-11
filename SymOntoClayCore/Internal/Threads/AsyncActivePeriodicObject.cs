using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Threads
{
    public class AsyncActivePeriodicObject : IActivePeriodicObject, IDisposable
    {
        public AsyncActivePeriodicObject(IActivePeriodicObjectContext context)
        {
            _context = context;

            context.AddChildActiveObject(this);
        }

        private readonly IActivePeriodicObjectContext _context;

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

                var task = new Task(() => {
                    while (true)
                    {
                        if (_context.IsNeedWating)
                        {
                            _isWaited = true;
                            _context.AutoResetEvent.WaitOne();
                            _isWaited = false;
                        }

                        if (_isExited)
                        {
                            return;
                        }

                        if (!PeriodicMethod())
                        {
                            return;
                        }
                    }
                });

                _taskValue = new TaskValue(task);

                task.Start();

                return _taskValue;
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
