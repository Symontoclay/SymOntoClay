using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://www-users.york.ac.uk/~lang18/Documentation/syn-intro.htm
    //https://www.researchgate.net/publication/221761397_A_Multi-Classifier_Based_Guideline_Sentence_Classification_System
    //https://www.researchgate.net/figure/Parsed-sentence-from-which-to-extract-the-feature-vector-CC-coordinating-conjunction_fig3_221761397
    //QP
    //"at least one", "very many"
    public class QuantifierPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.QuantifierPhrase;

        /// <inheritdoc/>
        public override bool IsQuantifierPhrase => true;

        /// <inheritdoc/>
        public override QuantifierPhrase AsQuantifierPhrase => this;

        /// <summary>
        /// The root word of the sentence.
        /// "many" in "very many"
        /// </summary>
        public BaseSentenceItem Q { get; set; }

        /// <summary>
        /// "very" in "very many"
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => Q.RootWordFrame;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Q), Q);
            sb.PrintObjProp(n, nameof(Adv), Adv);

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

            return sb.ToString();
        }
    }
}
