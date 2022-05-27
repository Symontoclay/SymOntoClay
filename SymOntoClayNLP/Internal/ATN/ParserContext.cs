using SymOntoClay.Core;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ParserContext : IObjectToString
    {
        public ParserContext(GlobalParserContext globalContext, IEntityLogger logger, IWordsDict wordsDict, string text)
        {
            _logger = logger;
            _globalContext = globalContext;

            _contextNum = _globalContext.GetContextNum();

            _dumpToLogDirOnExit = _globalContext.DumpToLogDirOnExit;
            _logDir = _globalContext.LogDir;

            _lexer = new ATNLexer(text, wordsDict);

            _parsers = new Stack<BaseParser>();

            if(_dumpToLogDirOnExit)
            {
                LogToInternal($"text: \"{text}\"");
            }
        }

        private ParserContext(GlobalParserContext globalContext, IEntityLogger logger, int parentNum)
        {
            _globalContext = globalContext;
            _logger = logger;
            _parentNum = parentNum;

            _contextNum = _globalContext.GetContextNum();

            _dumpToLogDirOnExit = _globalContext.DumpToLogDirOnExit;
            _logDir = _globalContext.LogDir;
        }

        private readonly GlobalParserContext _globalContext;
        private ATNLexer _lexer;
        private Stack<BaseParser> _parsers;
        private BaseParser _currentParser;
        private readonly IEntityLogger _logger;
        private readonly bool _dumpToLogDirOnExit;
        private readonly int _contextNum;
        private readonly int _parentNum;
        private readonly string _logDir;
        private bool _isActive = true;
        private List<string> _logMessages = new List<string>();

        private int _globalCounter = 0;
        private int _sentenceCounter = 0;

        private ParserContext Fork()
        {
#if DEBUG
            //_logger.Log($"Begin");
#endif

            var newContext = new ParserContext(_globalContext, _logger, _contextNum);
            newContext._lexer = _lexer.Fork();

            newContext._parsers = new Stack<BaseParser>(_parsers.Select(p => p.Fork(newContext)).ToList());

            newContext._currentParser = _currentParser.Fork(newContext);

            newContext._logMessages = _logMessages.ToList();
            newContext._isActive = _isActive;
            newContext._globalCounter = _globalCounter;
            newContext._sentenceCounter = _sentenceCounter;

#if DEBUG
            //_logger.Log($"_currentToken = {_currentToken}");
#endif

            newContext._currentToken = _currentToken;

#if DEBUG
            //_logger.Log($"_concreteATNToken = {_concreteATNToken}");
#endif

            newContext._concreteATNToken = _concreteATNToken;

            _globalContext.AddContext(newContext);

            return newContext;
        }

        [MethodForLoggingSupport]
        public void Log(string message)
        {
            LogToInternal(message);

            _logger.Log(message);
        }

        [MethodForLoggingSupport]
        private void LogToInternal(string message, bool addEmptyString = true)
        {
            _logMessages.Add(message);

            if(addEmptyString)
            {
                _logMessages.Add(string.Empty);
            }
        }

        private bool IsNotParsers()
        {
            return !_parsers.Any() && _currentParser == null;
        }

        public void SetParser(params IParsingDirective[] directives)
        {
            if(_parsingDirectives == null)
            {
                _parsingDirectives = new List<IParsingDirective>();
            }

            _parsingDirectives.AddRange(directives);
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

            var prevParserName = _currentParser?.GetParserName() ?? string.Empty;

            _currentParser = parser;

            if(_dumpToLogDirOnExit)
            {
                var logSb = new StringBuilder("Set Parser:");

                if(!string.IsNullOrWhiteSpace(prevParserName))
                {
                    logSb.Append(" ");
                    logSb.Append(prevParserName);
                    logSb.Append(" ->");
                }

                var currentParserName = _currentParser.GetParserName();

                logSb.Append(" ");
                logSb.Append(currentParserName);

                LogToInternal(logSb.ToString());

                LogToInternal($"{currentParserName}: OnEnter");
            }

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

            if (_dumpToLogDirOnExit)
            {
                throw new NotImplementedException();
            }

#if DEBUG
            //_logger.Log($"End");
#endif
        }

        public bool IsActive => _isActive;

        private ATNToken _currentToken;
        private ConcreteATNToken _concreteATNToken;
        private List<IParsingDirective> _parsingDirectives;

        private WholeTextParsingResult _result = new WholeTextParsingResult();

        public void Run()
        {
#if DEBUG
            _logger.Log($"Begin");
#endif

            try
            {
                if (!_parsingDirectives.IsNullOrEmpty())
                {
                    ProcessParsingDirectives();
                    return;
                }

                if (_currentParser == null && _lexer.HasToken())
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
                                if (_dumpToLogDirOnExit)
                                {
                                    throw new NotImplementedException();
                                }

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

                            if (_dumpToLogDirOnExit)
                            {
                                var currentParserName = _currentParser.GetParserName();

                                var logSb = new StringBuilder($"{currentParserName} OnRun: state = ");
                                logSb.Append(_currentParser.GetStateAsString());
                                LogToInternal(logSb.ToString());

                                logSb = new StringBuilder($"{currentParserName} OnRun: token = ");
                                logSb.Append(_currentToken);
                                LogToInternal(logSb.ToString());
                            }

                            _currentParser.OnRun(_currentToken);
                        }
                        break;

                    case ExpectedBehaviorOfParser.WaitForVariant:
                        {
#if DEBUG
                            _logger.Log($"_concreteATNToken = {_concreteATNToken}");
#endif

                            if (_dumpToLogDirOnExit)
                            {
                                var currentParserName = _currentParser.GetParserName();

                                var logSb = new StringBuilder($"{currentParserName} OnVariant: state = ");
                                logSb.Append(_currentParser.GetStateAsString());
                                LogToInternal(logSb.ToString());

                                logSb = new StringBuilder($"{currentParserName} OnVariant: token = ");
                                logSb.Append(_concreteATNToken);
                                LogToInternal(logSb.ToString());
                            }

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

        private void ProcessParsingDirectives()
        {
            var directives = _parsingDirectives.ToList();

            _parsingDirectives.Clear();

#if DEBUG
            _logger.Log($"directives = {directives.WriteListToString()}");
#endif

            if(directives.Count == 1)
            {
                ProcessParsingDirective(directives.Single());
                return;
            }

            var directiveForCurrentContext = directives.First();

            directives.Remove(directiveForCurrentContext);

#if DEBUG
            _logger.Log($"directiveForCurrentContext = {directiveForCurrentContext}");
            _logger.Log($"directives (2) = {directives.WriteListToString()}");
#endif

            var newParserContextsList = new List<ParserContext>();

            foreach (var directive in directives)
            {
#if DEBUG
                _logger.Log($"directive = {directive}");
#endif

                var newParserContext = Fork();

                newParserContext.LogToInternal($"Forked {newParserContext._contextNum} from {_contextNum}");

                newParserContextsList.Add(newParserContext);

                newParserContext.ProcessParsingDirective(directive);
            }

            if (_dumpToLogDirOnExit && newParserContextsList.Any())
            {
                foreach(var newParserContext in newParserContextsList)
                {
                    LogToInternal($"Forking {newParserContext._contextNum} from {_contextNum}");
                }
            }

            ProcessParsingDirective(directiveForCurrentContext);
        }

        private void ProcessParsingDirective(IParsingDirective directive)
        {
#if DEBUG
            _logger.Log($"directive = {directive}");
#endif

            if (IsNotParsers())
            {
                var parser = directive.CreateParser(this);

                parser.SetStateAsInt32(directive.State.Value);

                SetParser(parser);

                return;
            }

#if DEBUG
            _logger.Log($"directive.ParserType?.FullName = {directive.ParserType?.FullName}");
            _logger.Log($"_currentParser.GetType().FullName = {_currentParser.GetType().FullName}");
#endif

            if (directive.ParserType == _currentParser.GetType())
            {
                _currentParser.SetStateAsInt32(directive.State.Value);

                var kindOfParsingDirective = directive.KindOfParsingDirective;

                switch (kindOfParsingDirective)
                {
                    case KindOfParsingDirective.RunVariant:
                        _currentParser.ExpectedBehavior = ExpectedBehaviorOfParser.WaitForVariant;
                        _concreteATNToken = directive.ConcreteATNToken;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParsingDirective), kindOfParsingDirective, null);
                }
            }
            else
            {
                var kindOfParsingDirective = directive.KindOfParsingDirective;

#if DEBUG
                _logger.Log($"kindOfParsingDirective = {kindOfParsingDirective}");
#endif

                switch (kindOfParsingDirective)
                {
                    case KindOfParsingDirective.RunChild:
                        {
                            _currentParser.StateAfterRunChild = directive.StateAfterRunChild;
                            _currentParser.ExpectedBehavior = ExpectedBehaviorOfParser.WaitForReceiveReturn;

#if DEBUG
                            //_logger.Log($"directive.ConcreteATNToken = {directive.ConcreteATNToken}");
#endif
                            _lexer.Recovery(ConvertToATNToken(directive.ConcreteATNToken));

                            var parser = directive.CreateParser(this);

                            parser.SetStateAsInt32(directive.State.Value);

                            SetParser(parser);
                        }
                        break;

                    case KindOfParsingDirective.ReturnToParent:
                        {
#if DEBUG
                            //_logger.Log($"directive.ConcreteATNToken = {directive.ConcreteATNToken}");
#endif
                            if (directive.ConcreteATNToken != null)
                            {
                                _lexer.Recovery(ConvertToATNToken(directive.ConcreteATNToken));
                            }

                            SetPrevParser();

                            if (_currentParser == null)
                            {
                                var result = new ParsingResult()
                                {
                                    IsSuccess = true,
                                    CountSteps = _sentenceCounter,
                                    Phrase = directive.Phrase
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

                                if (_dumpToLogDirOnExit)
                                {
                                    throw new NotImplementedException();
                                }

                                _currentParser.OnReceiveReturn(directive.Phrase);
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParsingDirective), kindOfParsingDirective, null);
                }
            }
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

            if(_dumpToLogDirOnExit)
            {
                var currentParserName = _currentParser?.GetParserName();

                var logSb = new StringBuilder();

                if(!string.IsNullOrWhiteSpace(currentParserName))
                {
                    logSb.Append($"{_currentParser.GetParserName()}: ");
                }

                logSb.Append("Exit");

                LogToInternal(logSb.ToString());

                var sb = new StringBuilder();

                sb.Append(_contextNum.ToString());

                if(_parentNum > 0)
                {
                    sb.Append($" : {_parentNum}");
                }

                sb.AppendLine();
                sb.AppendLine();

                foreach(var msg in _logMessages)
                {
                    sb.AppendLine(msg);
                }

                File.WriteAllText(Path.Combine(_logDir, $"{_contextNum}.log"), sb.ToString());
            }

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
