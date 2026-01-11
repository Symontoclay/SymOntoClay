/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ParserContext : IObjectToString
    {
        public ParserContext(GlobalParserContext globalContext, IMonitorLogger logger, IWordsDict wordsDict, string text)
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

        private ParserContext(GlobalParserContext globalContext, IMonitorLogger logger, int parentNum)
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
        private readonly IMonitorLogger _logger;
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
            var newContext = new ParserContext(_globalContext, _logger, _contextNum);
            newContext._lexer = _lexer.Fork();

            newContext._parsers = new Stack<BaseParser>(_parsers.Select(p => p.Fork(newContext)).Reverse().ToList());

#if DEBUG

#endif

            newContext._currentParser = _currentParser.Fork(newContext);

            newContext._logMessages = _logMessages.ToList();
            newContext._isActive = _isActive;
            newContext._globalCounter = _globalCounter;
            newContext._sentenceCounter = _sentenceCounter;

            newContext._currentToken = _currentToken;

            newContext._concreteATNToken = _concreteATNToken;

            _globalContext.AddContext(newContext);

            return newContext;
        }

        [MethodForLoggingSupport]
        public void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Trace(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Debug(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Warn(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Error(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(exception.ToString());

            _logger.Error(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(message);

            _logger.Fatal(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToInternal(exception.ToString());

            _logger.Fatal(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        [Obsolete("", true)]
        public void Log(string message)
        {
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

        }

        private void SetPrevParser()
        {
            if (!_parsers.Any())
            {
                _currentParser = null;

                if (_dumpToLogDirOnExit)
                {
                    LogToInternal("All parsers has been finished");
                }

                return;
            }

            var prevParserName = _currentParser.GetParserName();

            _currentParser = _parsers.Pop();

            if (_dumpToLogDirOnExit)
            {
                var logSb = new StringBuilder("Back to prev Parser:");

                logSb.Append(" ");
                logSb.Append(prevParserName);
                logSb.Append(" ->");

                var currentParserName = _currentParser.GetParserName();

                logSb.Append(" ");
                logSb.Append(currentParserName);

                LogToInternal(logSb.ToString());
            }

        }

        public bool IsActive => _isActive;

        private ATNToken _currentToken;
        private ConcreteATNToken _concreteATNToken;
        private List<IParsingDirective> _parsingDirectives;

        private WholeTextParsingResult _result = new WholeTextParsingResult();

        public void Run()
        {
            try
            {
                if (!_parsingDirectives.IsNullOrEmpty())
                {
                    ProcessParsingDirectives();
                    return;
                }

                if (_currentParser == null)
                {
                    if (_lexer.HasToken())
                    {
                        SetParser(new RunCurrTokenDirective<SentenceParser>(SentenceParser.State.Init));
                    }
                    else
                    {
                        NExit();
                    }

                    return;
                }

                var expectedBehavior = _currentParser.ExpectedBehavior;

                switch (expectedBehavior)
                {
                    case ExpectedBehaviorOfParser.WaitForCurrToken:
                        {
                            _currentToken = _lexer.GetToken();

                            if (_currentToken == null)
                            {
                                if (_dumpToLogDirOnExit)
                                {
                                    LogToInternal($"{_currentParser.GetParserName()} OnEmptyLexer");
                                }

                                _currentParser.OnEmptyLexer();
                                break;
                            }

                            _globalCounter++;
                            _sentenceCounter++;

                            if (_dumpToLogDirOnExit)
                            {
                                var currentParserName = _currentParser.GetParserName();

                                var logSb = new StringBuilder($"{currentParserName} OnRun: state = ");
                                logSb.Append(_currentParser.GetStateAsString());
                                LogToInternal(logSb.ToString());

                                var roleName = _currentParser.GetRoleAsString();

                                if(!string.IsNullOrWhiteSpace(roleName))
                                {
                                    LogToInternal($"{currentParserName} OnRun: role = {roleName}");
                                }

                                logSb = new StringBuilder($"{currentParserName} OnRun: token = ");
                                logSb.Append(_currentToken);
                                LogToInternal(logSb.ToString());
                            }

                            _currentParser.OnRun(_currentToken);
                        }
                        break;

                    case ExpectedBehaviorOfParser.WaitForVariant:
                        {
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

                            if (_dumpToLogDirOnExit)
                            {
                                LogToInternal(_currentParser.GetPhraseAsString());
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(expectedBehavior), expectedBehavior, null);
                }
            }
            catch (FailStepInPhraseException e)
            {
                if(_dumpToLogDirOnExit)
                {
                    LogToInternal(e.ToString());
                }                

                _result.IsSuccess = false;

                NExit();
            }
            catch (Exception ex)
            {
                Error("94075437-62F1-46BA-B1AA-E8BFB0BB7961", ex);

                _result.IsSuccess = false;
                _result.Error = ex;

                NExit();
            }

        }

        private void ProcessParsingDirectives()
        {
            var directives = _parsingDirectives.ToList();

            _parsingDirectives.Clear();

            if(directives.Count == 1)
            {
                ProcessParsingDirective(directives.Single());
                return;
            }

            var directiveForCurrentContext = directives.First();

            directives.Remove(directiveForCurrentContext);

            var newParserContextsList = new List<ParserContext>();

            foreach (var directive in directives)
            {
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
            if (IsNotParsers())
            {
                var parser = directive.CreateParser(this);

                parser.SetStateAsInt32(directive.State.Value);

                SetParser(parser);

                return;
            }

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

                switch (kindOfParsingDirective)
                {
                    case KindOfParsingDirective.RunChild:
                        {
                            _currentParser.StateAfterRunChild = directive.StateAfterRunChild;
                            _currentParser.ExpectedBehavior = ExpectedBehaviorOfParser.WaitForReceiveReturn;

                            _lexer.Recovery(ConvertToATNToken(directive.ConcreteATNToken));

                            var parser = directive.CreateParser(this);

                            parser.SetStateAsInt32(directive.State.Value);

                            SetParser(parser);
                        }
                        break;

                    case KindOfParsingDirective.ReturnToParent:
                        {
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

                                if (_dumpToLogDirOnExit)
                                {
                                    var logSb = new StringBuilder();
                                    logSb.AppendLine("Put result:");
                                    logSb.Append(result.ToDbgString());
                                    LogToInternal(logSb.ToString());
                                }

                                _result.Results.Add(result);
                            }
                            else
                            {
                                if (_currentParser.StateAfterRunChild.HasValue)
                                {
                                    _currentParser.SetStateAsInt32(_currentParser.StateAfterRunChild.Value);

                                    _currentParser.StateAfterRunChild = null;
                                }

                                if (_dumpToLogDirOnExit)
                                {
                                    var logSb = new StringBuilder($"{_currentParser.GetParserName()} OnReceiveReturn: phrase = ");
                                    logSb.Append(directive.Phrase?.ToDbgString());
                                    LogToInternal(logSb.ToString());
                                }

                                _currentParser.OnReceiveReturn(directive.Phrase);

                                if (_dumpToLogDirOnExit)
                                {
                                    LogToInternal(_currentParser.GetPhraseAsString());
                                }
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
            _isActive = false;

            _result.CountSteps = _globalCounter;

            _globalContext.AddResult(_result);

            if (_dumpToLogDirOnExit)
            {
                var logSb = new StringBuilder();
                logSb.AppendLine("Put final result to global context:");
                logSb.Append(_result.ToDbgString());
                LogToInternal(logSb.ToString());
            }

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

        }

        private ATNToken ConvertToATNToken(ConcreteATNToken token)
        {
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
