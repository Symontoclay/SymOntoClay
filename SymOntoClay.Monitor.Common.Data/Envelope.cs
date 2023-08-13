using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    [DataContract]
    public class Envelope : IObjectToString
    {
        [DataMember]
        public KindOfMessage KindOfMessage { get; set; }

        [DataMember]
        public string NodeId { get; set; }

        [DataMember]
        public string ThreadId { get; set; }

        [DataMember]
        public ulong GlobalMessageNumber { get; set; }

        [DataMember]
        public ulong MessageNumber { get; set; }

        [DataMember]
        public string Text { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(KindOfMessage)} = {KindOfMessage}");
            sb.AppendLine($"{spaces}{nameof(NodeId)} = {NodeId}");
            sb.AppendLine($"{spaces}{nameof(ThreadId)} = {ThreadId}");
            sb.AppendLine($"{spaces}{nameof(GlobalMessageNumber)} = {GlobalMessageNumber}");
            sb.AppendLine($"{spaces}{nameof(MessageNumber)} = {MessageNumber}");
            sb.AppendLine($"{spaces}{nameof(Text)} = {Text}");
            return sb.ToString();
        }
    }
}
