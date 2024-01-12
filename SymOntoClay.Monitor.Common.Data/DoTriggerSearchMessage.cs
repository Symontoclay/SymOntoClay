using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class DoTriggerSearchMessage: BaseConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.DoTriggerSearch;

        public string InstanceId { get; set; }
        public string Holder { get; set; }
        public MonitoredHumanizedLabel TriggerLabel { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(InstanceId)} = {InstanceId}");
            sb.AppendLine($"{spaces}{nameof(Holder)} = {Holder}");
            sb.PrintObjProp(n, nameof(TriggerLabel), TriggerLabel);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
