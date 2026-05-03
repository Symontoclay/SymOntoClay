using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCards
{
    public class ExternalClassCard : IDataCard, IObjectToString
    {
        public SerializedValue Header { get; set; }
        public string Path { get; set; }
        public Dictionary<string, SerializedValue> Fields { get; set; }
        public Dictionary<string, SerializedValue> Properties { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Header)} = {Header}");
            sb.AppendLine($"{spaces}{nameof(Path)} = {Path}");
            sb.PrintPODDictProp(n, nameof(Fields), Fields);
            sb.PrintPODDictProp(n, nameof(Properties), Properties);

            return sb.ToString();
        }
    }
}
