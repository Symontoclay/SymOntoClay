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

        /// <inheritdoc/>
        public void Start()
        {
#if DEBUG
            _logger.Info("Begin");
#endif
            
            while(true)
            {
                if(!PeriodicMethod())
                {
#if DEBUG
                    _logger.Info("!PeriodicMethod() return;");
#endif

                    return;
                }
            }
        }
    }
}
