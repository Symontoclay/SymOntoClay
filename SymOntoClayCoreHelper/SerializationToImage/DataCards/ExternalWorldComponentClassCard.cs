using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ExternalWorldComponentClassCard : IDataCard, IObjectToString
    {
        /// <inheritdoc/>
        public KindOfDataCard KindOfDataCard => KindOfDataCard.ExternalWorldComponentClassCard;

        public SerializedValue Header { get; set; }
        public string Id { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");

            return sb.ToString();
        }
    }
}
