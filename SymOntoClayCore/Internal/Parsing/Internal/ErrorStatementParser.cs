using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class ErrorStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotError,
            GotFact
        }

        public ErrorStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private AstErrorStatement _rawStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new AstErrorStatement();

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
                                case KeyWordTokenKind.Error:
                                    _state = State.GotError;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotError:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

#if DEBUG
                                Log($"parser.Result = {parser.Result}");
#endif

                                _rawStatement.RuleInstanceValue = new RuleInstanceValue(parser.Result);

                                _state = State.GotFact;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFact:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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
    }
}
