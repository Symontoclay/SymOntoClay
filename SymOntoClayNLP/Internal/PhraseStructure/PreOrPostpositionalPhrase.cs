using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://en.wikipedia.org/wiki/Adpositional_phrase
    //PP
    public class PreOrPostpositionalPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.PreOrPostpositionalPhrase;

        /// <inheritdoc/>
        public override bool IsPreOrPostpositionalPhrase => true;

        /// <inheritdoc/>
        public override PreOrPostpositionalPhrase AsPreOrPostpositionalPhrase => this;

        /// <summary>
        /// The root word of the phrase: "with" in "with them".
        /// Or "from" in "from now on".
        /// </summary>
        public BaseSentenceItem P { get; set; }

        /// <summary>
        /// "on" in "from now on".
        /// </summary>
        public BaseSentenceItem P2 { get; set; }

        /// <summary>
        /// "now" in "from now on".
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <summary>
        /// "them" in "with them"
        /// "two years" in "two years ago"
        /// </summary>
        public BaseSentenceItem NP { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => P.RootWordFrame;

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public PreOrPostpositionalPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public PreOrPostpositionalPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PreOrPostpositionalPhrase)context[this];
            }

            var result = new PreOrPostpositionalPhrase();
            context[this] = result;

            result.P = P?.CloneBaseSentenceItem(context);
            result.P2 = P2?.CloneBaseSentenceItem(context);
            result.Adv = Adv?.CloneBaseSentenceItem(context);
            result.NP = NP?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(P), P);
            sb.PrintObjProp(n, nameof(P2), P2);
            sb.PrintObjProp(n, nameof(Adv), Adv);
            sb.PrintObjProp(n, nameof(NP), NP);

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
