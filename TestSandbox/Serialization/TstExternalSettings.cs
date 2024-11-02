using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace TestSandbox.Serialization
{
    public class TstExternalSettings : IObjectToString
    {
        public string Prop1 { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(Prop1)} = {Prop1}");
            //sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }
    }
}
