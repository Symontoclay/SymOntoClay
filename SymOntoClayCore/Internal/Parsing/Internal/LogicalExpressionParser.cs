using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public LogicalExpressionParser(InternalParserContext context, TokenKind terminatingTokenKind)
            : base(context)
        {
            _terminatingTokenKind = terminatingTokenKind;
        }

        private TokenKind _terminatingTokenKind;
        private State _state = State.Init;

        public LogicalQueryNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private LogicalQueryNode _lastLogicalQueryNode;

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
                        case TokenKind.Word:
                            ProcessWord();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessWord()
        {
            var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
#endif

            throw new NotImplementedException();
        }
    }
}
