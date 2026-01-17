using MessagePack;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Text;

namespace TestSandbox.MessagePacking
{
    [MessagePackObject]
    public abstract partial class TstBaseLogMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }

        public partial DateTime Timestamp { get; set; }

        public partial string Level { get; set; }

        public partial string Message { get; set; }

        public partial string Exception { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfMessage)} = {KindOfMessage}");
            sb.AppendLine($"{spaces}{nameof(Timestamp)} = {Timestamp}");
            sb.AppendLine($"{spaces}{nameof(Level)} = {Level}");
            sb.AppendLine($"{spaces}{nameof(Message)} = {Message}");
            sb.AppendLine($"{spaces}{nameof(Exception)} = {Exception}");

            return sb.ToString();
        }
    }
}
