using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public abstract class BaseParser
    {
        public void SetContext(ParserContext context)
        {
            _context = context;
        }

        protected ParserContext _context;

        public abstract void SetStateAsInt32(int state);

        private ExpectedBehaviorOfParser _expectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;

        public ExpectedBehaviorOfParser ExpectedBehavior => _expectedBehavior;

        public virtual void OnEnter()
        {
        }

        public virtual void OnRun(ATNToken token)
        {
        }

        public virtual void OnVariant(ConcreteATNToken concreteATNToken)
        {
        }

        public virtual void OnReceiveReturn(BaseSentenceItem phrase)
        {
        }

        public virtual void OnFinish()
        {
        }
    }
}
