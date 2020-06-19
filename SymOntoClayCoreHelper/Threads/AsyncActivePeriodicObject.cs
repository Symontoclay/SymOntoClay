using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.Threads
{
    public class AsyncActivePeriodicObject : IActivePeriodicObject, IDisposable
    {
        public AsyncActivePeriodicObject(IActivePeriodicObjectContext context)
        {
        }

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

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
