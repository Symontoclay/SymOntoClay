using SymOntoClay.CoreHelper.DebugHelpers;
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

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

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
