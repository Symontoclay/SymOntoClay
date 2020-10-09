/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        public List<AstStatement> Result { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");
            //Log($"_state = {_state}");
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
                                    ProcessExpressionStatement();
                                    break;

                                case KeyWordTokenKind.Use:
                                    ProcessUseStatement();
                                    break;
                                    
                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
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

        private void AddStatement(AstStatement statement)
        {
            if (statement != null)
            {
                Result.Add(statement);
            }
        }
    }
}
