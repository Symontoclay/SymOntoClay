using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public enum ExpectedBehaviorOfParser
    {
        WaitForCurrToken,
        WaitForVariant,
        WaitForReceiveReturn
    }
}
