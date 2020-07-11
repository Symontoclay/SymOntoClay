using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ActivePeriodicObjectCommonContext : IActivePeriodicObjectCommonContext
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        private volatile bool _isNeedWating;

        /// <inheritdoc/>
        public bool IsNeedWating => _isNeedWating;

        /// <inheritdoc/>
        public AutoResetEvent AutoResetEvent => _autoResetEvent;

        public void Lock()
        {
            _autoResetEvent.Reset();
            _isNeedWating = true;
        }

        public void UnLock()
        {
            _isNeedWating = false;
            _autoResetEvent.Set();
        }
    }
}
