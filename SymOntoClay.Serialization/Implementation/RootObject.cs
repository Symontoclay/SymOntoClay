using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Common;
using System.Text;

namespace SymOntoClay.Serialization.Implementation
{
    public class RootObject : IObjectToString
    {
        public ObjectPtr Data { get; set; }

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Data)} = {Data}");
            return sb.ToString();
        }
    }
}
