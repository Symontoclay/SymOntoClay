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
    public class EmptyMonitor: EmptyLogger, IMonitor
    {
        public EmptyMonitor(MonitorSettings monitorSettings)
            : base(Guid.NewGuid().ToString("D"))
        {
        }

        /// <inheritdoc/>
        public bool Enable { get; set; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get; set; }

        /// <inheritdoc/>
        public string SessionDirectoryName => string.Empty;

        /// <inheritdoc/>
        public string SessionDirectoryFullName => string.Empty;

        /// <inheritdoc/>
        public IMonitorNode CreateMotitorNode(string messagePointId, string nodeId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EmptyMonitorNode(nodeId);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
