using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public abstract class BaseParser
    {
        protected virtual void OnEnter()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnRun()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnVariant(ConcreteATNToken concreteATNToken)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnReceiveReturn(BaseSentenceItem phrase)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnFinish()
        {
            throw new NotImplementedException();
        }
    }
}
