using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public interface IParsingDirective : IObjectToString
    {
        BaseParser CreateParser(ParserContext parserContext);
    }
}
