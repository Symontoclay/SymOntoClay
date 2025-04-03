using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects
{
    public class ConditionalEntityValueMonitorSerializableObject: BaseEntityValueMonitorSerializableObject
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public string LogicalQuery { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(Expression)} = {Expression}");
            sb.AppendLine($"{spaces}{nameof(LogicalQuery)} = {LogicalQuery}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
