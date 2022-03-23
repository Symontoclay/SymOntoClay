using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class MutuallyExclusiveStatesParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotStatesMark,
            WaitForStateName,
            GotStateName
        }

        public MutuallyExclusiveStatesParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public MutuallyExclusiveStatesSet Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new MutuallyExclusiveStatesSet();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.States:
                            _state = State.GotStatesMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotStatesMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.WaitForStateName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForStateName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            Result.StateNames.Add(ParseName(_currToken.Content));
                            _state = State.GotStateName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotStateName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForStateName;
                            break;

                            case TokenKind.CloseFigureBracket:
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
