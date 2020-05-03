using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    /// <inheritdoc cref='ILoggerContext'/>
    public class LoggerContext : ILoggerContext
    {
        /// <inheritdoc/>
        public bool Enable { get; set; }
    }
}
