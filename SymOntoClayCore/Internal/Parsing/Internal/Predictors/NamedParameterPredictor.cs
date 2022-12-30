using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public class NamedParameterPredictor : BaseConcretePredictor
    {
        private enum State
        {
            Init,
            GotParamName
        }

        public NamedParameterPredictor(Token currToken, InternalParserContext context)
            : base(currToken, context)
        {
        }

        /// <inheritdoc/>
        protected override KeyWordTokenKind DefaultResult { get; set; } = KeyWordTokenKind.NamedParameter;

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Var:
                            _state= State.GotParamName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParamName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            Complete();
                            break;

                        case TokenKind.CloseRoundBracket:
                        case TokenKind.OpenRoundBracket:
                            Reject();
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
