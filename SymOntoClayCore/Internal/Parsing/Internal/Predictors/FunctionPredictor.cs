using System;
using System.Collections.Generic;
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
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"_openRoundBracketsCount = {_openRoundBracketsCount}");
            //Log($"_openFigureBracketsCount = {_openFigureBracketsCount}");
#endif

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

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.InParamDecls:
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
            switch (_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Unknown:
                        case KeyWordTokenKind.Return:
                            return true;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.Number: 
                    return true;

                default: 
                    return false;
            }
        }
    }
}
