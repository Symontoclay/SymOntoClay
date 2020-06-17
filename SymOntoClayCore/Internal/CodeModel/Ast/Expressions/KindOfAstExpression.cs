using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public enum KindOfAstExpression
    {
        Unknown,
        ConstValue,
        BinaryOperator,
        UnaryOperator,
        Channel
    }
}
