using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class ChangedRemoveFocusMessage : BecomeInvisibleMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.ChangedRemoveFocus;

        public string PublicInformationHumanizedStr { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(PublicInformationHumanizedStr)} = {PublicInformationHumanizedStr}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
