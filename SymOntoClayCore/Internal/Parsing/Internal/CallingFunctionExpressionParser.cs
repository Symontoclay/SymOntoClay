/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CallingFunctionExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForMainParameter,
            GotPositionedMainParameter,
            WaitForValueOfNamedMainParameter,
            GotValueOfNamedMainParameter
        }

        public CallingFunctionExpressionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public CallingFunctionAstExpression Result { get; private set; }
        private CallingParameter _currentParameter;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CallingFunctionAstExpression();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForMainParameter;
                            break;

                        case TokenKind.AsyncMarker:
                            Result.IsAsync = true;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

                                var node = new ConstValueAstExpression();
                                node.Value = value;

                                _currentParameter.Value = node;
                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPositionedMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _currentParameter.Name = _currentParameter.Value;
                            _currentParameter.Value = null;

                            _state = State.WaitForValueOfNamedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForValueOfNamedMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.EntityCondition:
                            {
                                _context.Recovery(_currToken);

                                var parser = new EntityConditionParser(_context);
                                parser.Run();

                                _currentParameter.Value = parser.Result;

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotValueOfNamedMainParameter:
                    switch(_currToken.TokenKind)
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
