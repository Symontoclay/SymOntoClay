using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public class Word : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.Word;

        /// <inheritdoc/>
        public override bool IsWord => true;

        /// <inheritdoc/>
        public override Word AsWord => this;

        public bool IsNumber { get; set; }
        public string Content { get; set; } = string.Empty;

        public BaseGrammaticalWordFrame WordFrame { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNumber)} = {IsNumber}");
            sb.AppendLine($"{spaces}{nameof(Content)} = {Content}");
            sb.PrintObjProp(n, nameof(WordFrame), WordFrame);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            throw new NotImplementedException();

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToDbgString(n));

            return sb.ToString();
        }
    }
}
