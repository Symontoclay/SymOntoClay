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
        Word
    }
}
