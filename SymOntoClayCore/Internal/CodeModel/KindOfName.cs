using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfName
    {
        Unknown,
        Concept,
        Entity,
        EntityCondition,
        AnonymousEntityCondition,
        EntityRefByConcept,
        Channel,
        Var,
        SystemVar,
        LogicalVar,
        QuestionVar
    }
}
