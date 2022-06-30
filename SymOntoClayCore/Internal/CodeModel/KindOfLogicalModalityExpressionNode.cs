using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfLogicalModalityExpressionNode
    {
        Unknown,
        BlankIdentifier,
        BinaryOperator,
        UnaryOperator,
        Value,
        Group
    }
}
