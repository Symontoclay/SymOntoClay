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
        }

        private readonly IActivePeriodicObjectContext _context;

#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        private volatile bool _isWaited;

        public bool IsWaited => _isWaited;

        private volatile bool _isExited;

        /// <inheritdoc/>
        public void Start()
        {
            var task = new Task(() => {
#if DEBUG
                _logger.Info("Hi!");
#endif

                while(true)
                {
#if DEBUG
                    _logger.Info($"_context.IsNeedWating = {_context.IsNeedWating}");
#endif

                    if(_context.IsNeedWating)
                    {
#if DEBUG
                        _logger.Info("_context.IsNeedWating");
#endif

                        _isWaited = true;
                        _context.AutoResetEvent.WaitOne();
                        _isWaited = false;
                    }

                    if(_isExited)
                    {
#if DEBUG
                        _logger.Info("_isExited return;");
#endif

                        return;
                    }

                    if (!PeriodicMethod())
                    {
#if DEBUG
                        _logger.Info("!PeriodicMethod() return;");
#endif

                        return;
                    }

#if DEBUG
                    _logger.Info("Do!!!");
#endif
                }
            });

            task.Start();
        }

        public void Stop()
        {
            _isExited = true;
            _isWaited = false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
