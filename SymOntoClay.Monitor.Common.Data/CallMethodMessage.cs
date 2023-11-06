using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public class CallMethodMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.CallMethod;

        public string CallMethodId { get; set; }
        public string MethodName { get; set; }
        public bool IsSynk { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.AppendLine($"{spaces}{nameof(MethodName)} = {MethodName}");
            sb.AppendLine($"{spaces}{nameof(IsSynk)} = {IsSynk}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
