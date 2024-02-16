using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class AddFactOrRuleTriggerResultMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.AddFactOrRuleTriggerResult;

        public MonitoredHumanizedLabel Fact { get; set; }
        public MonitoredHumanizedLabel LogicalStorage { get; set; }
        public MonitoredHumanizedLabel Result { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Fact), Fact);
            sb.PrintObjProp(n, nameof(LogicalStorage), LogicalStorage);
            sb.PrintObjProp(n, nameof(Result), Result);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
