using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //ConjP
    public class ConjunctionPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.ConjunctionPhrase;

        /// <inheritdoc/>
        public override bool IsConjunctionPhrase => true;

        /// <inheritdoc/>
        public override ConjunctionPhrase AsConjunctionPhrase => this;

        /// <summary>
        /// The first conjuction in pair of correlative conjunctions.
        /// "both" in "both cat and dog"
        /// </summary>
        public Word PreConj { get; set; }

        /// <summary>
        /// "cat" in "both cat and dog"
        /// "cat" in "cat and dog"
        /// </summary>
        public BaseSentenceItem Left { get; set; }

        /// <summary>
        /// The root word of the sentence or second conjuction in pair of correlative conjunctions.
        /// "and" in "both cat and dog"
        /// "and" in "cat and dog"
        /// </summary>
        public Word Conj { get; set; }

        /// <summary>
        /// "dog" in "both cat and dog"
        /// "dog" in "cat and dog"
        /// </summary>
        public BaseSentenceItem Right { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(PreConj), PreConj);
            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Conj), Conj);
            sb.PrintObjProp(n, nameof(Right), Right);

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
