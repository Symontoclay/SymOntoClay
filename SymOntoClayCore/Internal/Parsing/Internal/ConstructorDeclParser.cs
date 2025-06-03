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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Compiling;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class ConstructorDeclParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotConstructorMark,
            GotParameters,
            WaitForSuperClassContructorItem,
            GotSuperClassContructorItem,
            WaitForAction,
            GotAction
        }

        public ConstructorDeclParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Constructor Result { get;private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateConstructorAndSetAsCurrentCodeItem();
            Result.TypeOfAccess = _context.CurrentDefaultSettings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

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
                                case KeyWordTokenKind.Constructor:
                                    _state = State.GotConstructorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotConstructorMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessParameters();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotParameters:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.Colon:
                            _state = State.WaitForSuperClassContructorItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForSuperClassContructorItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:                        
                            {
                                _context.Recovery(_currToken);
                                var parser = new AstExpressionParser(_context, TokenKind.Comma, TokenKind.OpenFigureBracket);
                                parser.Run();

                                Result.CallSuperClassContructorsExpressions.Add(parser.Result);

                                _state = State.GotSuperClassContructorItem;
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                var token = new Token() 
                                { 
                                    TokenKind = TokenKind.Word,
                                    Content = StandardNamesConstants.DefaultCtorName,
                                    Line = _currToken.Line, 
                                    Pos= _currToken.Pos
                                };

                                _context.Recovery(_currToken);
                                _context.Recovery(token);

                                var parser = new AstExpressionParser(_context, TokenKind.Comma, TokenKind.OpenFigureBracket);
                                parser.Run();

                                Result.CallSuperClassContructorsExpressions.Add(parser.Result);

                                _state = State.GotSuperClassContructorItem;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotSuperClassContructorItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.Comma:
                            _state = State.WaitForSuperClassContructorItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                Result.Statements = statementsList;
                                Result.CompiledFunctionBody = _context.Compiler.Compile(statementsList, Result.CallSuperClassContructorsExpressions, KindOfCompilation.Constructor);
                                _state = State.GotAction;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotAction:
                    _context.Recovery(_currToken);
                    Exit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ProcessParameters()
        {
            _context.Recovery(_currToken);

            var parser = new FunctionParametersParser(_context);
            parser.Run();

            Result.Arguments = parser.Result;

            _state = State.GotParameters;
        }
    }
}
