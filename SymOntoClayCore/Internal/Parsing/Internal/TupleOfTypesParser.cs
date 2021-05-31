using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TupleOfTypesParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForTypeIdentifier,
            GotTypeIdentifier
        }

        public TupleOfTypesParser(InternalParserContext context, bool boundedBuRoundBrackets)
            : base(context)
        {
            _boundedBuRoundBrackets = boundedBuRoundBrackets;

            if(boundedBuRoundBrackets)
            {
                _state = State.Init;
            }
            else
            {
                _state = State.WaitForTypeIdentifier;
            }
        }

        private readonly bool _boundedBuRoundBrackets;

        private State _state = State.Init;

        public List<StrongIdentifierValue> Result { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForTypeIdentifier;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForTypeIdentifier:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                Result.Add(ParseName(_currToken.Content));

                                _state = State.GotTypeIdentifier;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTypeIdentifier:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Or:
                            _state = State.WaitForTypeIdentifier;
                            break;

                        case TokenKind.CloseRoundBracket:
                            if(!_boundedBuRoundBrackets)
                            {
                                _context.Recovery(_currToken);
                            }
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
