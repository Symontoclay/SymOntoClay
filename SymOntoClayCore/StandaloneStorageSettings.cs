using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class StandaloneStorageSettings: BaseStorageSettings
    {
        public bool IsWorld { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsWorld)} = {IsWorld}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
