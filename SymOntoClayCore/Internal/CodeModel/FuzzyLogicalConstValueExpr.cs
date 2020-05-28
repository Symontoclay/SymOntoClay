using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyLogicalConstValueExpr : AnnotatedItem, IFuzzyExpr
    {
        public FuzzyLogicalConstValueExpr(float constValue, ICodeModelContext context)
            : base(context)
        {
            _value = new FuzzyLogicalValue(constValue, context);
        }

        private readonly FuzzyLogicalValue _value;

        public Value Deffuzzificate(IStorage execContext)
        {
            return _value;
        }
    }
}
