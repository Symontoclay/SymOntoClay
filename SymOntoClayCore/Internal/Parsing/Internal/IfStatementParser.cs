using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class IfStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotIfMark,
            WaitForIfCondition,
            GotIfCondition,
            GotIfBody
        }

        public IfStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private AstIfStatement _rawStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new AstIfStatement();

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
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.If:
                                    _state = State.GotIfMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIfMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForIfCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForIfCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            _rawStatement.Condition = ProcessCondition();
                            _state = State.GotIfCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIfCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _rawStatement.IfStatements = ParseBody();
                            _state = State.GotIfBody;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIfBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
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

        private AstExpression ProcessCondition()
        {
            _context.Recovery(_currToken);
            var parser = new CodeExpressionStatementParser(_context);
            parser.Run();

#if DEBUG
            Log($"parser.Result = {parser.Result}");
#endif

            return parser.Result.Expression;
        }

        private List<AstStatement> ParseBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();

#if DEBUG
            Log($"parser.Result.WriteListToString() = {parser.Result.WriteListToString()}");
#endif

            return parser.Result;
        }
    }
}
