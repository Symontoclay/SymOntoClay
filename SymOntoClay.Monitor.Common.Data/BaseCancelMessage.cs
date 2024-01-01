﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseCancelMessage : BaseMessage
    {
        public string CancelledObjId { get; set; }
        public int ReasonOfChangeStatus { get; set; }
        public string ReasonOfChangeStatusStr { get; set; }
        public List<Changer> Changers { get; set; }
        public string CallMethodId { get; set; }
        
        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(CancelledObjId)} = {CancelledObjId}");
            sb.AppendLine($"{spaces}{nameof(ReasonOfChangeStatus)} = {ReasonOfChangeStatus}");
            sb.AppendLine($"{spaces}{nameof(ReasonOfChangeStatusStr)} = {ReasonOfChangeStatusStr}");
            sb.PrintObjListProp(n, nameof(Changers), Changers);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}