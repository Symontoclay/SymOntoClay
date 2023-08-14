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
        public MonitorFeatures Features { get; set; }
        public MonitorFileCache FileCache { get; set; }
        public MessageProcessor MessageProcessor { get; set; }
        public MessageNumberGenerator GlobalMessageNumberGenerator { get; set; } = new MessageNumberGenerator();

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
            sb.PrintObjProp(n, nameof(Features), Features);
            sb.PrintExisting(n, nameof(FileCache), FileCache);
            sb.PrintExisting(n, nameof(MessageProcessor), MessageProcessor);
            return sb.ToString();
        }
    }
}
