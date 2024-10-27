using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;
using System.Threading;

namespace SymOntoClay.Serialization.Settings
{
    public class LinkedCancellationTokenSourceSerializationSettings : IObjectToString
    {
        public CancellationToken? Token1 { get; set; }
        public CancellationToken? Token2 { get; set; }
        public CancellationToken? Token3 { get; set; }
        public CancellationToken? Token4 { get; set; }
        public CancellationToken? Token5 { get; set; }
        public CancellationToken? Token6 { get; set; }
        public CancellationToken? Token7 { get; set; }
        public CancellationToken? Token8 { get; set; }
        public CancellationToken? Token9 { get; set; }
        public CancellationToken? Token10 { get; set; }

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
            sb.PrintExisting(n, nameof(Token3), Token3);
            sb.PrintExisting(n, nameof(Token4), Token4);
            sb.PrintExisting(n, nameof(Token5), Token5);
            sb.PrintExisting(n, nameof(Token6), Token6);
            sb.PrintExisting(n, nameof(Token7), Token7);
            sb.PrintExisting(n, nameof(Token8), Token8);
            sb.PrintExisting(n, nameof(Token9), Token9);
            sb.PrintExisting(n, nameof(Token10), Token10);
            return sb.ToString();
        }
    }
}
