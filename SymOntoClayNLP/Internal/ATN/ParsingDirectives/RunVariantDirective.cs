using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public class RunVariantDirective<T> : ParsingDirective<T>
        where T : BaseParser, new()
    {
        public RunVariantDirective(object state, ConcreteATNToken concreteATNToken)
            : base(KindOfParsingDirective.RunVariant, state, null, concreteATNToken, null)
        {
        }
    }
}
