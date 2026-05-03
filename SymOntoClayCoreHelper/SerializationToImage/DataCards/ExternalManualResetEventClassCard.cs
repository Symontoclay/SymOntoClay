using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ExternalManualResetEventClassCard : IDataCard, IObjectToString
    {
        /// <inheritdoc/>
        public KindOfDataCard KindOfDataCard => KindOfDataCard.ExternalManualResetEventClassCard;

        public SerializedValue Header { get; set; }
        public string Path { get; set; }
        public bool IsSet { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfDataCard)} = {KindOfDataCard}");
            sb.AppendLine($"{spaces}{nameof(Header)} = {Header}");
            sb.AppendLine($"{spaces}{nameof(Path)} = {Path}");
            sb.AppendLine($"{spaces}{nameof(IsSet)} = {IsSet}");

            return sb.ToString();
        }
    }
}
