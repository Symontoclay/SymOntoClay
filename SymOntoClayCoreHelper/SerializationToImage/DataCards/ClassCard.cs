using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ClassCard : IDataCard, IDataCardWithHeader, IObjectToString
    {
        /// <inheritdoc/>
        public KindOfDataCard KindOfDataCard => KindOfDataCard.ClassCard;

        /// <inheritdoc/>
        public SerializedValue Header { get; set; }

        public List<(string, int, SerializedValue)> Fields { get; set; }
        public List<(string, int, SerializedValue)> Properties { get; set; }

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
            sb.PrintPODListProp(n, nameof(Fields), Fields);
            sb.PrintPODListProp(n, nameof(Properties), Properties);

            return sb.ToString();
        }
    }
}
