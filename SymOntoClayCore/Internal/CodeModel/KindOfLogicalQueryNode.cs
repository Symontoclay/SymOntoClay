﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfLogicalQueryNode
    {
        Unknown,
        Relation,
        Concept,
        EntityRef,
        EntityCondition,
        Var,
        QuestionVar,
        BinaryOperator,
        UnaryOperator,
        StubParam,
        Value
    }
}
