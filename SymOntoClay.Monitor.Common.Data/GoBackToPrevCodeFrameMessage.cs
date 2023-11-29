using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class GoBackToPrevCodeFrameMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.GoBackToPrevCodeFrame;

        public int TargetActionExecutionStatus { get; set; }
        public string TargetActionExecutionStatusStr { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TargetActionExecutionStatus)} = {TargetActionExecutionStatus}");
            sb.AppendLine($"{spaces}{nameof(TargetActionExecutionStatusStr)} = {TargetActionExecutionStatusStr}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
