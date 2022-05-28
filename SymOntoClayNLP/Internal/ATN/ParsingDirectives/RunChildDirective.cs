using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public class RunChildDirective<T> : ParsingDirective<T>
        where T : BaseParser, new()
    {
        public RunChildDirective(object state, ConcreteATNToken concreteATNToken)
            : base(KindOfParsingDirective.RunChild, state, null, concreteATNToken, null, null)
        {
        }

        public RunChildDirective(object state, object stateAfterRunChild, ConcreteATNToken concreteATNToken)
            : base(KindOfParsingDirective.RunChild, state, stateAfterRunChild, concreteATNToken, null, null)
        {
        }

        public RunChildDirective(object state, object stateAfterRunChild, ConcreteATNToken concreteATNToken, object role)
            : base(KindOfParsingDirective.RunChild, state, stateAfterRunChild, concreteATNToken, null, role)
        {
        }
    }
}
