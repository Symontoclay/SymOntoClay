/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FunctionBodyParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForStatement
        }

        public FunctionBodyParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public List<AstStatement> Result { get; private set; } = new List<AstStatement>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.WaitForStatement;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForStatement:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                        case TokenKind.QuestionMark:
                            ProcessExpressionStatement();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Unknown:
                                case KeyWordTokenKind.Select:
                                case KeyWordTokenKind.Insert:
                                case KeyWordTokenKind.Null:
                                    ProcessExpressionStatement();
                                    break;

                                case KeyWordTokenKind.Use:
                                    ProcessUseStatement();
                                    break;

                                case KeyWordTokenKind.Error:
                                    ProcessErrorStatement();
                                    break;

                                case KeyWordTokenKind.Try:
                                    ProcessTryStatement();
                                    break;

                                case KeyWordTokenKind.Await:
                                    ProcessAwaitStatement();
                                    break;

                                case KeyWordTokenKind.Complete:
                                    ProcessCompleteStatement();
                                    break;

                                case KeyWordTokenKind.Break:
                                    ProcessBreakStatement();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.Identifier:
                            ProcessExpressionStatement();
                            break;

                        case TokenKind.CloseFigureBracket:
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

        private void ProcessExpressionStatement()
        {
            _context.Recovery(_currToken);
            var parser = new CodeExpressionStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessUseStatement()
        {
            _context.Recovery(_currToken);
            var parser = new UseStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessErrorStatement()
        {
            _context.Recovery(_currToken);
            var parser = new ErrorStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessTryStatement()
        {
            _context.Recovery(_currToken);
            var parser = new TryStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessAwaitStatement()
        {
            _context.Recovery(_currToken);
            var parser = new AwaitStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessCompleteStatement()
        {
            _context.Recovery(_currToken);
            var parser = new CompleteStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void ProcessBreakStatement()
        {
            _context.Recovery(_currToken);
            var parser = new BreakStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void AddStatement(AstStatement statement)
        {
            if (statement != null)
            {
                Result.Add(statement);
            }
        }
    }
}
