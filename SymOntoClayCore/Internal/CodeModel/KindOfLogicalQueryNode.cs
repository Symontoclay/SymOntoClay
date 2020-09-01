using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfLogicalQueryNode
    {
        Unknown,
        Relation,
        Concept,
        Entity,
        EntityRef,
        EntityCondition,
        LogicalVar,
        QuestionVar,
        BinaryOperator,
        UnaryOperator,
        StubParam,
        Value
    }
}
