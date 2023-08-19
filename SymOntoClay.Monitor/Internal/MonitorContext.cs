using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorContext : IObjectToString
    {
        public bool Enable { get; set; }
        public bool EnableRemoteConnection { get; set; }
        public MonitorFeatures Features { get; set; }
        public MonitorFileCache FileCache { get; set; }
        public MessageProcessor MessageProcessor { get; set; }
        public MessageNumberGenerator GlobalMessageNumberGenerator { get; set; } = new MessageNumberGenerator();
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
            sb.AppendLine($"{spaces}{nameof(Enable)} = {Enable}");
            sb.AppendLine($"{spaces}{nameof(EnableRemoteConnection)} = {EnableRemoteConnection}");
            sb.PrintObjProp(n, nameof(Features), Features);
            sb.PrintExisting(n, nameof(FileCache), FileCache);
            sb.PrintExisting(n, nameof(MessageProcessor), MessageProcessor);
            sb.PrintExisting(n, nameof(OutputHandler), OutputHandler);
            sb.PrintExisting(n, nameof(ErrorHandler), ErrorHandler);
            return sb.ToString();
        }
    }
}
