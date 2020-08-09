﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public enum KindOfAstExpression
    {
        Unknown,
        ConstValue,
        Var,
        BinaryOperator,
        UnaryOperator,
        Channel,
        CallingFunction,
        EntityCondition
    }
}
