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

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Linq;
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
            GotCommaInPositionedMainParameter,
            WaitForNameOfNamedMainParameter,
            GotNameOfNamedMainParameter,
            WaitForValueOfNamedMainParameter,
            GotValueOfNamedMainParameter,            
            GotCommaInNamedMainParameter
        }

        public CallingFunctionInTriggerConditionParser(InternalParserContext context)
            : base(context)
        {
        }
        
        private State _state = State.Init;

        public TriggerConditionNode Result { get; private set; }

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
        protected override void OnRun()
        {
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

                        case TokenKind.Colon:
                            Result.IsNamedParameters = true;

                            var prevParam = Result.ParamsList.First();

                            if(prevParam.Kind == KindOfTriggerConditionNode.Var)
                            {
                                prevParam.Kind = KindOfTriggerConditionNode.Concept;
                            }

                            _state = State.WaitForValueOfNamedMainParameter;
                            break;

                        case TokenKind.Comma:
                            _state = State.GotCommaInPositionedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCommaInPositionedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                        case TokenKind.Word:
                        case TokenKind.EntityCondition:
                        case TokenKind.Var:
                        case TokenKind.String:
                            _context.Recovery(_currToken);
                            _state = State.WaitForMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForNameOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                _context.Recovery(_currToken);

                                var parser = new TriggerConditionParser(_context, TokenKind.CloseRoundBracket, TokenKind.Colon, TokenKind.Comma);
                                parser.Run();

                                var parserResult = parser.Result;

                                if (parserResult.Kind == KindOfTriggerConditionNode.Var)
                                {
                                    parserResult.Kind = KindOfTriggerConditionNode.Concept;
                                }

                                Result.ParamsList.Add(parserResult);

                                _state = State.GotNameOfNamedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotNameOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.WaitForValueOfNamedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForValueOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
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

                                Result.ParamsList.Add(parser.Result);

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotValueOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.GotCommaInNamedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCommaInNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            _context.Recovery(_currToken);
                            _state = State.WaitForNameOfNamedMainParameter;
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
