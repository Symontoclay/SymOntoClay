using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponentSettings : IObjectToString
    {
        public List<string> Categories { get; set; }
        public bool EnableCategories { get; set; }

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

            sb.PrintPODListProp(n, nameof(Categories), Categories);
            sb.AppendLine($"{spaces}{nameof(EnableCategories)} = {EnableCategories}");

            return sb.ToString();
        }
    }
}
