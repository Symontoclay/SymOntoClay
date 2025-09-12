using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseVisionFrameMessage: BaseMessage
    {
        public string VisionFrameId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(VisionFrameId)} = {VisionFrameId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
