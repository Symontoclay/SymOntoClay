using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
{
    public class ResultOfNode : IObjectToString
    {
        public string MainText { get; set; } = string.Empty;
        public string RootWord { get; set; } = string.Empty;

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

            sb.AppendLine($"{spaces}{nameof(MainText)} = {MainText}");
            sb.AppendLine($"{spaces}{nameof(RootWord)} = {RootWord}");

            return sb.ToString();
        }
    }
}
