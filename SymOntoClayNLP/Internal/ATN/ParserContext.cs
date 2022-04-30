using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
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
        }

        private GlobalParserContext _globalContext;
        private ATNLexer _lexer;
        private Stack<BaseParser> _parsers;
        private BaseParser _currentParser;
        private readonly IEntityLogger _logger;
        private bool _isActive = true;
        private List<string> _logMessages = new List<string>();

        private int _globalCounter = 0;
        private int _sentenceCounter = 0;

        [MethodForLoggingSupport]
        public void Log(string message)
        {
            _logMessages.Add(message);

            _logger.Log(message);
        }

        private bool IsNotParsers()
        {
            return !_parsers.Any() && _currentParser == null;
        }

        public void SetParser(params IParsingDirective[] directives)
        {
#if DEBUG
            //_logger.Log($"directives = {directives.WriteListToString()}");
#endif

            if(IsNotParsers())
            {
                if(directives.Length != 1)
                {
                    throw new Exception($"directives.Length != 1. Actual value is {directives.Length}.");
                }

                var targetDirective = directives.Single();

                var parser = targetDirective.CreateParser(this);

                parser.SetStateAsInt32(targetDirective.State.Value);

                SetParser(parser);

                return;
            }

            if(directives.Length == 1)
            {
                var targetDirective = directives.Single();

#if DEBUG
                //_logger.Log($"targetDirective.ParserType?.FullName = {targetDirective.ParserType?.FullName}");
                //_logger.Log($"_currentParser.GetType().FullName = {_currentParser.GetType().FullName}");
#endif

                if(targetDirective.ParserType == _currentParser.GetType())
                {
                    _currentParser.SetStateAsInt32(targetDirective.State.Value);

                    var kindOfParsingDirective = targetDirective.KindOfParsingDirective;

                    switch (kindOfParsingDirective)
                    {
                        case KindOfParsingDirective.RunVariant:
                            _currentParser.ExpectedBehavior = ExpectedBehaviorOfParser.WaitForVariant;
                            _concreteATNToken = targetDirective.ConcreteATNToken;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfParsingDirective), kindOfParsingDirective, null);
                    }
                }
                else
                {
                    var kindOfParsingDirective = targetDirective.KindOfParsingDirective;

#if DEBUG
                    //_logger.Log($"kindOfParsingDirective = {kindOfParsingDirective}");
#endif

                    switch (kindOfParsingDirective)
                    {
                        case KindOfParsingDirective.RunChild:
                            {
                                _currentParser.StateAfterRunChild = targetDirective.StateAfterRunChild;
                                _currentParser.ExpectedBehavior = ExpectedBehaviorOfParser.WaitForReceiveReturn;

#if DEBUG
                                //_logger.Log($"targetDirective.ConcreteATNToken = {targetDirective.ConcreteATNToken}");
#endif
                                _lexer.Recovery(ConvertToATNToken(targetDirective.ConcreteATNToken));

                                var parser = targetDirective.CreateParser(this);

                                parser.SetStateAsInt32(targetDirective.State.Value);

                                SetParser(parser);
                            }
                            break;

                        case KindOfParsingDirective.ReturnToParent:
                            {
#if DEBUG
                                //_logger.Log($"targetDirective.ConcreteATNToken = {targetDirective.ConcreteATNToken}");
#endif
                                if(targetDirective.ConcreteATNToken != null)
                                {
                                    _lexer.Recovery(ConvertToATNToken(targetDirective.ConcreteATNToken));
                                }

                                SetPrevParser();

                                if(_currentParser == null)
                                {
                                    var result = new ParsingResult()
                                    {
                                        IsSuccess = true,
                                        CountSteps = _sentenceCounter,
                                        Phrase = targetDirective.Phrase
                                    };

                                    _sentenceCounter = 0;

#if DEBUG
                                    //_logger.Log($"result = {result}");
                                    //_logger.Log($"result.Phrase = {result.Phrase.ToDbgString()}");
#endif

                                    _result.Results.Add(result);
                                }
                                else
                                {
#if DEBUG
                                    //_logger.Log($"_currentParser.StateAfterRunChild = {_currentParser.StateAfterRunChild}");
#endif

                                    if (_currentParser.StateAfterRunChild.HasValue)
                                    {
                                        _currentParser.SetStateAsInt32(_currentParser.StateAfterRunChild.Value);

                                        _currentParser.StateAfterRunChild = null;
                                    }

                                    _currentParser.OnReceiveReturn(targetDirective.Phrase);
                                }
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfParsingDirective), kindOfParsingDirective, null);
                    }
                }

                return;
            }

            throw new NotImplementedException();
        }

        private void SetParser(BaseParser parser)
        {
#if DEBUG
            //_logger.Log($"Begin");
#endif

            if (_currentParser != null)
            {
                _parsers.Push(_currentParser);
            }

            _currentParser = parser;

            _currentParser.OnEnter();

#if DEBUG
            //_logger.Log($"End");
#endif
        }

        private void SetPrevParser()
        {
#if DEBUG
            //_logger.Log($"Begin");
            //_logger.Log($"_parsers.Count = {_parsers.Count}");
#endif

            if (!_parsers.Any())
            {
                _currentParser = null;
                return;
            }

            _currentParser = _parsers.Pop();

#if DEBUG
            //_logger.Log($"End");
#endif
        }

        public bool IsActive => _isActive;

        private ATNToken _currentToken;
        private ConcreteATNToken _concreteATNToken;

        private WholeTextParsingResult _result = new WholeTextParsingResult();

        public void Run()
        {
#if DEBUG
            //_logger.Log($"Begin");
#endif

            try
            {
                if(_currentParser == null && _lexer.HasToken())
                {
                    SetParser(new RunCurrTokenDirective<SentenceParser>(SentenceParser.State.Init));
                    return;
                }

                var expectedBehavior = _currentParser.ExpectedBehavior;

#if DEBUG
                //_logger.Log($"expectedBehavior = {expectedBehavior}");
#endif

                switch (expectedBehavior)
                {
                    case ExpectedBehaviorOfParser.WaitForCurrToken:
                        {
                            _currentToken = _lexer.GetToken();

#if DEBUG
                            //_logger.Log($"_currentToken = {_currentToken}");
#endif
                            
                            if (_currentToken == null)
                            {
                                _currentParser.OnEmptyLexer();

                                NExit();
                                break;
                            }

                            _globalCounter++;
                            _sentenceCounter++;

#if DEBUG
                            //_logger.Log($"_globalCounter = {_globalCounter}");
                            //_logger.Log($"_sentenceCounter = {_sentenceCounter}");
#endif

                            _currentParser.OnRun(_currentToken);
                        }
                        break;

                    case ExpectedBehaviorOfParser.WaitForVariant:
                        {
                            _currentParser.OnVariant(_concreteATNToken);
                            _concreteATNToken = null;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(expectedBehavior), expectedBehavior, null);
                }
            }
            catch (FailStepInPhraseException e)
            {
                Log(e.ToString());

                _result.IsSuccess = false;

                NExit();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());

                _result.IsSuccess = false;
                _result.Error = ex;

                NExit();
            }

#if DEBUG
            //_logger.Log($"End");
#endif
        }

        private void NExit()
        {
#if DEBUG
            //Log($"Begin");
#endif

            _isActive = false;

            _result.CountSteps = _globalCounter;

            _globalContext.AddResult(_result);
            _globalContext.RemoveContext(this);

#if DEBUG
            //_logger.Log($"End");
#endif
        }

        private ATNToken ConvertToATNToken(ConcreteATNToken token)
        {
#if DEBUG
            //_logger.Log($"token = {token}");
#endif

            return new ATNToken()
            {
                Kind = token.Kind,
                Content = token.Content,
                Pos = token.Pos,
                Line = token.Line,
                WordFrames = new List<BaseGrammaticalWordFrame>
                {
                    token.WordFrame
                }
            };
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
