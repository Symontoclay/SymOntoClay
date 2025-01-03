using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BasePrimitiveTaskMessage : BaseMessage
    {
        public ulong TaskId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
