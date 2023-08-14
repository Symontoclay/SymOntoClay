using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }
        public DateTime DateTimeStamp { get; set; }
        public string NodeId { get; set; }
        public string ThreadId { get; set; }
        public ulong GlobalMessageNumber { get; set; }
        public ulong MessageNumber { get; set; }
        public string MessagePointId { get; set; }
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
        public int SourceLineNumber { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(MessageNumber)} = {MessageNumber}");
            sb.AppendLine($"{spaces}{nameof(MessagePointId)} = {MessagePointId}");
            sb.AppendLine($"{spaces}{nameof(MemberName)} = {MemberName}");
            sb.AppendLine($"{spaces}{nameof(SourceFilePath)} = {SourceFilePath}");
            sb.AppendLine($"{spaces}{nameof(SourceLineNumber)} = {SourceLineNumber}");

            return sb.ToString();
        }
    }
}
