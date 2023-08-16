using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorFeatures : IObjectToString
    {
        public bool EnableCallMethod { get; set; }
        public bool EnableParameter { get; set; }
        public bool EnableOutput { get; set; }
        public bool EnableInfo { get; set; }

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
            sb.AppendLine($"{spaces}{EnableInfo} = {EnableInfo}");
            return sb.ToString();
        }
    }
}
