using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class RefreshLifeTimeInLogicalStorageMessage : BaseFactLogicalStorageMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.RefreshLifeTimeInLogicalStorage;

        public int NewLifetime { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(NewLifetime)} = {NewLifetime}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
