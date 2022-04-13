using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InlineTriggerActiveRuleContentParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotItem,
            WaitForItem
        }

        public InlineTriggerActiveRuleContentParser(InternalParserContext context)
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
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessRuleInstance();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessRuleInstance();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForItem;
                            break;

                        default:
                            _context.Recovery(_currToken);
                            Exit();
                            break;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessRuleInstance()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            Result.Add(parser.Result);

            _state = State.GotItem;
        }
    }
}
