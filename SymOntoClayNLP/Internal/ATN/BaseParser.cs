/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        public virtual void SetRole(object role)
        {
        }

        public virtual string GetRoleAsString() => string.Empty;

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
