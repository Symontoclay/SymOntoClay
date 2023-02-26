/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class ConditionalEntityParser : BaseInternalParser
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

        public ConditionalEntityParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        private bool _isWayPoint;
        public Value Result { get; private set; }
        private StrongIdentifierValue _name;
        private Value _firstCoordinate;
        private Value _secondCoordinate;
        private EntityConditionExpressionNode _entityConditionExpression;
        private RuleInstance _entityConditionQuery;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(_isWayPoint)
            {
                Result = new WaypointSourceValue(_firstCoordinate, _secondCoordinate, _name);
                return;
            }

            if(_entityConditionExpression != null)
            {
                var result = new ConditionalEntitySourceValue(_entityConditionExpression, _name);

#if DEBUG
                //Log($"result = {result}");
#endif

                if (_context.NeedCheckDirty)
                {
                    result.CheckDirty();
                }

#if DEBUG
                //Log($"result (after) = {result}");
                //Log($"result.ToDbgString() (after) = {result.ToDbgString()}");
#endif

                Result = result;
                return;
            }

            {
                var result = new ConditionalEntitySourceValue(_entityConditionQuery, _name);

                if (_context.NeedCheckDirty)
                {
                    result.CheckDirty();
                }

#if DEBUG
                //Log($"result (after) = {result}");
                //Log($"result.ToDbgString() (after) = {result.ToDbgString()}");
#endif

                Result = result;
            }
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
                                var name = NameHelper.CreateName(_currToken.Content);

#if DEBUG
                                //Log($"name = {name}");
#endif

                                _name = name;

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
                            _isWayPoint = true;
                            _state = State.WaitForFirstCoordinate;
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                _isWayPoint = false;

                                _context.Recovery(_currToken);

                                var parser = new EntityConditionParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
                                //Log($"parser.Result.GetHumanizeDbgString() = {parser.Result.ToHumanizedString()}");
#endif

                                _entityConditionExpression = parser.Result;

                                Exit();
                            }
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _isWayPoint = false;

                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
                                //Log($"parser.Result.GetHumanizeDbgString() = {parser.Result.ToHumanizedString()}");
#endif
                                _entityConditionQuery = parser.Result;

                                Exit();
                            }
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

                                _firstCoordinate = parser.Result;

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
                            if (_isWayPoint)
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

                                _secondCoordinate = parser.Result;

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
                            if(_isWayPoint)
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
