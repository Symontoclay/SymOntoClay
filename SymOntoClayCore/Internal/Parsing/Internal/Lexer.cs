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

                        if (string.Compare(content, "world", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.World;
                            break;
                        }

                        if (string.Compare(content, "app", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.App;
                            break;
                        }

                        if (string.Compare(content, "lib", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Lib;
                            break;
                        }

                        if (string.Compare(content, "class", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Class;
                            break;
                        }

                        if (string.Compare(content, "action", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Action;
                            break;
                        }

                        if (string.Compare(content, "actions", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Actions;
                            break;
                        }

                        if (string.Compare(content, "fun", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Fun;
                            break;
                        }

                        if (string.Compare(content, "ctor", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Constructor;
                            break;
                        }

                        if (string.Compare(content, "op", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Operator;
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

                        if (string.Compare(content, "enter", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Enter;
                            break;
                        }

                        if (string.Compare(content, "leave", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Leave;
                            break;
                        }

                        if (string.Compare(content, "set", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Set;
                            break;
                        }                        

                        if (string.Compare(content, "not", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Not;
                            break;
                        }

                        if (string.Compare(content, "and", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.And;
                            break;
                        }

                        if (string.Compare(content, "or", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Or;
                            break;
                        }

                        if (string.Compare(content, "select", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Select;
                            break;
                        }

                        if (string.Compare(content, "insert", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Insert;
                            break;
                        }

                        if (string.Compare(content, "null", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Null;
                            break;
                        }

                        if (string.Compare(content, "linvar", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.LinguisticVariable;
                            break;
                        }

                        if (string.Compare(content, "for", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.For;
                            break;
                        }

                        if (string.Compare(content, "range", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Range;
                            break;
                        }

                        if (string.Compare(content, "terms", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Terms;
                            break;
                        }

                        if (string.Compare(content, "constraints", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Constraints;
                            break;
                        }

                        if (string.Compare(content, "relation", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Relation;
                            break;
                        }

                        if (string.Compare(content, "rel", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Relation;
                            break;
                        }

                        if (string.Compare(content, "inheritance", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Inheritance;
                            break;
                        }

                        if (string.Compare(content, "inh", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Inheritance;
                            break;
                        }

                        if (string.Compare(content, "error", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Error;
                            break;
                        }

                        if (string.Compare(content, "try", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Try;
                            break;
                        }

                        if (string.Compare(content, "catch", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Catch;
                            break;
                        }

                        if (string.Compare(content, "else", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Else;
                            break;
                        }

                        if (string.Compare(content, "ensure", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Ensure;
                            break;
                        }

                        if (string.Compare(content, "where", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Where;
                            break;
                        }

                        if (string.Compare(content, "await", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Await;
                            break;
                        }

                        if (string.Compare(content, "wait", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Wait;
                            break;
                        }

                        if (string.Compare(content, "complete", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Complete;
                            break;
                        }

                        if (string.Compare(content, "completed", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Completed;
                            break;
                        }

                        if (string.Compare(content, "break", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Break;
                            break;
                        }

                        if (string.Compare(content, "alias", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Alias;
                            break;
                        }

                        if (string.Compare(content, "var", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Var;
                            break;
                        }

                        if (string.Compare(content, "public", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Public;
                            break;
                        }

                        if (string.Compare(content, "protected", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Protected;
                            break;
                        }

                        if (string.Compare(content, "private", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Private;
                            break;
                        }

                        if (string.Compare(content, "return", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Return;
                            break;
                        }

                        if (string.Compare(content, "if", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.If;
                            break;
                        }

                        if (string.Compare(content, "elif", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Elif;
                            break;
                        }

                        if (string.Compare(content, "while", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.While;
                            break;
                        }

                        if (string.Compare(content, "continue", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Continue;
                            break;
                        }

                        if (string.Compare(content, "repeat", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Repeat;
                            break;
                        }

                        if (string.Compare(content, "state", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.State;
                            break;
                        }

                        if (string.Compare(content, "states", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.States;
                            break;
                        }

                        if (string.Compare(content, "as", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.As;
                            break;
                        }

                        if (string.Compare(content, "default", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Default;
                            break;
                        }

                        if (string.Compare(content, "down", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Down;
                            break;
                        }

                        if (string.Compare(content, "duration", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Duration;
                            break;
                        }

                        if (string.Compare(content, "_", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.BlankIdentifier;
                            break;
                        }

                        if (string.Compare(content, "add", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Add;
                            break;
                        }

                        if (string.Compare(content, "fact", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Fact;
                            break;
                        }

                        if (string.Compare(content, "reject", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Reject;
                            break;
                        }

                        if (string.Compare(content, "exec", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Exec;
                            break;
                        }

                        if (string.Compare(content, "synonym", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Synonym;
                            break;
                        }

                        if (string.Compare(content, "idle", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Idle;
                            break;
                        }

                        if (string.Compare(content, "with", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.With;
                            break;
                        }

                        if (string.Compare(content, "import", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Import;
                            break;
                        }

                        if (string.Compare(content, "new", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.New;
                            break;
                        }

                        if (string.Compare(content, "weak", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Weak;
                            break;
                        }

                        if (string.Compare(content, "cancel", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Cancel;
                            break;
                        }

                        if (string.Compare(content, "canceled", true) == 0)
                        {
                            kindOfKeyWord = KeyWordTokenKind.Canceled;
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
