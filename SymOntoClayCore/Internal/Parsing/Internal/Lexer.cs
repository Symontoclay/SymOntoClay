using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class Lexer
    {
        private enum State
        {
            Init,
            InWord,
            InString,
            InIdentifier
        }

        public Lexer()
        {
        }

        public Lexer(string text, ILogger logger)
        {
            _logger = logger;
            _items = new Queue<char>(text.ToList());
        }

        private ILogger _logger;
        private Queue<char> _items;
        private State _state = State.Init;

        private Queue<Token> _recoveriesTokens = new Queue<Token>();

        private int _currentPos;
        private int _currentLine = 1;

        public Token GetToken()
        {
            if (_recoveriesTokens.Count > 0)
            {
                return _recoveriesTokens.Dequeue();
            }

            StringBuilder tmpBuffer = null;

            while (_items.Count > 0)
            {
                var tmpChar = _items.Dequeue();

                _currentPos++;

#if DEBUG
                //_logger.Log($"tmpChar = {tmpChar}");
                //_logger.Log($"_currentPos = {_currentPos}");
#endif

                switch (_state)
                {
                    case State.Init:
                        if (char.IsLetterOrDigit(tmpChar))
                        {
                            tmpBuffer = new StringBuilder();
                            tmpBuffer.Append(tmpChar);

                            _state = State.InWord;
                            break;
                        }

                        switch (tmpChar)
                        {
                            case ' ':
                                break;

                            case '{':
                                return CreateToken(TokenKind.OpenFigureBracket);

                            case '}':
                                return CreateToken(TokenKind.CloseFigureBracket);

                            case ';':
                                return CreateToken(TokenKind.Semicolon);

                            default:
                                {
                                    var intCharCode = (int)tmpChar;

                                    if (intCharCode == 13)
                                    {
                                        break;
                                    }

                                    if (intCharCode == 10)
                                    {
                                        _currentPos = 0;
                                        _currentLine++;
                                        break;
                                    }

                                    throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }
                        }
                        break;

                    case State.InWord:
                        {
                            tmpBuffer.Append(tmpChar);

                            if (_items.Count == 0)
                            {
                                _state = State.Init;
                                return CreateToken(TokenKind.Word, tmpBuffer.ToString());
                            }

                            var tmpNextChar = _items.Peek();

                            if (!char.IsLetterOrDigit(tmpNextChar) && tmpNextChar != '_')
                            {
                                _state = State.Init;
                                return CreateToken(TokenKind.Word, tmpBuffer.ToString());
                            }
                        }
                        break;

                    case State.InString:
                        throw new NotImplementedException();

                    case State.InIdentifier:
                        throw new NotImplementedException();

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
                }
            }

            return null;
        }

        private Token CreateToken(TokenKind kind, string content = "")
        {
#if DEBUG
            //_logger.Log($"kind = {kind}");
            //_logger.Log($"content = '{content}'");
#endif

            var kindOfKeyWord = KeyWordTokenKind.Unknown;
            var contentLength = 0; 

            switch (kind)
            {
                case TokenKind.Word:
                    {
                        contentLength = content.Length - 1;

                        if (content.All(p => char.IsDigit(p)))
                        {
                            kind = TokenKind.Number;
                            break;
                        }

                        if (string.Compare(content, "app", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.App;
                            break;
                        }
                    }
                    break;
            }

#if DEBUG
            //_logger.Log($"kind (2) = {kind}");
            //_logger.Log($"kindOfKeyWord = {kindOfKeyWord}");
            //_logger.Log($"contentLength = {contentLength}");
#endif

            var result = new Token();
            result.TokenKind = kind;
            result.KeyWordTokenKind = kindOfKeyWord;
            result.Content = content;
            result.Pos = _currentPos - contentLength;
            result.Line = _currentLine;

            return result;
        }

        public void Recovery(Token token)
        {
            _recoveriesTokens.Enqueue(token);
        }

        /// <summary>
        /// Number of remaining characters.
        /// </summary>
        public int Count
        {
            get
            {
                return _recoveriesTokens.Count + _items.Count;
            }
        }

        public Lexer Fork()
        {
            var result = new Lexer();
            result._logger = _logger;
            result._items = new Queue<char>(_items);
            result._recoveriesTokens = new Queue<Token>(_recoveriesTokens);
            result._state = _state;
            return result;
        }

        public void Assing(Lexer source)
        {
            _items = new Queue<char>(source._items);
            _recoveriesTokens = new Queue<Token>(source._recoveriesTokens);
            _state = source._state;
        }
    }
}
