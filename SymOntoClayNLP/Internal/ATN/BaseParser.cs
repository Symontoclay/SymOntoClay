using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
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

        public ExpectedBehaviorOfParser ExpectedBehavior { get; set; } = ExpectedBehaviorOfParser.WaitForCurrToken;

        [MethodForLoggingSupport]
        protected void Log(string message)
        {
            _context.Log(message);
        }

        protected void SetParser(params IParsingDirective[] directives)
        {
            _context.SetParser(directives);
        }

        protected void ExitNode()
        {
            throw new NotImplementedException();
        }

        protected void ExitSentence()
        {
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

        protected ConcreteATNToken ConvertToConcreteATNToken(ATNToken token, BaseGrammaticalWordFrame wordFrame)
        {
#if DEBUG
            Log($"token = {token}");
            Log($"wordFrame = {wordFrame}");
#endif

            return new ConcreteATNToken()
            {
                 Kind = token.Kind,
                 Content = token.Content,
                 Pos = token.Pos,
                 Line = token.Line,
                 WordFrame = wordFrame
            };
        }
    }
}
