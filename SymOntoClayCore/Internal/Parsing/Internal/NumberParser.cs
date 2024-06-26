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

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class NumberParser : BaseInternalParser
    {
        private readonly static CultureInfo _targetCulture = new CultureInfo("en-GB");

        private enum State
        {
            Init,
            GotIntegerPart,
            GotSeparator,
            GotFractionPart
        }

        public NumberParser(InternalParserContext context, bool createLogicalValue = false)
            : base(context)
        {
            _createLogicalValue = createLogicalValue;
        }

        private State _state = State.Init;

        public Value Result { get; private set; }
        public StringBuilder _buffer;
        private readonly bool _createLogicalValue;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _buffer = new StringBuilder();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            _state = State.GotIntegerPart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIntegerPart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            break;

                        case TokenKind.Point:
                            {
                                var nextToken = _context.GetToken();

                                if(nextToken.TokenKind == TokenKind.Number)
                                {
                                    _context.Recovery(nextToken);

                                    _buffer.Append(".");

                                    _state = State.GotSeparator;
                                }
                                else
                                {
                                    _context.Recovery(nextToken);
                                    _context.Recovery(_currToken);
                                    Exit();
                                }                              
                            }
                            break;

                        case TokenKind.CloseSquareBracket:
                            ProcessCloseSquareBracket();
                            break;

                        case TokenKind.Comma:
                        case TokenKind.Semicolon:
                        case TokenKind.CloseRoundBracket:
                        case TokenKind.OpenRoundBracket:
                        case TokenKind.CloseFactBracket:
                        case TokenKind.CloseAnnotationBracket:
                        case TokenKind.Plus:
                        case TokenKind.Minus:
                        case TokenKind.Multiplication:
                        case TokenKind.Division:
                        case TokenKind.LeftRightStream:
                        case TokenKind.Or:
                        case TokenKind.And:
                        case TokenKind.Not:
                        case TokenKind.More:
                        case TokenKind.MoreOrEqual:
                        case TokenKind.Less:
                        case TokenKind.LessOrEqual:
                        case TokenKind.Lambda:
                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Word:
                            if(_createLogicalValue)
                            {
                                _context.Recovery(_currToken);
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSeparator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            _state = State.GotFractionPart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFractionPart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            break;

                        case TokenKind.CloseSquareBracket:
                            ProcessCloseSquareBracket();
                            break;

                        case TokenKind.Comma:
                        case TokenKind.Semicolon:
                        case TokenKind.CloseRoundBracket:
                        case TokenKind.OpenRoundBracket:
                        case TokenKind.CloseFactBracket:
                        case TokenKind.CloseFigureBracket:
                        case TokenKind.CloseAnnotationBracket:
                        case TokenKind.Plus:
                        case TokenKind.Minus:
                        case TokenKind.Multiplication:
                        case TokenKind.Division:
                        case TokenKind.LeftRightStream:
                        case TokenKind.Or:
                        case TokenKind.And:
                        case TokenKind.Not:
                        case TokenKind.More:
                        case TokenKind.MoreOrEqual:
                        case TokenKind.Less:
                        case TokenKind.LessOrEqual:
                        case TokenKind.Lambda:
                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Word:
                            if (_createLogicalValue)
                            {
                                _context.Recovery(_currToken);
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessCloseSquareBracket()
        {
            _context.Recovery(_currToken);
            Exit();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(_createLogicalValue)
            {
                var numberValue = float.Parse(_buffer.ToString(), _targetCulture);

                Result = new LogicalValue(numberValue);
            }
            else
            {
                var numberValue = double.Parse(_buffer.ToString(), _targetCulture);

                Result = new NumberValue(numberValue);
            }
        }
    }
}
