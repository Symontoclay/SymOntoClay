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
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");          
#endif

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
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
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

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                Result.Expression = parser.Result;

                                _state = State.GotExpression;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
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

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
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
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
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
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
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
