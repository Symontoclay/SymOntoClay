using MessagePack;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System.Text;

namespace TestSandbox.MessagePacking
{
    [MessagePackObject]
    public partial class TstLogMessage : TstBaseLogMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.ActionResolving;

        public partial string SomeField { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(SomeField)} = {SomeField}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
