using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.Threads
{
    public class AsyncActivePeriodicObject : IActivePeriodicObject
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        /// <inheritdoc/>
        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
