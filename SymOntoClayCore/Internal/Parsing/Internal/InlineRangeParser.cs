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
#if DEBUG
            //Log($"_leftBoundary = {_leftBoundary}");
            //Log($"_rightBoundary = {_rightBoundary}");
            //Log($"Result = {Result}");
#endif

            if(_leftBoundary.Value != null)
            {
                Result.LeftBoundary = _leftBoundary;
            }

            if(_rightBoundary.Value != null)
            {
                Result.RightBoundary = _rightBoundary;
            }

#if DEBUG
            //Log($"Result (after) = {Result}");
#endif
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");            
#endif

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

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

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

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

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
