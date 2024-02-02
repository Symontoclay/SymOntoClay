using SymOntoClay.Monitor;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.Monitoring
{
    public class TestMonitorNode : TestLogger, IMonitorNode
    {
        public TestMonitorNode(MonitorSettings monitorSettings, string nodeId)
            : base(monitorSettings, nodeId)
        {
        }

        /// <inheritdoc/>
        public bool Enable { get; set; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get; set; }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TestThreadLogger(_monitorSettings, threadId);
        }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentTheadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TestThreadLogger(_monitorSettings, threadId);
        }
    }
}
