using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public enum KindOfVerbPhraseTarget
    {
        Unknown,
        Intrusion,
        V,
        VP,
        Negation,
        Particle,
        Object,
        PP,
        CP
    }
}
