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
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveHtnTaskOperatorParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotOperatorMark,
            GotCallingExpr
        }

        public PrimitiveHtnTaskOperatorParser(InternalParserContext context)
            : base(context)
        {
        }

        public PrimitiveHtnTaskOperator Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new PrimitiveHtnTaskOperator();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("37F9FBC3-94E5-4926-98B4-4B0C40D5A278", $"_state = {_state}");
            //Info("B3BC4040-868D-4647-B09A-25E54901EFC6", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Operator:
                                    _state = State.GotOperatorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotOperatorMark:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, new TerminationToken(TokenKind.Semicolon, true));
                        parser.Run();

#if DEBUG
                        //Info("743FE80C-35FB-4007-A029-0E1EFBEB8DDB", $"parser.Result = {parser.Result}");
#endif

                        var statement = new AstExpressionStatement();
                        Result.Statement = statement;

                        statement.Expression = parser.Result;

                        Result.IntermediateCommandsList = _context.Compiler.CompileToIntermediateCommands(statement);

                        _state = State.GotCallingExpr;

                    }
                    break;

                case State.GotCallingExpr:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}
