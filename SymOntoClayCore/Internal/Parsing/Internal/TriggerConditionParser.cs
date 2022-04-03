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
            Init
        }

        public TriggerConditionParser(InternalParserContext context)
            : base(context)
        {
        }

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
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            Result = ParseFact();
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

        private TriggerConditionNode ParseFact()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Fact };
            conditionNode.RuleInstance = parser.Result;

            return conditionNode;
        }
    }
}
