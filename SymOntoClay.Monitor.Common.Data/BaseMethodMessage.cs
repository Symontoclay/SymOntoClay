using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseMethodMessage : BaseMessage
    {
        public string CallMethodId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
