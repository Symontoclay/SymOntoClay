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

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TryStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotTryMark,
            GotTryBody,
            GotCatch,
            GotElseMark,
            GotElseBody,
            GotEnsureMark,
            GotEnsureBody
        }

        public TryStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private AstTryStatement _rawStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new AstTryStatement();

            DefaultSettingsOfCodeEntityHelper.SetUpAnnotatedItem(_rawStatement, CurrentDefaultSetings);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            _rawStatement.CheckDirty();

            Result = _rawStatement;
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
                                case KeyWordTokenKind.Try:
                                    _state = State.GotTryMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTryMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                _rawStatement.TryStatements = statementsList;

                                _state = State.GotTryBody;
                            }                            
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTryBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Else:
                                    _state = State.GotElseMark;
                                    break;

                                case KeyWordTokenKind.Ensure:
                                    _state = State.GotEnsureMark;
                                    break;

                                case KeyWordTokenKind.Catch:
                                    ProcessCatch();
                                    break;

                                case KeyWordTokenKind.Select:
                                case KeyWordTokenKind.Insert:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.String:
                        case TokenKind.Number:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCatch:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Else:
                                    _state = State.GotElseMark;
                                    break;

                                case KeyWordTokenKind.Ensure:
                                    _state = State.GotEnsureMark;
                                    break;

                                case KeyWordTokenKind.Catch:
                                    ProcessCatch();
                                    break;

                                case KeyWordTokenKind.Select:
                                case KeyWordTokenKind.Insert:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.String:
                        case TokenKind.Number:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotElseMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                _rawStatement.ElseStatements = statementsList;

                                _state = State.GotElseBody;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotElseBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Ensure:
                                    _state = State.GotEnsureMark;
                                    break;

                                case KeyWordTokenKind.Select:
                                case KeyWordTokenKind.Insert:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.String:
                        case TokenKind.Number:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotEnsureMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                _rawStatement.EnsureStatements = statementsList;

                                _state = State.GotEnsureBody;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotEnsureBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Select:
                                case KeyWordTokenKind.Insert:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.String:
                        case TokenKind.Number:
                        case TokenKind.Var:
                        case TokenKind.SystemVar:
                            _context.Recovery(_currToken);
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

        private void ProcessCatch()
        {
            _context.Recovery(_currToken);

            var parser = new CatchStatementParser(_context);
            parser.Run();

            _rawStatement.CatchStatements.Add(parser.Result);

            _state = State.GotCatch;
        }
    }
}
