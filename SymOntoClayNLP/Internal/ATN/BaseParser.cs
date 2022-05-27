using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
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
            _parserName = GetType().Name;
        }

        public abstract BaseParser Fork(ParserContext newParserContext);

        protected void FillUpBaseParser(BaseParser source, ParserContext newParserContext)
        {
            _context = newParserContext;
            _parserName = source._parserName;
            ExpectedBehavior = source.ExpectedBehavior;
            StateAfterRunChild = source.StateAfterRunChild;
        }

        protected ParserContext _context;
        private string _parserName;

        public string GetParserName()
        {
            return _parserName;
        }

        public abstract void SetStateAsInt32(int state);
        public abstract string GetStateAsString();
        public abstract string GetPhraseAsString();

        public ExpectedBehaviorOfParser ExpectedBehavior { get; set; } = ExpectedBehaviorOfParser.WaitForCurrToken;

        public int? StateAfterRunChild { get; set; }

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

        public virtual void OnVariant(ConcreteATNToken token)
        {
        }

        public virtual void OnReceiveReturn(BaseSentenceItem phrase)
        {
        }

        public virtual void OnEmptyLexer()
        {
            throw new NotImplementedException();
        }

        protected ConcreteATNToken ConvertToConcreteATNToken(ATNToken token, BaseGrammaticalWordFrame wordFrame)
        {
#if DEBUG
            //Log($"token = {token}");
            //Log($"wordFrame = {wordFrame}");
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

        protected Word ConvertToWord(ConcreteATNToken token)
        {
#if DEBUG
            //Log($"token = {token}");
#endif

            return new Word()
            {
                IsNumber = token.Kind == KindOfATNToken.Number,
                Content = token.Content,
                WordFrame = token.WordFrame
            };
        }
    }
}
