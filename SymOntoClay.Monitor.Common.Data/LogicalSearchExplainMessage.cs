using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class LogicalSearchExplainMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.LogicalSearchExplain;

        public MonitoredHumanizedLabel Query { get; set; }
        public string DotContent { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Query), Query);
            sb.AppendLine($"{spaces}{nameof(DotContent)} = {DotContent}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
