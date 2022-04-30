using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://en.wikipedia.org/wiki/Adverbial_phrase
    //AdvP
    public class AdverbialPhrase: BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.AdverbialPhrase;

        /// <inheritdoc/>
        public override bool IsAdverbialPhrase => true;

        /// <inheritdoc/>
        public override AdverbialPhrase AsAdverbialPhrase => this;

        /// <summary>
        /// The root adverbial of the phrase.
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <summary>
        /// "quickly" in "very quickly"
        /// </summary>
        public BaseSentenceItem AdvP { get; set; }

        /// <summary>
        /// "beautiful" in "very beautiful"
        /// </summary>
        public BaseSentenceItem AP { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Adv), Adv);
            sb.PrintObjProp(n, nameof(AdvP), AdvP);
            sb.PrintObjProp(n, nameof(AP), AP);

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
