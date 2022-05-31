using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
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
        public override BaseGrammaticalWordFrame RootWordFrame => A.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => A.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return A.GetConditionalLogicalMeaning(conditionalWord);
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
        public AdjectivePhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public AdjectivePhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AdjectivePhrase)context[this];
            }

            var result = new AdjectivePhrase();
            context[this] = result;

            result.AdvP = AdvP?.CloneBaseSentenceItem(context);
            result.A = A?.Clone(context);
            result.PP = PP?.CloneBaseSentenceItem(context);

            return result;
        }

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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNspaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}AP");

            if(AdvP != null)
            {
                sb.AppendLine($"{nextNspaces}AdvP:");
                sb.Append(AdvP.ToDbgString(nextNextN));
            }

            if (A != null)
            {
                sb.AppendLine($"{nextNspaces}A:");
                sb.Append(A.ToDbgString(nextNextN));
            }

            if (PP != null)
            {
                sb.AppendLine($"{nextNspaces}PP:");
                sb.Append(PP.ToDbgString(nextNextN));
            }

            return sb.ToString();
        }
    }
}
