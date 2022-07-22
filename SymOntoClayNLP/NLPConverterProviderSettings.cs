using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverterProviderSettings : IObjectToString
    {
        public IList<string> DictsPaths { get; set; }
        public CreationStrategy CreationStrategy { get; set; } = CreationStrategy.Unknown;

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

            sb.AppendLine($"{spaces}{nameof(CreationStrategy)} = {CreationStrategy}");
            sb.PrintPODList(n, nameof(DictsPaths), DictsPaths);

            return sb.ToString();
        }
    }
}
