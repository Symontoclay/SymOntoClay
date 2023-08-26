using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common
{
    public interface IMonitor : IMonitorLogger, IDisposable
    {
        bool Enable { get; set; }
        bool EnableRemoteConnection { get; set; }

        string SessionDirectoryName { get; }
        string SessionDirectoryFullName { get; }

        IMonitorNode CreateMotitorNode(string messagePointId, string nodeId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }
        string LogicalSearchExplainDumpDir { get; }
        bool EnableAddingRemovingFactLoggingInStorages { get; }
    }
}
