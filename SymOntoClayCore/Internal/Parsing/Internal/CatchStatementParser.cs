using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CatchStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotCatchMark,
            WaitForVariable,
            GotVariable,
            ClosedVariableSection,
            GotWhereMark,
            GotCondition
        }

        public CatchStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstCatchStatement Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new AstCatchStatement();

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
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Catch:
                                    _state = State.GotCatchMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCatchMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ParseBody();
                            break;

                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForVariable:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            Result.VariableName = ParseName(_currToken.Content);
                            _state = State.GotVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotVariable:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            _state = State.ClosedVariableSection;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ClosedVariableSection:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ParseBody();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Where:
                                    _state = State.GotWhereMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotWhereMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

                                Result.Condition = parser.Result;

                                _state = State.GotCondition;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ParseBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ParseBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

            Result.Statements = parser.Result;

            Exit();
        }
    }
}
