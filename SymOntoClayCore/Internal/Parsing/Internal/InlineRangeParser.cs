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

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InlineRangeParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotRangeMark,
            GotLeftLimitationMark,
            GotLeftValue,
            WeitForRightValue,
            GotRightValue
        }

        public InlineRangeParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public RangeValue Result { get; private set; }
        private RangeBoundary _leftBoundary;
        private RangeBoundary _rightBoundary;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new RangeValue();

            _leftBoundary = new RangeBoundary();
            _rightBoundary = new RangeBoundary();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(_leftBoundary.Value != null)
            {
                Result.LeftBoundary = _leftBoundary;
            }

            if(_rightBoundary.Value != null)
            {
                Result.RightBoundary = _rightBoundary;
            }

        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Range:
                            _state = State.GotRangeMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRangeMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenSquareBracket:
                            _leftBoundary.Includes = true;
                            _state = State.GotLeftLimitationMark;
                            break;

                        case TokenKind.OpenRoundBracket:
                            _leftBoundary.Includes = false;
                            _state = State.GotLeftLimitationMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotLeftLimitationMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context);
                                parser.Run();

                                _leftBoundary.Value = parser.Result.AsNumberValue;

                                _state = State.GotLeftValue;
                            }
                            break;

                        case TokenKind.NegativeInfinity:
                        case TokenKind.Multiplication:
                            _state = State.GotLeftValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotLeftValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WeitForRightValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WeitForRightValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context);
                                parser.Run();

                                _rightBoundary.Value = parser.Result.AsNumberValue;

                                _state = State.GotRightValue;
                            }
                            break;

                        case TokenKind.PositiveInfinity:
                        case TokenKind.Multiplication:
                            _state = State.GotRightValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRightValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseSquareBracket:
                            _rightBoundary.Includes = true;
                            Exit();
                            break;

                        case TokenKind.CloseRoundBracket:
                            _rightBoundary.Includes = false;
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
