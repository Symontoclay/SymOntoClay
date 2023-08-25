using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorFeatures : IMonitorFeatures, IObjectToString
    {
        /// <inheritdoc/>
        public bool EnableCallMethod { get; set; }

        /// <inheritdoc/>
        public bool EnableParameter { get; set; }

        /// <inheritdoc/>
        public bool EnableOutput { get; set; }

        /// <inheritdoc/>
        public bool EnableTrace { get; set; }

        /// <inheritdoc/>
        public bool EnableDebug { get; set; }

        /// <inheritdoc/>
        public bool EnableInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableWarn { get; set; }

        /// <inheritdoc/>
        public bool EnableError { get; set; }

        /// <inheritdoc/>
        public bool EnableFatal { get; set; }

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
            sb.AppendLine($"{spaces}{EnableCallMethod} = {EnableCallMethod}");
            sb.AppendLine($"{spaces}{EnableParameter} = {EnableParameter}");
            sb.AppendLine($"{spaces}{EnableOutput} = {EnableOutput}");
            sb.AppendLine($"{spaces}{EnableTrace} = {EnableTrace}");
            sb.AppendLine($"{spaces}{EnableDebug} = {EnableDebug}");
            sb.AppendLine($"{spaces}{EnableInfo} = {EnableInfo}");
            sb.AppendLine($"{spaces}{EnableWarn} = {EnableWarn}");
            sb.AppendLine($"{spaces}{EnableError} = {EnableError}");
            sb.AppendLine($"{spaces}{EnableFatal} = {EnableFatal}");
            return sb.ToString();
        }
    }
}
