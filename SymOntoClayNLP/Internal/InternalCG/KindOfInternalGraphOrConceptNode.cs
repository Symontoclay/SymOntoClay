using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public enum KindOfInternalGraphOrConceptNode
    {
        Undefined,
        Graph,
        EntityRef,
        EntityCondition,
        Property,
        Concept,
        Value,
        Variable
    }
}
