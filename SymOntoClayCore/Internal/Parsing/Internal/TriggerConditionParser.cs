using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TriggerConditionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotDurationMark
        }

        public TriggerConditionParser(InternalParserContext context, bool setCondition)
            : base(context)
        {
            _setCondition = setCondition;
        }

        private readonly bool _setCondition;

        private State _state = State.Init;

        public TriggerConditionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(Result == null)
            {
                Result = _nodePoint.BuildExpr<TriggerConditionNode>();
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            Result = ParseFact();
                            Exit();
                            break;

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
                                Log($"parser.Result = {parser.Result}");
#endif

                                Result = CreateDuration(parser.Result);

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

        private TriggerConditionNode ParseFact()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Fact };
            conditionNode.RuleInstance = parser.Result;

            return conditionNode;
        }

        private TriggerConditionNode CreateDuration(Value value)
        {
            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Duration };
            conditionNode.Value = value;

            return conditionNode;
        }
    }
}
