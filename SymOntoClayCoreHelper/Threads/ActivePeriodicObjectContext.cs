using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.CoreHelper.Threads
{
    public class ActivePeriodicObjectContext: IActivePeriodicObjectContext
    {
        public ActivePeriodicObjectContext(IActivePeriodicObjectCommonContext commonContext)
        {
            _commonContext = commonContext;
        }

        private readonly IActivePeriodicObjectCommonContext _commonContext;

        /// <inheritdoc/>
        public bool IsNeedWating => _commonContext.IsNeedWating;

        /// <inheritdoc/>
        public AutoResetEvent AutoResetEvent => _commonContext.AutoResetEvent;
    }
}
