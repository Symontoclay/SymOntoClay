using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EntityConditionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForExpression,
            GotExpresion
        }

        public EntityConditionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            switch(_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForExpression;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForExpression:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {
                                _context.Recovery(_currToken);

                                var parser = new EntityConditionExpressionParser(_context);
                                parser.Run();

                                throw new NotImplementedException();
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotExpresion:
                    switch (_currToken.TokenKind)
                    {
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
