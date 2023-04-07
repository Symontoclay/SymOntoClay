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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public class FunctionPredictor: BaseConcretePredictor
    {
        private enum State
        {
            Init,
            GotFunMark,
            InParamDecls,
            GotParamDecls,
            GotLambda,
            InFunctionBody
        }

        public FunctionPredictor(Token currToken, InternalParserContext context)
            : base(currToken, context)
        {
        }
        
        /// <inheritdoc/>
        protected override KeyWordTokenKind DefaultResult { get; set; } = KeyWordTokenKind.Fun;

        private State _state = State.Init;

        private int _openRoundBracketsCount = 0;
        private int _openFigureBracketsCount = 0;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Fun:
                                    _state = State.GotFunMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFunMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _openRoundBracketsCount++;
                            _state = State.InParamDecls;
                            break;

                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.InParamDecls:
                    if(IsUsualContentOfParameter())
                    {
                        break;
                    }
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            if(_openRoundBracketsCount == 1)
                            {
                                _openRoundBracketsCount = 0;
                                _state = State.GotParamDecls;
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParamDecls:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.GotLambda;
                            break;

                        case TokenKind.OpenFigureBracket:
                            _openFigureBracketsCount++;
                            _state = State.InFunctionBody;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotLambda:
                    if(IsUsualContentOfBody())
                    {
                        _context.Recovery(_currToken);
                        _state = State.InFunctionBody;
                        break;
                    }

                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.InFunctionBody:
                    if (IsUsualContentOfBody())
                    {
                        break;
                    }
                    
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            if(_openFigureBracketsCount == 0)
                            {
                                Complete();
                                break;
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _openFigureBracketsCount--;

                            if(_openFigureBracketsCount == 0)
                            {
                                Complete();
                                break;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private bool IsUsualContentOfBody()
        {
            if(IsUsualContentOfExpression())
            {
                return true;
            }

            switch (_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        
                        case KeyWordTokenKind.Return:
                            return true;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }

                default: 
                    return false;
            }
        }

        private bool IsUsualContentOfParameter()
        {
            if (IsUsualContentOfExpression())
            {
                return true;
            }

            return false;
        }

        private bool IsUsualContentOfExpression()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Unknown:
                            return true;

                        default:
                            return false;
                    }

                case TokenKind.Var:
                case TokenKind.Channel:
                case TokenKind.SystemVar:
                case TokenKind.LogicalVar:
                case TokenKind.EntityCondition:
                case TokenKind.Point:
                case TokenKind.Comma:
                case TokenKind.Colon:
                case TokenKind.Number:
                case TokenKind.String:
                case TokenKind.Identifier:
                case TokenKind.Entity:
                case TokenKind.Plus:
                case TokenKind.Minus:
                case TokenKind.Multiplication:
                case TokenKind.Division:
                case TokenKind.QuestionMark:
                case TokenKind.AsyncMarker:
                case TokenKind.DoubleAsyncMarker:
                case TokenKind.OpenFactBracket:
                case TokenKind.CloseFactBracket:
                case TokenKind.Assign:
                case TokenKind.More:
                case TokenKind.MoreOrEqual:
                case TokenKind.Less:
                case TokenKind.LessOrEqual:
                case TokenKind.And:
                case TokenKind.Or:
                case TokenKind.Not:
                case TokenKind.LeftRightStream:
                case TokenKind.PrimaryLogicalPartMark:
                case TokenKind.LeftRightArrow:
                case TokenKind.PositiveInfinity:
                case TokenKind.NegativeInfinity:
                    return true;

                default:
                    return false;
            }
        }
    }
}
