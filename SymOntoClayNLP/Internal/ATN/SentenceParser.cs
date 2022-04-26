using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class SentenceParser: BaseParser
    {
        public enum State
        {
            Init
        }

        public SentenceParser(ParserContext context)
            : base(context)
        {
        }
    }
}
