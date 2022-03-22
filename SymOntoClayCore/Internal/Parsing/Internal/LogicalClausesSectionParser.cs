using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalClausesSectionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotColon
        }

        public LogicalClausesSectionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<RuleInstance> Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<RuleInstance>();
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
                        case TokenKind.Colon:
                            _state = State.GotColon;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotColon:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new LogicalQueryAsCodeEntityParser(_context);
                                parser.Run();

#if DEBUG
                                Log($"parser.Result = {parser.Result}");
#endif

                                Result.Add(parser.Result);
                            }
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                case KeyWordTokenKind.Leave:
                                case KeyWordTokenKind.Var:
                                case KeyWordTokenKind.Operator:
                                case KeyWordTokenKind.Fun:
                                case KeyWordTokenKind.Public:
                                case KeyWordTokenKind.Protected:
                                case KeyWordTokenKind.Private:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.Var:
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
    }
}
