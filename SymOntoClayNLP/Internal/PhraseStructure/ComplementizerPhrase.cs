using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://theclassicjournal.uga.edu/index.php/2020/10/29/under-the-surface/
    //CP
    //It is "she is a teacher" in sentence "I know she is a teacher".
    public class ComplementizerPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.ComplementizerPhrase;

        public override bool IsComplementizerPhrase => true;
        public override ComplementizerPhrase AsComplementizerPhrase => this;

        /// <summary>
        /// Complementizer. It is a word which connect sentence to previous sentence.
        /// "that" in "I know that she is a teacher".
        /// </summary>
        public BaseSentenceItem C { get; set; }

        /// <summary>
        /// A sentence which is connected to previous sentence.
        /// "she is a teacher" in "I know that she is a teacher".
        /// </summary>
        public BaseSentenceItem S { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => C.RootWordFrame;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(C), C);
            sb.PrintObjProp(n, nameof(S), S);

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

