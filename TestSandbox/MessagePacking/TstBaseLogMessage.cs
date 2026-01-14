using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Text;
using System.Threading;

namespace TestSandbox.MessagePacking
{
    public abstract class TstBaseLogMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string? Exception { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(DateTimeStamp)} = {DateTimeStamp}");
            sb.AppendLine($"{spaces}{nameof(NodeId)} = {NodeId}");
            sb.AppendLine($"{spaces}{nameof(ThreadId)} = {ThreadId}");
            sb.AppendLine($"{spaces}{nameof(GlobalMessageNumber)} = {GlobalMessageNumber}");
            sb.AppendLine($"{spaces}{nameof(ClassFullName)} = {ClassFullName}");
            sb.AppendLine($"{spaces}{nameof(MessageNumber)} = {MessageNumber}");
            sb.AppendLine($"{spaces}{nameof(MessagePointId)} = {MessagePointId}");
            sb.AppendLine($"{spaces}{nameof(MemberName)} = {MemberName}");
            sb.AppendLine($"{spaces}{nameof(SourceFilePath)} = {SourceFilePath}");
            sb.AppendLine($"{spaces}{nameof(SourceLineNumber)} = {SourceLineNumber}");

            return sb.ToString();
        }
    }
}
