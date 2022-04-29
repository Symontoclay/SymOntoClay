using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public class RunCurrTokenDirective<T>: ParsingDirective<T>
        where T : BaseParser, new()
    {
        public RunCurrTokenDirective(object state)
            : base(KindOfParsingDirective.RunCurrToken, state, null, null, null)
        {
        }
    }
}
