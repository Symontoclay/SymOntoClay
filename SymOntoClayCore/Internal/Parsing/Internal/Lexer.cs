/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using NLog.LayoutRenderers.Wrappers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
            InSingleLineComment,
            InMultiLineComment,
            At,
            Sharp,
            DollarSign,
            FactIdPrefix
        }

        private enum KindOfPrefix
        {
            Unknown,
            Var,
            SystemVar,
            LogicalVar,
            Channel,
            Entity,
            EntityCondition,
            EntityRefByConcept
        }

        private Lexer()
        {
        }

        public Lexer(string text, IMonitorLogger logger, LexerMode mode = LexerMode.Code)
        {
            _mode = mode;
            _logger = logger;
            _items = new Queue<char>(text.ToList());
        }

        private IMonitorLogger _logger;
        private LexerMode _mode;
        private Queue<char> _items;
        private State _state = State.Init;
        private State _stateBeforeComment = State.Init;

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
                _logger.Info("63869F33-9EDD-4C0E-AD5C-0A40A2A7D247", $"tmpChar = {tmpChar}");
                _logger.Info("2440A33D-A1F1-45BF-9AEC-2F5B05AF6CF6", $"_currentPos = {_currentPos}");
                _logger.Info("2A1A450C-E1C9-4168-B124-423E1B5CB1AD", $"_state = {_state}");
                _logger.Info("38B84256-148B-407F-AADB-1855BB8F2D9C", $"_kindOfPrefix = {_kindOfPrefix}");
                _logger.Info("88DF13BA-DBD3-4A20-9A61-27413870733B", $"buffer == null = {buffer == null}");
                _logger.Info("A7CD854B-679A-4A04-84A6-C24213E5D47B", $"buffer = {buffer}");
#endif

                switch (_state)
                {
                    case State.Init:
                        _kindOfPrefix = KindOfPrefix.Unknown;

                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
#if DEBUG
                            _logger.Info("3E741E3B-9E26-4DFC-AFAB-8D24863E2951", $"_items.Count = {_items.Count}");
#endif

                            buffer = new StringBuilder();
                            buffer.Append(tmpChar);

                            if (_items.Count == 0)
                            {
                                return CreateToken(TokenKind.Word, buffer.ToString());
                            }

                            var nextChar = _items.Peek();

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
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

#if DEBUG
                                    //_logger.Info("5F68BCEA-D588-484C-B980-3BF16C46F212", $"nextChar = {nextChar}");
#endif

                                    switch (nextChar)
                                    {
                                        case ':':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.DoubleColon);

                                        case '}':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.CloseFactBracket);

                                        case ']':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.CloseAnnotationBracket);

                                        default:
                                            return CreateToken(TokenKind.Colon);
                                    }
                                }

                            case ';':
                                return CreateToken(TokenKind.Semicolon);

                            case '.':
                                return CreateToken(TokenKind.Point);

                            case '?':
                                return CreateToken(TokenKind.QuestionMark);

                            case ',':
                                return CreateToken(TokenKind.Comma);

                            case '~':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '~':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.DoubleAsyncMarker);
                                    }

                                    return CreateToken(TokenKind.AsyncMarker);
                                }                                

                            case '+':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '∞':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.PositiveInfinity);
                                    }

                                    return CreateToken(TokenKind.Plus);
                                }

                            case '-':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '>':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.LeftRightArrow);

                                        case '∞':
                                            _currentPos++;
                                            if(buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.NegativeInfinity);

                                        default:
                                            if(char.IsDigit(nextChar))
                                            {
                                                buffer = new StringBuilder();
                                                buffer.Append(tmpChar);
                                                _state = State.InWord;
                                                break;
                                            }
                                            return CreateToken(TokenKind.Minus);
                                    }
                                }
                                break;

                            case '*':
                                return CreateToken(TokenKind.Multiplication);

                            case '/':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '/':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            _stateBeforeComment = _state;
                                            _state = State.InSingleLineComment;
                                            break;

                                        case '*':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            _stateBeforeComment = _state;
                                            _state = State.InMultiLineComment;
                                            break;

                                        default:
                                            return CreateToken(TokenKind.Division);
                                    }
                                }
                                break;

                            case '=':
                                return CreateToken(TokenKind.Assign);

                            case '>':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch (nextChar)
                                    {
                                        case '=':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.MoreOrEqual);

                                        default:
                                            return CreateToken(TokenKind.More);
                                    }
                                }

                            case '<':
                                {
                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    switch (nextChar)
                                    {
                                        case '=':
                                            _currentPos++;
                                            if (buffer == null)
                                            {
                                                buffer = new StringBuilder();
                                            }
                                            buffer.Append(nextChar);
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.LessOrEqual);

                                        default:
                                            return CreateToken(TokenKind.Less);
                                    }
                                }

                            case '&':
                                return CreateToken(TokenKind.And);

                            case '|':
                                return CreateToken(TokenKind.Or);

                            case '!':
                                return CreateToken(TokenKind.Not);

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
                                switch(_mode)
                                {
                                    case LexerMode.Code:
                                        _state = State.InIdentifier;
                                        buffer = new StringBuilder();
                                        break;

                                    case LexerMode.StrongIdentifier:
                                        return CreateToken(TokenKind.Gravis);

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(_mode), _mode, null);
                                }
                                break;

                            case '@':
                                switch(_mode)
                                {
                                    case LexerMode.Code:
                                        buffer = new StringBuilder();
                                        buffer.Append(tmpChar);
                                        _state = State.At;
                                        break;

                                    case LexerMode.StrongIdentifier:
                                        {
                                            if (_items.Count == 0)
                                            {
                                                d
                                            }

                                            var nextChar = _items.Peek();

#if DEBUG
                                            //_logger.Info("C0BE062A-119E-490C-828F-98E8CD3FB118", $"nextChar = {nextChar}");
#endif

                                            switch (nextChar)
                                            {
                                                case '>':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.ChannelVarPrefix);

                                                case '@':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.SystemVarPrefix);

                                                case ':':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.PropertyPrefix);

                                                default:
                                                    return CreateToken(TokenKind.VarPrefix);
                                            }
                                        }

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(_mode), _mode, null);
                                }
                                break;

                            case '#':
                                switch (_mode)
                                {
                                    case LexerMode.Code:
                                        buffer = new StringBuilder();
                                        buffer.Append(tmpChar);
                                        _state = State.Sharp;
                                        break;

                                    case LexerMode.StrongIdentifier:
                                        {
                                            if (_items.Count == 0)
                                            {
                                                d
                                            }

                                            var nextChar = _items.Peek();

#if DEBUG
                                            //_logger.Info("5B30C864-E243-434D-92EC-F6EF6A4600EC", $"nextChar = {nextChar}");
#endif

                                            switch(nextChar)
                                            {
                                                case '@':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.EntityConditionPrefix);

                                                case '^':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.RuleOrFactIdentifierPrefix);

                                                case '#':
                                                    _items.Dequeue();
                                                    nextChar = _items.Peek();

#if DEBUG
                                                    //_logger.Info("6B80E14B-3DED-4812-8FB8-5A9791B39966", $"nextChar = {nextChar}");
#endif
                                                    switch(nextChar)
                                                    {
                                                        case '@':
                                                            _items.Dequeue();
                                                            return CreateToken(TokenKind.OnceResolvedEntityConditionPrefix);

                                                        default:
                                                            return CreateToken(TokenKind.ConceptPrefix);
                                                    }
                                                    
                                                case '|':
                                                    _items.Dequeue();
                                                    return CreateToken(TokenKind.LinguisticVarPrefix);

                                                default:
                                                    return CreateToken(TokenKind.IdentifierPrefix);
                                            }
                                        }

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(_mode), _mode, null);
                                }
                                break;

                            case '$':
                                switch (_mode)
                                {
                                    case LexerMode.Code:
                                        buffer = new StringBuilder();
                                        buffer.Append(tmpChar);
                                        _state = State.DollarSign;
                                        break;

                                    case LexerMode.StrongIdentifier:
                                        return CreateToken(TokenKind.LogicalVarPrefix);

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(_mode), _mode, null);
                                }
                                break;

                            case '∞':
                                return CreateToken(TokenKind.Infinity);

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

                                    case KindOfPrefix.EntityCondition:
                                        return CreateToken(TokenKind.EntityCondition, buffer.ToString());

                                    default:
                                        throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                }
                            }

                            var nextChar = _items.Peek();

                            if (!char.IsLetterOrDigit(nextChar) && nextChar != '_')
                            {
                                _state = State.Init;

                                switch (_kindOfPrefix)
                                {
                                    case KindOfPrefix.Unknown:
                                        return CreateToken(TokenKind.Word, buffer.ToString());

                                    case KindOfPrefix.Channel:
                                        return CreateToken(TokenKind.Channel, buffer.ToString());

                                    case KindOfPrefix.Var:
                                        return CreateToken(TokenKind.Var, buffer.ToString());

                                    case KindOfPrefix.SystemVar:
                                        return CreateToken(TokenKind.SystemVar, buffer.ToString());

                                    case KindOfPrefix.EntityCondition:
                                        return CreateToken(TokenKind.EntityCondition, buffer.ToString());

                                    case KindOfPrefix.Entity:
                                        return CreateToken(TokenKind.Entity, buffer.ToString());

                                    case KindOfPrefix.LogicalVar:
                                        return CreateToken(TokenKind.LogicalVar, buffer.ToString());

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

                    case State.InSingleLineComment:
                        {
                            var intCharCode = (int)tmpChar;

                            switch (intCharCode)
                            {
                                case 10:
                                    _state = _stateBeforeComment;
                                    _currentPos = 0;
                                    _currentLine++;
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;

                    case State.InMultiLineComment:
                        switch (tmpChar)
                        {
                            case '*':
                                {
                                    if (_items.Count > 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

                                    if(nextChar == '/')
                                    {
                                        _currentPos++;
                                        if (buffer == null)
                                        {
                                            buffer = new StringBuilder();
                                        }
                                        buffer.Append(nextChar);
                                        _items.Dequeue();
                                        _state = _stateBeforeComment;
                                    }
                                }
                                break;

                            default:
                                {
                                    var intCharCode = (int)tmpChar;

                                    if(intCharCode == 10)
                                    {
                                        _currentPos = 0;
                                        _currentLine++;
                                    }
                                }
                                break;
                        }
                        break;

                    case State.At:
                        switch (tmpChar)
                        {
                            case '>':
                                {
                                    buffer.Append(tmpChar);
                                    _kindOfPrefix = KindOfPrefix.Channel;

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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
                                            _state = State.Init;

                                            return CreateToken(TokenKind.Var, buffer.ToString());
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

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

                                    var nextChar = _items.Peek();

#if DEBUG
                                    //_logger.Info("11D91A9D-B8B8-4ECF-B70E-C638E9647910", $"nextChar = {nextChar}");
#endif

                                    switch(nextChar)
                                    {
                                        case '`':
                                            _state = State.InIdentifier;
                                            break;

                                        case '_':
                                            _state = State.InWord;
                                            break;

                                        case '@':
                                            _currentPos++;
                                            buffer.Append(nextChar);
                                            _items.Dequeue();

                                            _state = State.Init;

                                            return CreateToken(TokenKind.OnceEntityCondition, buffer.ToString());

                                        default:
                                            if(char.IsLetterOrDigit(nextChar))
                                            {
                                                _state = State.InWord;
                                            }
                                            else
                                            {
                                                throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                                            }
                                            break;
                                    }
                                }
                                break;

                            case '`':
                                _state = State.InIdentifier;
                                break;

                            case '^':
                                buffer.Append(tmpChar);
                                _state = State.FactIdPrefix;
                                break;

                            default:
                                if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                                {
                                    buffer.Append(tmpChar);

                                    _kindOfPrefix = KindOfPrefix.Entity;
                                    _state = State.InIdentifier;

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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
                                            _state = State.Init;
                                            return CreateToken(TokenKind.Entity, buffer.ToString());
                                        }
                                    }
                                    break;
                                }
                                throw new UnexpectedSymbolException(tmpChar, _currentLine, _currentPos);
                        }
                        break;

                    case State.FactIdPrefix:
                        switch (tmpChar)
                        {
                            case '`':
                                _state = State.InIdentifier;
                                break;

                            default:
                                if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                                {
                                    buffer.Append(tmpChar);

                                    _kindOfPrefix = KindOfPrefix.Entity;
                                    _state = State.InIdentifier;

                                    if (_items.Count == 0)
                                    {
                                        d
                                    }

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
                                            _state = State.Init;
                                            return CreateToken(TokenKind.Entity, buffer.ToString());
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

                            _kindOfPrefix = KindOfPrefix.LogicalVar;

                            if (_items.Count == 0)
                            {
                                d
                            }

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
                                    _state = State.Init;

                                    return CreateToken(TokenKind.LogicalVar, buffer.ToString());
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
            //_logger.Info("C12D5E15-F3BA-4908-893A-43F979C5FC5B", $"kind = {kind}");
            //_logger.Info("79D35261-8C2F-4FF4-88CE-898DA6969C69", $"content = {content}");
#endif

            var kindOfKeyWord = KeyWordTokenKind.Unknown;
            var contentLength = 0;

            switch (kind)
            {
                case TokenKind.Word:
                    {
                        contentLength = content.Length - 1;

                        if (content.All(p => char.IsDigit(p)) || (content.First() == '-' && content.Skip(1).All(p => char.IsDigit(p))))
                        {
                            kind = TokenKind.Number;
                            break;
                        }

                        if (string.Equals(content, "world", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.World;
                            break;
                        }

                        if (string.Equals(content, "app", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.App;
                            break;
                        }

                        if (string.Equals(content, "lib", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Lib;
                            break;
                        }

                        if (string.Equals(content, "class", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Class;
                            break;
                        }

                        if (string.Equals(content, "action", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Action;
                            break;
                        }

                        if (string.Equals(content, "actions", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Actions;
                            break;
                        }

                        if (string.Equals(content, "fun", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Fun;
                            break;
                        }

                        if (string.Equals(content, "ctor", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Constructor;
                            break;
                        }

                        if (string.Equals(content, "op", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Operator;
                            break;
                        }

                        if (string.Equals(content, "operator", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Operator;
                            break;
                        }

                        if (string.Equals(content, "is", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Is;
                            break;
                        }

                        if (string.Equals(content, "on", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.On;
                            break;
                        }

                        if (string.Equals(content, "init", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Init;
                            break;
                        }

                        if (string.Equals(content, "enter", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Enter;
                            break;
                        }

                        if (string.Equals(content, "leave", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Leave;
                            break;
                        }

                        if (string.Equals(content, "set", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Set;
                            break;
                        }                        

                        if (string.Equals(content, "not", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Not;
                            break;
                        }

                        if (string.Equals(content, "and", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.And;
                            break;
                        }

                        if (string.Equals(content, "or", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Or;
                            break;
                        }

                        if (string.Equals(content, "select", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Select;
                            break;
                        }

                        if (string.Equals(content, "insert", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Insert;
                            break;
                        }

                        if (string.Equals(content, "null", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Null;
                            break;
                        }

                        if (string.Equals(content, "linvar", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.LinguisticVariable;
                            break;
                        }

                        if (string.Equals(content, "for", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.For;
                            break;
                        }

                        if (string.Equals(content, "range", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Range;
                            break;
                        }

                        if (string.Equals(content, "terms", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Terms;
                            break;
                        }

                        if (string.Equals(content, "constraints", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Constraints;
                            break;
                        }

                        if (string.Equals(content, "relation", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Relation;
                            break;
                        }

                        if (string.Equals(content, "rel", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Relation;
                            break;
                        }

                        if (string.Equals(content, "inheritance", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Inheritance;
                            break;
                        }

                        if (string.Equals(content, "inh", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Inheritance;
                            break;
                        }

                        if (string.Equals(content, "error", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Error;
                            break;
                        }

                        if (string.Equals(content, "try", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Try;
                            break;
                        }

                        if (string.Equals(content, "catch", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Catch;
                            break;
                        }

                        if (string.Equals(content, "else", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Else;
                            break;
                        }

                        if (string.Equals(content, "ensure", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Ensure;
                            break;
                        }

                        if (string.Equals(content, "where", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Where;
                            break;
                        }

                        if (string.Equals(content, "await", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Await;
                            break;
                        }

                        if (string.Equals(content, "wait", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Wait;
                            break;
                        }

                        if (string.Equals(content, "complete", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Complete;
                            break;
                        }

                        if (string.Equals(content, "completed", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Completed;
                            break;
                        }

                        if (string.Equals(content, "break", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Break;
                            break;
                        }

                        if (string.Equals(content, "alias", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Alias;
                            break;
                        }

                        if (string.Equals(content, "var", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Var;
                            break;
                        }

                        if (string.Equals(content, "public", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Public;
                            break;
                        }

                        if (string.Equals(content, "protected", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Protected;
                            break;
                        }

                        if (string.Equals(content, "private", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Private;
                            break;
                        }

                        if (string.Equals(content, "return", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Return;
                            break;
                        }

                        if (string.Equals(content, "if", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.If;
                            break;
                        }

                        if (string.Equals(content, "elif", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Elif;
                            break;
                        }

                        if (string.Equals(content, "while", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.While;
                            break;
                        }

                        if (string.Equals(content, "continue", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Continue;
                            break;
                        }

                        if (string.Equals(content, "repeat", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Repeat;
                            break;
                        }

                        if (string.Equals(content, "state", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.State;
                            break;
                        }

                        if (string.Equals(content, "states", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.States;
                            break;
                        }

                        if (string.Equals(content, "as", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.As;
                            break;
                        }

                        if (string.Equals(content, "default", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Default;
                            break;
                        }

                        if (string.Equals(content, "down", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Down;
                            break;
                        }

                        if (string.Equals(content, "duration", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Duration;
                            break;
                        }

                        if (string.Equals(content, "_", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.BlankIdentifier;
                            break;
                        }

                        if (string.Equals(content, "add", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Add;
                            break;
                        }

                        if (string.Equals(content, "fact", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Fact;
                            break;
                        }

                        if (string.Equals(content, "reject", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Reject;
                            break;
                        }

                        if (string.Equals(content, "exec", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Exec;
                            break;
                        }

                        if (string.Equals(content, "synonym", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Synonym;
                            break;
                        }

                        if (string.Equals(content, "idle", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Idle;
                            break;
                        }

                        if (string.Equals(content, "with", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.With;
                            break;
                        }

                        if (string.Equals(content, "import", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Import;
                            break;
                        }

                        if (string.Equals(content, "new", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.New;
                            break;
                        }

                        if (string.Equals(content, "weak", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Weak;
                            break;
                        }

                        if (string.Equals(content, "cancel", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Cancel;
                            break;
                        }

                        if (string.Equals(content, "canceled", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Canceled;
                            break;
                        }

                        if (string.Equals(content, "each", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Each;
                            break;
                        }

                        if (string.Equals(content, "once", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Once;
                            break;
                        }

                        if (string.Equals(content, "root", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Root;
                            break;
                        }

                        if (string.Equals(content, "task", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Task;
                            break;
                        }

                        if (string.Equals(content, "compound", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Compound;
                            break;
                        }

                        if (string.Equals(content, "case", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Case;
                            break;
                        }

                        if (string.Equals(content, "primitive", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Primitive;
                            break;
                        }

                        if (string.Equals(content, "strategic", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Strategic;
                            break;
                        }

                        if (string.Equals(content, "tactical", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Tactical;
                            break;
                        }

                        if (string.Equals(content, "prop", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Prop;
                            break;
                        }

                        if (string.Equals(content, "global", StringComparison.OrdinalIgnoreCase))
                        {
                            kindOfKeyWord = KeyWordTokenKind.Global;
                            break;
                        }
                    }
                    break;

                case TokenKind.Identifier:
                    if(content.StartsWith("##"))
                    {
                        throw new NotImplementedException("FBDEBA2D-C686-4AAD-9A11-B8B623280E8E");
                    }
                    else if (content.StartsWith("#@"))
                    {
                        kind = TokenKind.EntityCondition;
                    }
                    else if(content.StartsWith("#"))
                    {
                        kind = TokenKind.Entity;
                    }
                    break;

                case TokenKind.Assign:
                    {
                        if (_items.Count > 0)
                        {
                            d
                        }

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
                        if (_items.Count > 0)
                        {
                            d
                        }

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
                        if (_items.Count > 0)
                        {
                            d
                        }

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

                case TokenKind.OpenSquareBracket:
                    {
                        if (_items.Count > 0)
                        {
                            d
                        }


                        var nextChar = _items.Peek();

                        switch (nextChar)
                        {
                            case ':':
                                _items.Dequeue();
                                content = "[:";
                                kind = TokenKind.OpenAnnotationBracket;
                                break;
                        }
                    }
                    break;

                case TokenKind.Colon:
                    content = ":";
                    break;

                case TokenKind.CloseFactBracket:
                    content = ":}";
                    break;

                case TokenKind.CloseAnnotationBracket:
                    content = ":]";
                    break;

                case TokenKind.DoubleColon:
                    content = "::";
                    break;

                case TokenKind.Gravis:
                    content = "`";
                    break;

                case TokenKind.IdentifierPrefix:
                    content = "#";
                    break;

                case TokenKind.EntityConditionPrefix:
                    content = "#@";
                    break;

                case TokenKind.ConceptPrefix:
                    content = "##";
                    break;

                case TokenKind.OnceResolvedEntityConditionPrefix:
                    content = "##@";
                    break;

                case TokenKind.RuleOrFactIdentifierPrefix:
                    content = "#^";
                    break;

                case TokenKind.LinguisticVarPrefix:
                    content = "#|";
                    break;

                case TokenKind.LogicalVarPrefix:
                    content = "$";
                    break;

                case TokenKind.VarPrefix:
                    content = "@";
                    break;

                case TokenKind.SystemVarPrefix:
                    content = "@@";
                    break;

                case TokenKind.ChannelVarPrefix:
                    content = "@>";
                    break;

                case TokenKind.PropertyPrefix:
                    content = "@:";
                    break;
            }

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
            result._mode = _mode;
            return result;
        }

        public void Assign(Lexer source)
        {
            _items = new Queue<char>(source._items);
            _recoveriesTokens = new Queue<Token>(source._recoveriesTokens);
            _state = source._state;
        }
    }
}
