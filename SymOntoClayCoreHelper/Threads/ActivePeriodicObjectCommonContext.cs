using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.CoreHelper.Threads
{
    public class ActivePeriodicObjectCommonContext: IActivePeriodicObjectCommonContext
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        private volatile bool _isNeedWating;

        /// <inheritdoc/>
        public bool IsNeedWating => _isNeedWating;

        /// <inheritdoc/>
        public AutoResetEvent AutoResetEvent => _autoResetEvent;
    }
}
