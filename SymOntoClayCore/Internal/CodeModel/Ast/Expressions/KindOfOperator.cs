using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public enum KindOfOperator
    {
        Unknown,
        Is,
        IsNot,
        LeftRightStream,
        Point,
        CallFunction,
        Predicate,
        And,
        CallLogicalQuery,
        Assign
    }
}
