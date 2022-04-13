using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TriggerConditionDurationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotDurationMark
        }

        public TriggerConditionDurationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public TriggerConditionNode Result { get; private set; }

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
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Duration:
                                    _state = State.GotDurationMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotDurationMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif
                                var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Duration };
                                conditionNode.Value = parser.Result;

                                Result = conditionNode;

                                Exit();
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
