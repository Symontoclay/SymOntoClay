using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public enum KindOfSentenceItem
    {
        Sentence,
        /// <summary>
        /// NP
        /// </summary>
        NounPhrase,
        /// <summary>
        /// VP
        /// </summary>
        VerbPhrase,
        /// <summary>
        /// AP
        /// </summary>
        AdjectivePhrase,
        /// <summary>
        /// PP
        /// </summary>
        PreOrPostpositionalPhrase,
        /// <summary>
        /// AdvP
        /// </summary>
        AdverbialPhrase,
        /// <summary>
        /// CP
        /// </summary>
        ComplementizerPhrase,
        /// <summary>
        /// QP
        /// </summary>
        QuantifierPhrase,
        /// <summary>
        /// ConjP
        /// </summary>
        ConjunctionPhrase,
        Word
    }
}
