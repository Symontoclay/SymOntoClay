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
            InIdentifier,
            At
        }

        private enum KindOfPrefix
        {
            Unknown,
            Var,
            SystemVar,
            LogicalVar,
            Channel,
            Entity,
            EntityCondition
        }

        public Lexer()
        {
        }

        public Lexer(string text, IEntityLogger logger)
        {
            _logger = logger;
            _items = new Queue<char>(text.ToList());
        }

        private IEntityLogger _logger;
        private Queue<char> _items;
        private State _state = State.Init;

        private Queue<Token> _recoveriesTokens = new Queue<Token>();

        private int _currentPos;
        private int _currentLine = 1;

        private char _closeBracket;
        private KindOfPrefix _kindOfPrefix = KindOfPrefix.Unknown;

        public Token GetToken()
        {
            if (_recoveriesTokens.Count > 0)
            {
                return _recoveriesTokens.Dequeue();
            }

            StringBuilder buffer = null;

            while (_items.Count > 0)
            {
                var tmpChar = _items.Dequeue();

                _currentPos++;

#if DEBUG
                //_logger.Log($"tmpChar = {tmpChar}");
                //_logger.Log($"(int)tmpChar = {(int)tmpChar}");
                //_logger.Log($"_currentPos = {_currentPos}");
                //_logger.Log($"_state = {_state}");
                //_logger.Log($"tmpBuffer?.ToString() = {buffer?.ToString()}");
#endif

                switch (_state)
                {
                    case State.Init:
                        _kindOfPrefix = KindOfPrefix.Unknown;

                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
                            var tmpNextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"tmpNextChar = {tmpNextChar}");
#endif

                            buffer = new StringBuilder();
                            buffer.Append(tmpChar);

                            if (tmpNextChar == '.' || !char.IsLetterOrDigit(tmpNextChar))
                            {
                                return CreateToken(TokenKind.Word, buffer.ToString());
                            }
                            else
                            {
                                _state = State.InWord;
                            }                       
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

                            case '[':
                                return CreateToken(TokenKind.OpenSquareBracket);

                            case ']':
                                return CreateToken(TokenKind.CloseSquareBracket);

                            case '(':
                                return CreateToken(TokenKind.OpenRoundBracket);

                            case ')':
                                return CreateToken(TokenKind.CloseRoundBracket);

                            case ';':
                                return CreateToken(TokenKind.Semicolon);

                            case '.':
                                return CreateToken(TokenKind.Point);

                            case ',':
                                return CreateToken(TokenKind.Comma);

                            case '=':
                                return CreateToken(TokenKind.Assign);

                            case '>':
                                return CreateToken(TokenKind.More);

                            case '\'':
                                _closeBracket = '\'';
                                _state = State.InString;
                                buffer = new StringBuilder();
                                break;

                            case '"':
                                _closeBracket = '"';
                                _state = State.InString;
                                buffer = new StringBuilder();
                                break;

                            case '@':
                                buffer = new StringBuilder();
                                buffer.Append(tmpChar);
                                _state = State.At;
                                break;

                            default:
                                {
                                    var intCharCode = (int)tmpChar;

                                    switch(intCharCode)
                                    {
                                        case 9:
                                            break;

                                        case 10:
                                            _currentPos = 0;
                                            _currentLine++;
                                            break;

                                        case 13:
                                            break;

                                        default:
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                    }         
                                }
                                break;
                        }
                        break;

                    case State.InWord:
                        {
                            buffer.Append(tmpChar);

                            if (_items.Count == 0)
                            {
                                _state = State.Init;

                                switch(_kindOfPrefix)
                                {
                                    case KindOfPrefix.Unknown:
                                        return CreateToken(TokenKind.Word, buffer.ToString());

                                    case KindOfPrefix.Channel:
                                        return CreateToken(TokenKind.Channel, buffer.ToString());

                                    default:
                                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }
                            }

                            var tmpNextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"tmpNextChar = {tmpNextChar}");
#endif

                            if (!char.IsLetterOrDigit(tmpNextChar) && tmpNextChar != '_')
                            {
                                _state = State.Init;

                                switch (_kindOfPrefix)
                                {
                                    case KindOfPrefix.Unknown:
                                        return CreateToken(TokenKind.Word, buffer.ToString());

                                    case KindOfPrefix.Channel:
                                        return CreateToken(TokenKind.Channel, buffer.ToString());

                                    case KindOfPrefix.SystemVar:
                                        return CreateToken(TokenKind.SystemVar, buffer.ToString());

                                    default:
                                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }
                            }
                        }
                        break;

                    case State.InString:
                        {
                            if(tmpChar == _closeBracket)
                            {
                                _state = State.Init;
                                return CreateToken(TokenKind.String, buffer.ToString());
                            }
                            else
                            {
                                buffer.Append(tmpChar);
                            }
                        }
                        break;

                    case State.InIdentifier:
                        throw new NotImplementedException();

                    case State.At:
                        switch (tmpChar)
                        {
                            case '>':
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.Channel;

                                    var nextChar = _items.Peek();

                                    if(nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        _state = State.InWord;
                                    }
                                }
                                break;

                            case '@':
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.SystemVar;

                                    var nextChar = _items.Peek();

                                    if (nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        _state = State.InWord;
                                    }
                                }
                                break;

                            default:
                                throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                        }
                        break;

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

                        if (string.Compare(content, "class", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Class;
                            break;
                        }

                        if (string.Compare(content, "is", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Is;
                            break;
                        }

                        if (string.Compare(content, "on", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.On;
                            break;
                        }

                        if (string.Compare(content, "init", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Init;
                            break;
                        }

                        if (string.Compare(content, "use", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Use;
                            break;
                        }

                        if (string.Compare(content, "not", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Not;
                            break;
                        }
                    }
                    break;

                case TokenKind.Assign:
                    {
                        var nextChar = _items.Peek();

                        switch(nextChar)
                        {
                            case '>':
                                _items.Dequeue();
                                content = "=>";
                                kind = TokenKind.Lambda;
                                break;
                        }
                    }
                    break;

                case TokenKind.More:
                    {
                        var nextChar = _items.Peek();

                        switch (nextChar)
                        {
                            case '>':
                                _items.Dequeue();
                                content = ">>";
                                kind = TokenKind.LeftRightStream;
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
