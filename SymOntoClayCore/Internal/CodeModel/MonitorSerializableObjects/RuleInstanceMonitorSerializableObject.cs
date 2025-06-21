using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Data;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects
{
    public class RuleInstanceMonitorSerializableObject : IObjectToString
    {
        public string LogicalQuery { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(LogicalQuery)} = {LogicalQuery}");
            return sb.ToString();
        }
    }
}
