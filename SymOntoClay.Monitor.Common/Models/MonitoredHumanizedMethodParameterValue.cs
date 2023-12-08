using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Models
{
    public class MonitoredHumanizedMethodParameterValue : IObjectToString
    {
        public string NameHumanizedStr { get; set; }
        public string ValueHumanizedStr { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(NameHumanizedStr)} = {NameHumanizedStr}");
            sb.AppendLine($"{spaces}{nameof(ValueHumanizedStr)} = {ValueHumanizedStr}");

            return sb.ToString();
        }
    }
}
