using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public enum KindOfParsingDirective
    {
        RunCurrToken,
        RunVariant,
        RunChild,
        ReturnToParent
    }
}
