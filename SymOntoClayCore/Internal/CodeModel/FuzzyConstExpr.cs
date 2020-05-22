using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyConstExpr : AnnotatedItem, IFuzzyExpr
    {
        public FuzzyConstExpr(float constValue, ICodeModelContext context)
            : base(context)
        {
            _value = new FuzzyValue(context);
            _value.Value = constValue;
        }

        private readonly FuzzyValue _value;

        public FuzzyValue Deffuzzificate(IStorage execContext)
        {
            return _value;
        }
    }
}
