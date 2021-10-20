using SymOntoClay.Core.Internal.CodeModel;
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

        public EntityConditionExpressionNode Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
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

                                var entityConditionExpressionParserContext = new EntityConditionExpressionParserContext(_context);

                                var parser = new EntityConditionExpressionParser(entityConditionExpressionParserContext);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
                                //Log($"parser.Result.GetHumanizeDbgString() = {parser.Result.ToHumanizedString()}");
#endif

                                Result = parser.Result;

                                _state = State.GotExpresion;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotExpresion:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
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
