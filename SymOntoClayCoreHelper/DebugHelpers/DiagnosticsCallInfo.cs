using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public class DiagnosticsCallInfo : IObjectToString
    {
        public string ClassFullName { get; set; }
        public string MethodName { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(ClassFullName)} = {ClassFullName}");
            sb.AppendLine($"{spaces}{nameof(MethodName)} = {MethodName}");
            return sb.ToString();
        }
    }
}
