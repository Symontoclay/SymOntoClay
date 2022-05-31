using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
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
        public override BaseGrammaticalWordFrame RootWordFrame => Adv.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => Adv.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return Adv.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AdverbialPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public AdverbialPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AdverbialPhrase)context[this];
            }

            var result = new AdverbialPhrase();
            context[this] = result;

            result.Adv = Adv?.CloneBaseSentenceItem(context);
            result.AdvP = AdvP?.CloneBaseSentenceItem(context);
            result.AP = AP?.CloneBaseSentenceItem(context);

            return result;
        }

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
