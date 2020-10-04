using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryOperationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotKindOfLogicalQueryOperation,
            GotTarget
        }

        public LogicalQueryOperationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public LogicalQueryOperationValue Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new LogicalQueryOperationValue();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.QuestionMark:
                            Result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation.Select;
                            _state = State.GotKindOfLogicalQueryOperation;
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Select:
                                    Result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation.Select;
                                    _state = State.GotKindOfLogicalQueryOperation;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotKindOfLogicalQueryOperation:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

                                var ruleInstanceItem = parser.Result;

#if DEBUG
                                //Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

                                var value = new RuleInstanceValue(ruleInstanceItem);

                                Result.Target = value;

                                _state = State.GotTarget;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTarget:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LeftRightStream:
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
