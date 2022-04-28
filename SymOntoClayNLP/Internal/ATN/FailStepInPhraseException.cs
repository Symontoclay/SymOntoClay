using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class FailStepInPhraseException: Exception
    {
        public FailStepInPhraseException()
        {
        }

        public FailStepInPhraseException(string message)
            : base(message)
        {
        }
    }
}
