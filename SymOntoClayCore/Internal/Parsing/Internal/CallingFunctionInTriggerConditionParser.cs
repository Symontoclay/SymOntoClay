using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CallingFunctionInTriggerConditionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForMainParameter,
            GotPositionedMainParameter,
            WaitForValueOfNamedMainParameter,
            GotValueOfNamedMainParameter,
            GotComma
        }

        public CallingFunctionInTriggerConditionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public TriggerConditionNode Result { get; private set; }

        //private CallingParameter _currentParameter;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new TriggerConditionNode()
            {
                Kind = KindOfTriggerConditionNode.UnaryOperator,
                KindOfOperator = KindOfOperator.CallFunction,
                ParamsList = new List<TriggerConditionNode>()
            };
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log($"_currentParameter = {_currentParameter}");
#endif

            //throw new NotImplementedException();

            //if (_currentParameter != null)
            //{
            //    if (_currentParameter.IsNamed && _currentParameter.Name != null && _currentParameter.Value == null)
            //    {
            //        throw new NotImplementedException();
            //    }
            //}
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Number:
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                        case TokenKind.String:
                        case TokenKind.Var:
                        case TokenKind.EntityCondition:
                            {
                                _context.Recovery(_currToken);

                                var parser = new TriggerConditionParser(_context, TokenKind.CloseRoundBracket, TokenKind.Colon, TokenKind.Comma);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                Result.ParamsList.Add(parser.Result);

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPositionedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForValueOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotValueOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotComma:
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
