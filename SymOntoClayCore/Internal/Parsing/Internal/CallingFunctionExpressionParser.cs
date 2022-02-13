/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
    public class CallingFunctionExpressionParser : BaseInternalParser
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
        protected override void OnFinish()
        {
#if DEBUG
            //Log($"_currentParameter = {_currentParameter}");
#endif

            if(_currentParameter != null)
            {
                if(_currentParameter.IsNamed && _currentParameter.Name != null && _currentParameter.Value == null)
                {
                    throw new NotImplementedException();
                }
            }
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
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForMainParameter;
                            break;

                        case TokenKind.AsyncMarker:
                            if(Result.IsAsync)
                            {
                                throw new UnexpectedTokenException(_currToken);
                            }
                            Result.IsAsync = true;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                switch(_currToken.KeyWordTokenKind)
                                {
                                    case KeyWordTokenKind.Null:
                                        {
                                            var node = new ConstValueAstExpression();
                                            node.Value = new NullValue();

                                            _currentParameter.Value = node;
                                        }
                                        break;

                                    default:
                                        {
                                            var value = NameHelper.CreateName(_currToken.Content);

                                            var node = new ConstValueAstExpression();
                                            node.Value = value;

                                            _currentParameter.Value = node;
                                        }
                                        break;
                                }

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.Number:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                _currentParameter.Value = node;
                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.String:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                var node = new ConstValueAstExpression();
                                node.Value = new StringValue(_currToken.Content);

                                _currentParameter.Value = node;
                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.Var:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                var value = NameHelper.CreateName(_currToken.Content);

                                var node = new VarAstExpression();
                                node.Name = value;

                                _currentParameter.Value = node;

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.EntityCondition:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                _context.Recovery(_currToken);

                                var parser = new ConditionalEntityParser(_context);
                                parser.Run();

                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                _currentParameter.Value = node;

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPositionedMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            if(_currentParameter.Value != null)
                            {
                                _currentParameter.Name = ConvertValueExprToNameExpr(_currentParameter.Value);
                                _currentParameter.Value = null;
                            }

                            _state = State.WaitForValueOfNamedMainParameter;
                            break;

                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.GotComma;
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

                                var parser = new ConditionalEntityParser(_context);
                                parser.Run();

                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                _currentParameter.Value = node;

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

                                var node = new ConstValueAstExpression();
                                node.Value = parser.Result;

                                _currentParameter.Value = node;

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        case TokenKind.String:
                            {
                                var node = new ConstValueAstExpression();
                                node.Value = new StringValue(_currToken.Content);

                                _currentParameter.Value = node;

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            {
                                var value = NameHelper.CreateName(_currToken.Content);

                                var node = new ConstValueAstExpression();
                                node.Value = value;

                                _currentParameter.Value = node;

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

                case State.GotComma:
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private AstExpression ConvertValueExprToNameExpr(AstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            var kind = expression.Kind;

            switch(kind)
            {
                case KindOfAstExpression.ConstValue:
                    return expression;

                case KindOfAstExpression.Var:
                    {
                        var node = new ConstValueAstExpression();
                        node.Value = expression.AsVarAstExpression.Name;
                        return node;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
