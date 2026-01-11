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
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EventDeclStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotOnMark,
            GotExpression,
            GotKindOfEvent,
            WaitForAction,
            GotAction
        }

        public EventDeclStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private static TerminationToken[] _exprTerminationTokensList = new List<TerminationToken>() 
        {
            new TerminationToken(TokenKind.Word, KeyWordTokenKind.Complete, true),
            new TerminationToken(TokenKind.Word, KeyWordTokenKind.Completed, true),
            new TerminationToken(TokenKind.Word, KeyWordTokenKind.Weak, true)
        }.ToArray();

        private State _state = State.Init;

        public AstEventDeclStatement Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new AstEventDeclStatement();

            DefaultSettingsOfCodeEntityHelper.SetUpAnnotatedItem(Result, CurrentDefaultSetings);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.CheckDirty();
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
                                case KeyWordTokenKind.On:
                                    _state = State.GotOnMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                 case State.GotOnMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            {
                                _context.Recovery(_currToken);

                                var parser = new AstExpressionParser(_context, _exprTerminationTokensList);
                                parser.Run();

                                Result.Expression = parser.Result;

                                _state = State.GotExpression;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotExpression:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Complete:
                                case KeyWordTokenKind.Completed:
                                    Result.KindOfLifeCycleEvent = ParseName(_currToken.Content);
                                    _state = State.GotKindOfEvent;
                                    break;

                                case KeyWordTokenKind.Weak:
                                    {
                                        var nextToken = _context.GetToken();

                                        switch (nextToken.TokenKind)
                                        {
                                            case TokenKind.Word:
                                                switch (nextToken.KeyWordTokenKind)
                                                {
                                                    case KeyWordTokenKind.Cancel:
                                                    case KeyWordTokenKind.Canceled:
                                                        Result.KindOfLifeCycleEvent = ParseName($"{_currToken.Content} {nextToken.Content}");

                                                        _state = State.GotKindOfEvent;
                                                        break;

                                                    default:
                                                        throw new UnexpectedTokenException(Text, _currToken);
                                                }
                                                break;

                                            default:
                                                throw new UnexpectedTokenException(nextToken);
                                        }
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotKindOfEvent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            break;

                        default:
                            _context.Recovery(_currToken);
                            break;
                    }
                    Exit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ProcessFunctionBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();

            var statementsList = parser.Result;

            var function = new NamedFunction() { TypeOfAccess = TypeOfAccess.Local, IsAnonymous = true };
            function.Statements = statementsList;
            function.CompiledFunctionBody = _context.Compiler.Compile(statementsList);

            Result.Handler = function;

            _state = State.GotAction;
        }
    }
}
