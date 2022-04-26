using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Phrase_structure_grammar
    /// https://en.wikipedia.org/wiki/Phrase_structure_rules
    /// https://en.wikipedia.org/wiki/Phrase
    /// </summary>
    public class Sentence : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.Sentence;

        /// <inheritdoc/>
        public override bool IsSentence => true;

        /// <inheritdoc/>
        public override Sentence AsSentence => this;

        public BaseSentenceItem Condition { get; set; }
        public NounPhrase Subject { get; set; }
        public VerbPhrase Predicate { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(Subject), Subject);
            sb.PrintObjProp(n, nameof(Predicate), Predicate);

            //sb.PrintObjProp(n, nameof(), );

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
