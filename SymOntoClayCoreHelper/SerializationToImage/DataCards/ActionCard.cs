using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ActionCard : IDataCard, IObjectToString
    {
        /// <inheritdoc/>
        public KindOfDataCard KindOfDataCard => KindOfDataCard.ActionCard;

        public SerializedValue Header { get; set; }

        public string KeyValue { get; set; }

        public int Index { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(KeyValue)} = {KeyValue}");
            sb.AppendLine($"{spaces}{nameof(Index)} = {Index}");

            return sb.ToString();
        }
    }
}
