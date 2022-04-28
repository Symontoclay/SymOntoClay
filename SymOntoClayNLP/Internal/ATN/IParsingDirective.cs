using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public interface IParsingDirective : IObjectToString
    {
        KindOfParsingDirective KindOfParsingDirective { get; }
        int State { get; }
        bool RecoveryCurrToken { get; }
        ConcreteATNToken ConcreteATNToken { get; }

        BaseParser CreateParser(ParserContext parserContext);

        Type ParserType { get; }
    }
}
