using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor
{
    public class MonitorSettings : IObjectToString
    {
        public string MessagesDir { get; set; }
        public IRemoteMonitor RemoteMonitor { get; set; }
        public Action<string> OutputHandler { get; set; }
        public Action<string> ErrorHandler { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(MessagesDir)} = {MessagesDir}");
            sb.PrintExisting(n, nameof(RemoteMonitor), RemoteMonitor);
            sb.PrintExisting(n, nameof(OutputHandler), OutputHandler);
            sb.PrintExisting(n, nameof(ErrorHandler), ErrorHandler);
            return sb.ToString();
        }
    }
}
