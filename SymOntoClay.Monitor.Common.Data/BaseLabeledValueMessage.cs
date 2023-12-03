using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseLabeledValueMessage : BaseValueMessage
    {
        public string CallMethodId { get; set; }

        public MonitoredHumanizedLabel Label { get; set; }
        public string AltLabel { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.AppendLine($"{spaces}{nameof(AltLabel)} = {AltLabel}");
            sb.PrintObjProp(n, nameof(Label), Label);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
