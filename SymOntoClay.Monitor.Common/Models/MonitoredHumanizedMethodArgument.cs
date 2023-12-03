using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Monitor.Common.Models
{
    public class MonitoredHumanizedMethodArgument: IObjectToString
    {
        public string HumanizedStr { get; set; }

        public List<string> TypesList { get; set; }

        public MonitoredHumanizedMethodParameterValue DefaultValue { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(HumanizedStr)} = {HumanizedStr}");
            sb.PrintPODList(n, nameof(TypesList), TypesList);
            sb.PrintObjProp(n, nameof(DefaultValue), DefaultValue);

            return sb.ToString();
        }
    }
}
