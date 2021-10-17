using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EntityConditionExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, bool isGroup)
            : base(context.InternalParserContext)
        {
            if (isGroup)
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.CloseRoundBracket };
            }
            else
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket };
            }

            _logicalExpressionParserContext = context;
            _isGroup = isGroup;
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context)
            : this(context, new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket })
        {
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, TokenKind terminatingTokenKind)
            : this(context, new List<TokenKind>() { terminatingTokenKind })
        {
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, List<TokenKind> terminatingTokenKindList)
            : base(context.InternalParserContext)
        {
            _terminatingTokenKindList = terminatingTokenKindList;
            _logicalExpressionParserContext = context;
        }

        private bool _isGroup;
        private List<TokenKind> _terminatingTokenKindList = new List<TokenKind>();
        private EntityConditionExpressionParserContext _logicalExpressionParserContext;
        private State _state = State.Init;

        public EntityConditionExpressionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<EntityConditionExpressionNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            if (_terminatingTokenKindList.Contains(_currToken.TokenKind))
            {
                _context.Recovery(_currToken);
                Exit();
                return;
            }

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
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
