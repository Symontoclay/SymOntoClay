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

        public NumberParser(InternalParserContext context)
            : base(context)
        {

        }

        private State _state = State.Init;

        public NumberValue Result { get; private set; }
        public StringBuilder _buffer;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _buffer = new StringBuilder();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            Log($"buffer = {_buffer}");
            Log($"_state = {_state}");
#endif

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

#if DEBUG
                                Log($"nextToken = {nextToken}");
#endif

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
                            _context.Recovery(_currToken);
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

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            Log($"buffer = {_buffer}");
#endif

            var numberValue = double.Parse(_buffer.ToString(), _targetCulture);

#if DEBUG
            Log($"numberValue = {numberValue}");
#endif

            Result = new NumberValue(numberValue);
        }
    }
}
