using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public interface IParsingDirective : IObjectToString
    {
        KindOfParsingDirective KindOfParsingDirective { get; }
        int? State { get; }
        int? StateAfterRunChild { get; }
        ConcreteATNToken ConcreteATNToken { get; }
        BaseSentenceItem Phrase { get; }

        BaseParser CreateParser(ParserContext parserContext);

        Type ParserType { get; }
    }
}
