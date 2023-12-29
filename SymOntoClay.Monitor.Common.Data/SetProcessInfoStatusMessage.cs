using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class SetProcessInfoStatusMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.SetProcessInfoStatus;

        public int Status { get; set; }
        public string StatusStr { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(StatusStr)} = {StatusStr}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
