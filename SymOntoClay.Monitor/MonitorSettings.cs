using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor
{
    public class MonitorSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool Enable { get; set; }
        public string MessagesDir { get; set; }
        public IRemoteMonitor RemoteMonitor { get; set; }
        public Action<string> OutputHandler { get; set; }
        public Action<string> ErrorHandler { get; set; }
        /// <summary>
        /// Gets or sets list of platform specific loggers.
        /// It alows us to add, for example, console logger for Unity3D.
        /// </summary>
        public IList<IPlatformLogger> PlatformLoggers { get; set; }

        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; set; } = KindOfLogicalSearchExplain.None;
        public string LogicalSearchExplainDumpDir { get; set; }
        public bool EnableAddingRemovingFactLoggingInStorages { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Enable)} = {Enable}");
            sb.AppendLine($"{spaces}{nameof(MessagesDir)} = {MessagesDir}");
            sb.PrintExisting(n, nameof(RemoteMonitor), RemoteMonitor);
            sb.PrintExisting(n, nameof(OutputHandler), OutputHandler);
            sb.PrintExisting(n, nameof(ErrorHandler), ErrorHandler);
            sb.PrintExistingList(n, nameof(PlatformLoggers), PlatformLoggers);
            sb.AppendLine($"{spaces}{nameof(KindOfLogicalSearchExplain)} = {KindOfLogicalSearchExplain}");
            sb.AppendLine($"{spaces}{nameof(LogicalSearchExplainDumpDir)} = {LogicalSearchExplainDumpDir}");
            sb.AppendLine($"{spaces}{nameof(EnableAddingRemovingFactLoggingInStorages)} = {EnableAddingRemovingFactLoggingInStorages}");
            return sb.ToString();
        }
    }
}
