using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Serialization.Implementation.InternalPlainObjects
{
    public class ActionPo : IObjectToString
    {
        public string Key { get; set; }
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(Index)} = {Index}");
            return sb.ToString();
        }
    }
}
