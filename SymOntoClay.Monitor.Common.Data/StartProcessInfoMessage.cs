using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class StartProcessInfoMessage: BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.StartProcessInfo;

        public string ProcessInfoId {  get; set; }
        public MonitoredHumanizedLabel ProcessInfo { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ProcessInfoId)} = {ProcessInfoId}");
            sb.PrintObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
