using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal.FileCache;
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
        IMonitorFeatures Features { get; }
        IList<IPlatformLogger> PlatformLoggers { get; }
        IFileCache FileCache { get; }
        MessageNumberGenerator GlobalMessageNumberGenerator { get; }
        MessageNumberGenerator MessageNumberGenerator { get; }
        string NodeId { get; }
        string ThreadId { get; }
        bool EnableRemoteConnection { get; }
        bool EnableFullCallInfo { get; }
    }
}
