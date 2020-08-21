using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Threads
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

        private Value _taskValue = new NullValue();

        /// <inheritdoc/>
        public Value TaskValue => _taskValue;

        /// <inheritdoc/>
        public Value Start()
        {
            _isActive = true;

            while (true)
            {
                if (!PeriodicMethod())
                {
                    _isActive = false;
                    return _taskValue;
                }
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
