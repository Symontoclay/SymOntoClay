using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;
using System.Threading;

namespace SymOntoClay.Serialization.Settings
{
    [SocSerialization]
    public partial class LinkedCancellationTokenSourceSettings: IObjectToString
    {
        public CancellationToken? Token1 { get; set; }
        public CancellationToken? Token2 { get; set; }

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
            sb.PrintExisting(n, nameof(Token1), Token1);
            sb.PrintExisting(n, nameof(Token2), Token2);
            return sb.ToString();
        }
    }
}
