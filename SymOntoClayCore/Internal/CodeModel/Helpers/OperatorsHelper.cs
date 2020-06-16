using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class OperatorsHelper
    {
        //I use operator precedence of C and C++. https://en.wikipedia.org/wiki/Operators_in_C_and_C%2B%2B
        public int GetPriority(KindOfOperator kind)
        {
            switch(kind)
            {
                case KindOfOperator.LeftRightStream:
                    return 15;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
