using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal
{
    public interface IMonitorLoggerContext
    {
        Action<string> OutputHandler { get; }
        Action<string> ErrorHandler { get; }
        MessageProcessor MessageProcessor { get; }
    }
}
