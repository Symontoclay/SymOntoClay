using NLog.LayoutRenderers.Wrappers;
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
            At,
            Sharp,
            DollarSign,
            InQuestionVar
        }

        private enum KindOfPrefix
        {
            Unknown,
            Var,
            SystemVar,
            LogicalVar,
            QuestionVar,
            Channel,
            Entity,
            EntityCondition,
            EntityRefByConcept
        }

        private Lexer()
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
                //_logger.Log($"buffer?.ToString() = {buffer?.ToString()}");
#endif

                switch (_state)
                {
                    case State.Init:
                        _kindOfPrefix = KindOfPrefix.Unknown;

                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
                            var nextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"nextChar = {nextChar}");
#endif

                            buffer = new StringBuilder();
                            buffer.Append(tmpChar);

                            if (nextChar == '.' || !char.IsLetterOrDigit(nextChar))
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

                            case ':':
                                return CreateToken(TokenKind.Colon);

                            case ';':
                                return CreateToken(TokenKind.Semicolon);

                            case '.':
                                return CreateToken(TokenKind.Point);

                            case '?':
                                {
                                    var nextChar = _items.Peek();

#if DEBUG
                                    _logger.Log($"nextChar = {nextChar}");
#endif

                                    if(char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                    {
                                        _state = State.InQuestionVar;

                                        buffer = new StringBuilder();
                                        buffer.Append(tmpChar);

                                        break;
                                    }

                                    throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }                                

                            case ',':
                                return CreateToken(TokenKind.Comma);

                            case '~':
                                return CreateToken(TokenKind.AsyncMarker);

                            case '-':
                                {
                                    var nextChar = _items.Peek();

#if DEBUG
                                    //_logger.Log($"nextChar = {nextChar}");
#endif

                                    switch(nextChar)
                                    {
                                        case '>':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.LeftRightArrow);

                                        default:
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                    }
                                }

                            case '=':
                                return CreateToken(TokenKind.Assign);

                            case '>':
                                return CreateToken(TokenKind.More);

                            case '&':
                                return CreateToken(TokenKind.And);

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

                            case '`':
                                _state = State.InIdentifier;
                                buffer = new StringBuilder();
                                break;

                            case '@':
                                buffer = new StringBuilder();
                                buffer.Append(tmpChar);
                                _state = State.At;
                                break;

                            case '#':
                                buffer = new StringBuilder();
                                buffer.Append(tmpChar);
                                _state = State.Sharp;
                                break;

                            case '$':
                                buffer = new StringBuilder();
                                buffer.Append(tmpChar);
                                _state = State.DollarSign;
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

#if DEBUG
                            //_logger.Log($"case State.InWord: buffer?.ToString() = {buffer?.ToString()}");
#endif

                            if (_items.Count == 0)
                            {
                                _state = State.Init;

                                switch(_kindOfPrefix)
                                {
                                    case KindOfPrefix.Unknown:
                                        return CreateToken(TokenKind.Word, buffer.ToString());

                                    case KindOfPrefix.Channel:
                                        return CreateToken(TokenKind.Channel, buffer.ToString());

                                    case KindOfPrefix.EntityCondition:
                                        return CreateToken(TokenKind.EntityCondition, buffer.ToString());

                                    default:
                                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }
                            }

                            var nextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"nextChar = {nextChar}");
#endif

                            if (!char.IsLetterOrDigit(nextChar) && nextChar != '_')
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

                                    case KindOfPrefix.EntityCondition:
                                        return CreateToken(TokenKind.EntityCondition, buffer.ToString());

                                    case KindOfPrefix.Entity:
                                        return CreateToken(TokenKind.Entity, buffer.ToString());

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
                        {
                            if (tmpChar == '`')
                            {
                                _state = State.Init;
                                return CreateToken(TokenKind.Identifier, buffer.ToString());
                            }
                            else
                            {
                                buffer.Append(tmpChar);
                            }
                        }
                        break;

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
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                        }
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
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                        }
                                    }
                                }
                                break;

                            default:
                                if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.Var;

                                    var nextChar = _items.Peek();

                                    if (nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                        }
                                    }
                                    break;
                                }
                                throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                        }
                        break;

                    case State.Sharp:
                        switch (tmpChar)
                        {
                            case '@':
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.EntityCondition;

                                    var nextChar = _items.Peek();

#if DEBUG
                                    //_logger.Log($"nextChar = {nextChar}");
#endif

                                    if (nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            _state = State.Init;

                                            return CreateToken(TokenKind.EntityCondition, buffer.ToString());
                                        }                                         
                                    }
                                }
                                break;

                            case '#':
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.EntityRefByConcept;

                                    var nextChar = _items.Peek();

                                    if (nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                        }
                                    }
                                }
                                break;

                            default:
                                if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                                {
                                    buffer.Append(tmpChar);

                                    _kindOfPrefix = KindOfPrefix.Entity;

                                    var nextChar = _items.Peek();

                                    if (nextChar == '`')
                                    {
                                        _state = State.InIdentifier;
                                    }
                                    else
                                    {
                                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                        {
                                            _state = State.InWord;
                                        }
                                        else
                                        {
                                            throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                        }
                                    }
                                    break;
                                }
                                throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                        }
                        break;

                    case State.DollarSign:
                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
                            buffer.Append(tmpChar);

#if DEBUG
                            //_logger.Log($"case State.DollarSign: buffer?.ToString() = {buffer?.ToString()}");
#endif

                            _kindOfPrefix = KindOfPrefix.LogicalVar;

                            var nextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"nextChar = {nextChar}");
#endif

                            if (nextChar == '`')
                            {
                                _state = State.InIdentifier;
                            }
                            else
                            {
                                if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                {
                                    _state = State.InWord;
                                }
                                else
                                {
                                    _state = State.Init;

                                    return CreateToken(TokenKind.LogicalVar, buffer.ToString());
                                }
                            }
                            break;
                        }
                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);

                    case State.InQuestionVar:
                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
                            buffer.Append(tmpChar);

#if DEBUG
                            //_logger.Log($"case State.DollarSign: buffer?.ToString() = {buffer?.ToString()}");
#endif

                            _kindOfPrefix = KindOfPrefix.QuestionVar;

                            var nextChar = _items.Peek();

#if DEBUG
                            //_logger.Log($"nextChar = {nextChar}");
#endif

                            if (nextChar == '`')
                            {
                                _state = State.InIdentifier;
                            }
                            else
                            {
                                if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                {
                                    _state = State.InWord;
                                }
                                else
                                {
                                    _state = State.Init;

                                    return CreateToken(TokenKind.QuestionVar, buffer.ToString());
                                }
                            }
                            break;
                        }
                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);

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

                            case ':':
                                _items.Dequeue();
                                content = ">:";
                                kind = TokenKind.PrimaryLogicalPartMark;
                                break;
                        }
                    }
                    break;

                case TokenKind.OpenFigureBracket:
                    {
                        var nextChar = _items.Peek();

                        switch(nextChar)
                        {
                            case ':':
                                _items.Dequeue();
                                content = "{:";
                                kind = TokenKind.OpenFactBracket;
                                break;
                        }
                    }
                    break;

                case TokenKind.Colon:
                    {
                        var nextChar = _items.Peek();

                        switch (nextChar)
                        {
                            case '}':
                                _items.Dequeue();
                                content = ":}";
                                kind = TokenKind.CloseFactBracket;
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
