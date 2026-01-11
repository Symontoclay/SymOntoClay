/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Parsing.Internal.Predictors;
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
            GotCommaInPositionedMainParameter,
            WaitForNameOfNamedMainParameter,
            GotNameOfNamedMainParameter,
            WaitForValueOfNamedMainParameter,
            GotValueOfNamedMainParameter,
            GotCommaInNamedMainParameter
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
            if(_currentParameter != null)
            {
                if(_currentParameter.IsNamed && _currentParameter.Name != null && _currentParameter.Value == null)
                {
                    throw new NotImplementedException("46C8EB09-4AB7-4063-913F-E4CCD594DA33");
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("344D2180-43CD-4AEA-A308-D2CEF95E03D6", $"_state = {_state}");
            //Info("200D2045-E688-476B-9219-815760A3471F", $"_currToken = {_currToken}");
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
                                throw new UnexpectedTokenException(Text, _currToken);
                            }
                            Result.IsAsync = true;
                            Result.IsChild = true;
                            break;

                        case TokenKind.DoubleAsyncMarker:
                            if (Result.IsAsync)
                            {
                                throw new UnexpectedTokenException(Text, _currToken);
                            }
                            Result.IsAsync = true;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForMainParameter:
                    if (_currToken.TokenKind == TokenKind.Word || _currToken.TokenKind == TokenKind.Var)
                    {
                        var predictedKeyWordTokenKind = PredictKeyWordTokenKind(KindOfSpecialPrediction.NamedParameter);

                        if (predictedKeyWordTokenKind == KeyWordTokenKind.NamedParameter)
                        {
                            _currentParameter = new CallingParameter();
                            Result.Parameters.Add(_currentParameter);

                            var value = NameHelper.CreateName(_currToken.Content, Logger);

                            var node = new ConstValueAstExpression();
                            node.Value = value;

                            _currentParameter.Name = node;

                            _state = State.GotNameOfNamedMainParameter;
                            break;
                        }
                    }
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                        case TokenKind.Number:
                        case TokenKind.String:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                        case TokenKind.Entity:
                        case TokenKind.OpenFigureBracket:
                           {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                _currentParameter.Value = ParseParameterValue();

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.EntityCondition:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                _currentParameter.Value = ParseEntityConditionValue();

                                _state = State.GotPositionedMainParameter;
                            }
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                _currentParameter.Value = ParseLogicalQueryValue();

                                _state = State.GotPositionedMainParameter;
                            }
                            break;


                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                            _state = State.GotCommaInPositionedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForNameOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                _currentParameter = new CallingParameter();
                                Result.Parameters.Add(_currentParameter);

                                var node = new ConstValueAstExpression();
                                node.Value = NameHelper.CreateName(_currToken.Content);

                                _currentParameter.Name = node;

                                _state = State.GotNameOfNamedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotNameOfNamedMainParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.WaitForValueOfNamedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForValueOfNamedMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                        case TokenKind.Number:
                        case TokenKind.String:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                        case TokenKind.Entity:
                        case TokenKind.OpenFigureBracket:
                            {
                                _currentParameter.Value = ParseParameterValue();

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        case TokenKind.EntityCondition:
                            {
                                _currentParameter.Value = ParseEntityConditionValue();

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _currentParameter.Value = ParseLogicalQueryValue();

                                _state = State.GotValueOfNamedMainParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotValueOfNamedMainParameter:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.GotCommaInNamedMainParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private AstExpression ParseParameterValue()
        {
            _context.Recovery(_currToken);
            var parser = new AstExpressionParser(_context, TokenKind.Comma, TokenKind.CloseRoundBracket);
            parser.Run();

            return parser.Result;
        }

        private AstExpression ParseEntityConditionValue()
        {
            _context.Recovery(_currToken);

            var parser = new ConditionalEntityParser(_context);
            parser.Run();

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

            return node;
        }

        private AstExpression ParseLogicalQueryValue()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

#if DEBUG
            //Info("2F3CBE2F-D17C-462E-8C96-2C51EC9B32BF", $"parser.Result = {parser.Result}");
#endif

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

            return node;
        }

        private AstExpression ConvertValueExprToNameExpr(AstExpression expression)
        {
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
