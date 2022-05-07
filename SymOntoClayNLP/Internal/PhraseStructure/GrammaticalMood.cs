using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// See at https://en.wikipedia.org/wiki/Grammatical_mood
    /// </summary>
    public enum GrammaticalMood
    {
        Undefined,
        Indicative,
        Subjunctive,
        Imperative,
        Jussive,
        Potential,
        Hypothetical,
        Hortative,
        Optative
    }
}
