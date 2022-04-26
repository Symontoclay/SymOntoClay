using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ParsingResult : IObjectToString
    {
        public bool IsSuccess { get; set; }
        public int CountSteps { get; set; }
        public BaseSentenceItem Result { get; set; }
        public Exception Error { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(CountSteps)} = {CountSteps}");
            sb.PrintObjProp(n, nameof(Result), Result);
            sb.AppendLine($"{spaces}{nameof(Error)} = {Error}");

            return sb.ToString();
        }
    }
}
