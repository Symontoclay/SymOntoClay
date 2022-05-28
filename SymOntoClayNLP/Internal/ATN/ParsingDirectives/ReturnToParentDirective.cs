using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public class ReturnToParentDirective: ParsingDirective<EmptyDummyParser>
    {
        public ReturnToParentDirective(BaseSentenceItem phrase, ConcreteATNToken concreteATNToken)
            : base(KindOfParsingDirective.ReturnToParent, null, null, concreteATNToken, phrase, null)
        {
        }

        public ReturnToParentDirective(BaseSentenceItem phrase)
            : base(KindOfParsingDirective.ReturnToParent, null, null, null, phrase, null)
        {
        }
    }
}
