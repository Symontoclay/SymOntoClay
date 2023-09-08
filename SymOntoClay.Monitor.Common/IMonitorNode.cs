using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common
{
    public interface IMonitorNode : IMonitorLogger
    {
        bool Enable { get; set; }
        bool EnableRemoteConnection { get; set; }

        IThreadLogger CreateThreadLogger(string messagePointId, string threadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentThreadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}
