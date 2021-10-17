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
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.CloseFactBracket };
            }
            else
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket };
            }

            _logicalExpressionParserContext = context;
            _isGroup = isGroup;
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context)
        {
            throw new NotImplementedException();
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context)
        {
            throw new NotImplementedException();
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, List<TokenKind> terminatingTokenKindList)
            : base(context)
        {
            throw new NotImplementedException();
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

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
