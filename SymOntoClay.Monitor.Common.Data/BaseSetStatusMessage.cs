using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseSetStatusMessage : BaseMessage
    {
        public string ObjId { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public int PrevStatus { get; set; }
        public string PrevStatusStr { get; set; }
        public List<Changer> Changers { get; set; }
        public string CallMethodId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ObjId)} = {ObjId}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(StatusStr)} = {StatusStr}");
            sb.AppendLine($"{spaces}{nameof(PrevStatus)} = {PrevStatus}");
            sb.AppendLine($"{spaces}{nameof(PrevStatusStr)} = {PrevStatusStr}");
            sb.PrintObjListProp(n, nameof(Changers), Changers);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
