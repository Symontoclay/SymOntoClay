using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CoreHelper.Threads
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

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        private volatile bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        private volatile bool _isExited = true;

        /// <inheritdoc/>
        public bool IsActive => !_isExited && !_isWaited;

        /// <inheritdoc/>
        public void Start()
        {
            if (_isDisposed)
            {
                return;
            }

            if (!_isExited)
            {
                return;
            }

            _isExited = false;
            _isWaited = false;

            var task = new Task(() => {
                while (true)
                {
                    if(_context.IsNeedWating)
                    {
                        _isWaited = true;
                        _context.AutoResetEvent.WaitOne();
                        _isWaited = false;
                    }

                    if(_isExited)
                    {
                        return;
                    }

                    if (!PeriodicMethod())
                    {
                        return;
                    }
                }
            });

            task.Start();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            if (_isDisposed)
            {
                return;
            }

            if(_isExited)
            {
                return;
            }

            _isExited = true;
            _isWaited = false;
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _isExited = true;

            _context.RemoveChildActiveObject(this);
        }
    }
}
