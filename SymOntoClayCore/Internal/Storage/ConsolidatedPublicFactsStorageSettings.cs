using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ConsolidatedPublicFactsStorageSettings : IObjectToString
    {
        public KindOfOnAddingFactEvent EnableOnAddingFactEvent { get; set; } = KindOfOnAddingFactEvent.None;

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EnableOnAddingFactEvent)} = {EnableOnAddingFactEvent}");

            return sb.ToString();
        }
    }
}
