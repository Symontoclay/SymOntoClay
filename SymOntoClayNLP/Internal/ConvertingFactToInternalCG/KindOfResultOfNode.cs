using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public enum KindOfResultOfNode
    {
        Unknown,
        ProcessRelation,
        ResolveVar,
        ProcessConcept,
        ProcessFact,
        ProcessConditionalEntity
    }
}
