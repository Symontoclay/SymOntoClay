using SymOntoClay.CoreHelper.DebugHelpers;
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
            _logger = context.Logger;
        }

        protected ParserContext _context;
        protected IEntityLogger _logger;

        public abstract void SetStateAsInt32(int state);

        private ExpectedBehaviorOfParser _expectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;

        public ExpectedBehaviorOfParser ExpectedBehavior => _expectedBehavior;

        protected void SetParser(params IParsingDirective[] directives)
        {
#if DEBUG
            _logger.Log($"directives = {directives.WriteListToString()}");
#endif

            throw new NotImplementedException();
        }

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
