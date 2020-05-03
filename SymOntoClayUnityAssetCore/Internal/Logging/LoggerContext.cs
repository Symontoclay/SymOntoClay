using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    /// <inheritdoc cref='ILoggerContext'/>
    public class LoggerContext : ILoggerContext
    {
        private readonly object _lockObj = new object();
        private bool _enable;

        /// <inheritdoc/>
        public bool Enable
        {
            get
            {
                lock(_lockObj)
                {
                    return _enable;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _enable = value;
                }
            }
        }
    }
}
