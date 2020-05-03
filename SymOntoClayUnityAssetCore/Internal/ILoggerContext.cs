using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface ILoggerContext
    {
        /// <summary>
        /// Gets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        bool Enable { get; }
    }
}
