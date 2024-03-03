using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseTaskMessage : BaseMessage
    {
        public ulong TaskId { get; set; }
        public ulong TasksCount { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}{nameof(TasksCount)} = {TasksCount}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
