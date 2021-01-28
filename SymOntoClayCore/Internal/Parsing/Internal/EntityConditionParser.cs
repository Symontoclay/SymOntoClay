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
    public class EntityConditionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotName,
            WaitForFirstCoordinate,
            GotFirstCoordinate,
            WaitForSecondCoordinate,
            GotSecondCoordinate
        }

        public EntityConditionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public EntityConditionAstExpression Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new EntityConditionAstExpression();
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
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.EntityCondition:
                            {
                                var name = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
                                //Log($"name = {name}");
#endif

                                Result.Name = name;

                                _state = State.GotName;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenSquareBracket:
                            Result.KindOfEntityConditionAstExpression = KindOfEntityConditionAstExpression.Waypoint;
                            _state = State.WaitForFirstCoordinate;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForFirstCoordinate:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context);
                                parser.Run();
                                
                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                Result.FirstCoordinate = node;

                                _state = State.GotFirstCoordinate;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFirstCoordinate:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForSecondCoordinate;
                            break;

                        case TokenKind.CloseSquareBracket:
                            if (Result.KindOfEntityConditionAstExpression == KindOfEntityConditionAstExpression.Waypoint)
                            {
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSecondCoordinate:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context);
                                parser.Run();

                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                Result.SecondCoordinate = node;

                                _state = State.GotSecondCoordinate;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSecondCoordinate:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseSquareBracket:
                            if(Result.KindOfEntityConditionAstExpression == KindOfEntityConditionAstExpression.Waypoint)
                            {
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

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
