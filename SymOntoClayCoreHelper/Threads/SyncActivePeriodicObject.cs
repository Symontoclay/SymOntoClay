using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.Threads
{
    public class SyncActivePeriodicObject : IActivePeriodicObject
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        private volatile bool _isActive;

        /// <inheritdoc/>
        public bool IsActive => _isActive;

        /// <inheritdoc/>
        public bool IsWaited => false;

        /// <inheritdoc/>
        public void Start()
        {
            _isActive = true;

            while (true)
            {
                if(!PeriodicMethod())
                {
                    _isActive = false;
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
        }
    }
}
