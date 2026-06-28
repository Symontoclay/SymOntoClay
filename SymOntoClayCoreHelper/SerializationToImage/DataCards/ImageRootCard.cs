using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ImageRootCard : IDataCard, IObjectToString
    {
        /// <inheritdoc/>
        public KindOfDataCard KindOfDataCard => KindOfDataCard.ImageRootCard;

        public SerializedValue RootSerializedValue { get; set; }
        public SerializedValue RootSerializedSettingsValue { get; set; }
        public SerializedValue MainRootSerializedValue { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(RootSerializedValue)} = {RootSerializedValue}");
            sb.AppendLine($"{spaces}{nameof(RootSerializedSettingsValue)} = {RootSerializedSettingsValue}");
            sb.AppendLine($"{spaces}{nameof(MainRootSerializedValue)} = {MainRootSerializedValue}");

            return sb.ToString();
        }
    }
}
