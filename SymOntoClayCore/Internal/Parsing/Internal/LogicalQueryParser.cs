using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForContent,
            WaitForPrimaryRulePart
        }

        public LogicalQueryParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public RuleInstance Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new RuleInstance();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            switch(_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            _state = State.WaitForContent;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForContent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.PrimaryLogicalPartMark:
                            _state = State.WaitForPrimaryRulePart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForPrimaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);

                                var paser = new PrimaryRulePartParser(_context, TokenKind.CloseFigureBracket);
                                paser.Run();

#if DEBUG
                                Log($"paser.Result = {paser.Result}");
#endif

                                throw new NotImplementedException();
                            }
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
/*

*/