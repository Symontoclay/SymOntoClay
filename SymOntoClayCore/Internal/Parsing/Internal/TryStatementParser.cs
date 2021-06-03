﻿using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
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
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
#endif

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

#if DEBUG
                                //Log($"statementsList = {statementsList.WriteListToString()}");
#endif

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

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
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

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
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

#if DEBUG
                                //Log($"statementsList = {statementsList.WriteListToString()}");
#endif

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

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
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

#if DEBUG
                                //Log($"statementsList = {statementsList.WriteListToString()}");
#endif

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
                        case TokenKind.CloseFigureBracket:
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

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            _rawStatement.CatchStatements.Add(parser.Result);

            _state = State.GotCatch;
        }
    }
}
