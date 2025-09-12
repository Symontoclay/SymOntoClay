using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class BecomeVisibleMessage: BecomeInvisibleMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.BecomeVisible;

        public float Distance { get; set; }
        public string PublicInformationHumanizedStr { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(PublicInformationHumanizedStr)} = {PublicInformationHumanizedStr}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
