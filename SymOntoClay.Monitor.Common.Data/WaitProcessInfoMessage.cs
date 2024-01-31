using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class WaitProcessInfoMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.WaitProcessInfo;

        public string WaitingProcessInfoId { get; set; }
        public MonitoredHumanizedLabel WaitingProcessInfo { get; set; }
        public List<MonitoredHumanizedLabel> Processes { get; set; }
        public string CallMethodId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(WaitingProcessInfoId)} = {WaitingProcessInfoId}");
            sb.PrintObjProp(n, nameof(WaitingProcessInfo), WaitingProcessInfo);
            sb.PrintObjListProp(n, nameof(Processes), Processes);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
