using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class SetProcessInfoStatusMessage : BaseSetStatusMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.SetProcessInfoStatus;

        public MonitoredHumanizedLabel ProcessInfo { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
