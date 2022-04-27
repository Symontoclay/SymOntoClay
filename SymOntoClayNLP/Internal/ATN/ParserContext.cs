using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ParserContext : IObjectToString
    {
        public ParserContext(GlobalParserContext globalContext, INLPContext context, IWordsDict wordsDict, string text)
        {
            _logger = context.Logger;
            _globalContext = globalContext;

            _lexer = new ATNLexer(text, wordsDict);

            _parsers = new Stack<BaseParser>();

            SetParser(new ParsingDirective<SentenceParser, SentenceParser.State>(SentenceParser.State.Init));
        }

        private GlobalParserContext _globalContext;
        private ATNLexer _lexer;
        private Stack<BaseParser> _parsers;
        private BaseParser _currentParser;
        private readonly IEntityLogger _logger;
        private bool _isActive = true;

        public void SetParser(params IParsingDirective[] directives)
        {
#if DEBUG
            _logger.Log($"directives = {directives.WriteListToString()}");
#endif

            if(!_parsers.Any())
            {
                if(directives.Length != 1)
                {
                    throw new Exception($"directives.Length != 1. Actual value is {directives.Length}.");
                }

                var targetDirective = directives.Single();

                var parser = targetDirective.CreateParser(this);

                SetParser(parser);

                return;
            }

            throw new NotImplementedException();
        }

        private void SetParser(BaseParser parser)
        {
            _currentParser = parser;
            _parsers.Push(parser);

            _currentParser.OnEnter();
        }

        public bool IsActive => _isActive;

        private ATNToken _currentToken;

        public void Run()
        {
#if DEBUG
            _logger.Log($"Begin");
#endif

            var expectedBehavior = _currentParser.ExpectedBehavior;

#if DEBUG
            _logger.Log($"expectedBehavior = {expectedBehavior}");
#endif

            switch (expectedBehavior)
            {
                case ExpectedBehaviorOfParser.WaitForCurrToken:
                    {
                        _currentToken = _lexer.GetToken();

#if DEBUG
                        _logger.Log($"_currentToken = {_currentToken}");
#endif
                        if(_currentToken == null)
                        {
                            throw new NotImplementedException();
                        }

                        _currentParser.OnRun(_currentToken);

                        throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedBehavior), expectedBehavior, null);
            }

#if DEBUG
            _logger.Log($"End");
#endif
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
