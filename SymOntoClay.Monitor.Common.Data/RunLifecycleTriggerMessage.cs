using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class RunLifecycleTriggerMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.RunLifecycleTrigger;

        public string InstanceId { get; set; }
        public string Holder { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public MonitoredHumanizedLabel Label { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(InstanceId)} = {InstanceId}");
            sb.AppendLine($"{spaces}{nameof(Holder)} = {Holder}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(StatusStr)} = {StatusStr}");
            sb.PrintObjProp(n, nameof(Label), Label);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
