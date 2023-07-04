/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

        public Lexer(string text, IEntityLogger logger)
        {
            _logger = logger;
            _items = new Queue<char>(text.ToList());
        }

        private IEntityLogger _logger;
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

                switch (_state)
                {
                    case State.Init:
                        _kindOfPrefix = KindOfPrefix.Unknown;

                        if (char.IsLetterOrDigit(tmpChar) || tmpChar == '_')
                        {
                            var nextChar = _items.Peek();

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
                                return CreateToken(TokenKind.QuestionMark);

                            case ',':
                                return CreateToken(TokenKind.Comma);

                            case '~':
                                {
                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '~':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.DoubleAsyncMarker);
                                    }

                                    return CreateToken(TokenKind.AsyncMarker);
                                }                                

                            case '+':
                                {
                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '∞':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.PositiveInfinity);
                                    }

                                    return CreateToken(TokenKind.Plus);
                                }

                            case '-':
                                {
                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '>':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.LeftRightArrow);

                                        case '∞':
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
                                    var nextChar = _items.Peek();

                                    switch(nextChar)
                                    {
                                        case '/':
                                            _items.Dequeue();
                                            _stateBeforeComment = _state;
                                            _state = State.InSingleLineComment;
                                            break;

                                        case '*':
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
                                    var nextChar = _items.Peek();

                                    switch (nextChar)
                                    {
                                        case '=':
                                            _items.Dequeue();
                                            return CreateToken(TokenKind.MoreOrEqual);

                                        default:
                                            return CreateToken(TokenKind.More);
                                    }
                                }

                            case '<':
                                {
                                    var nextChar = _items.Peek();

                                    switch (nextChar)
                                    {
                                        case '=':
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
                                    var nextChar = _items.Peek();

                                    if(nextChar == '/')
                                    {
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
                    }
                    break;

                case TokenKind.Identifier:
                    if(content.StartsWith("##"))
                    {
                        throw new NotImplementedException();
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

                case TokenKind.OpenSquareBracket:
                    {
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
                    {
                        var nextChar = _items.Peek();

                        switch (nextChar)
                        {
                            case '}':
                                _items.Dequeue();
                                content = ":}";
                                kind = TokenKind.CloseFactBracket;
                                break;

                            case ']':
                                _items.Dequeue();
                                content = ":]";
                                kind = TokenKind.CloseAnnotationBracket;
                                break;
                        }
                    }
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
