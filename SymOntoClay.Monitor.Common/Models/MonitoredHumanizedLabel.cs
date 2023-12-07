using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Models
{
    public class MonitoredHumanizedLabel : IObjectToString
    {
        public string CallMethodId { get; set; }
        public string KindOfCodeItemDescriptor { get; set; }
        public string Label { get; set; }
        public List<MonitoredHumanizedMethodArgument> Signatures { get; set; }
        public List<MonitoredHumanizedMethodParameterValue> Values { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.AppendLine($"{spaces}{nameof(KindOfCodeItemDescriptor)} = {KindOfCodeItemDescriptor}");
            sb.AppendLine($"{spaces}{nameof(Label)} = {Label}");
            sb.PrintObjListProp(n, nameof(Signatures), Signatures);
            sb.PrintObjListProp(n, nameof(Values), Values);

            return sb.ToString();
        }
    }
}
