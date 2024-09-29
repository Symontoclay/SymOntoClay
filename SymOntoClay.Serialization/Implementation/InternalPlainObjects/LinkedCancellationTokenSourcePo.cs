using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Serialization.Implementation.InternalPlainObjects
{
    public partial class LinkedCancellationTokenSourcePo : CancellationTokenSourcePo
    {
        public ObjectPtr Settings { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Settings)} = {Settings}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
