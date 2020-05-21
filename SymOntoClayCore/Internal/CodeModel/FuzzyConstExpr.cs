using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyConstExpr : AnnotatedItem, IFuzzyExpr
    {
        public FuzzyConstExpr(float constValue)
        {
            _value = new FuzzyValue();
            _value.Value = constValue;
        }

        private readonly FuzzyValue _value;

        public FuzzyValue Deffuzzificate(IStorage context)
        {
            return _value;
        }
    }
}
