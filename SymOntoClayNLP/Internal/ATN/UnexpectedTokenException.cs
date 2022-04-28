using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class UnExpectedTokenException: FailStepInPhraseException
    {
        public UnExpectedTokenException(ATNToken token)
            : base($"Unexpected token {token.ToDebugString()}.")
        {
        }
    }
}
