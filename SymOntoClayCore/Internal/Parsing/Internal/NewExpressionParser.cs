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
    public class NewExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotNewMark
        }

        public NewExpressionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public NewAstExpression Result { get; private set; } = new NewAstExpression();

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
                                case KeyWordTokenKind.New:
                                    _state = State.GotNewMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotNewMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {                           
                                var node = new ConstValueAstExpression();
                                node.Value = NameHelper.CreateName(_currToken.Content);

                                Result.Left = node;

                                var nextToken = _context.GetToken();

                                _context.Recovery(nextToken);

                                if (nextToken.TokenKind == TokenKind.OpenRoundBracket)
                                {
                                    ProcessParameters();
                                }

                                Exit();
                            }
                            break;

                        case TokenKind.Var:
                            {
                                var value = NameHelper.CreateName(_currToken.Content);

                                var node = new VarAstExpression();
                                node.Name = value;

                                Result.Left = node;

                                var nextToken = _context.GetToken();

                                _context.Recovery(nextToken);

                                if (nextToken.TokenKind == TokenKind.OpenRoundBracket)
                                {
                                    ProcessParameters();
                                }

                                Exit();
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

        private void ProcessParameters()
        {
            var parser = new CallingFunctionExpressionParser(_context);
            parser.Run();

            Result.Parameters = parser.Result.Parameters;
        }
    }
}
