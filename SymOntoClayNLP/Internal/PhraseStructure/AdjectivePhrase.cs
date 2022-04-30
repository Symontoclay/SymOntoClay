using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://en.wikipedia.org/wiki/Adjective_phrase
    //AP
    public class AdjectivePhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.AdjectivePhrase;

        /// <inheritdoc/>
        public override bool IsAdjectivePhrase => true;

        /// <inheritdoc/>
        public override AdjectivePhrase AsAdjectivePhrase => this;

        /// <summary>
        /// "very" in "very big"
        /// </summary>
        public BaseSentenceItem AdvP { get; set; }

        /// <summary>
        /// The root ajective of the phrase.
        /// </summary>
        public Word A { get; set; }

        /// <summary>
        /// "of It" in "proud of It"
        /// </summary>
        public BaseSentenceItem PP { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(AdvP), AdvP);
            sb.PrintObjProp(n, nameof(A), A);
            sb.PrintObjProp(n, nameof(PP), PP);

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
