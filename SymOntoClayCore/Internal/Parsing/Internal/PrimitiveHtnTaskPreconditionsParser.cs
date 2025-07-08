using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveHtnTaskPreconditionsParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public PrimitiveHtnTaskPreconditionsParser(InternalParserContext context)
            : base(context)
        {
        }

        public LogicalExecutableExpression Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            throw new NotImplementedException("0C1C7D4C-BD91-4EF0-A7BE-3ABBB7A33160");
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Info("D99532FA-6FA7-444A-B6DD-678033D29E63", $"_state = {_state}");
            Info("A4A5D717-394B-4067-8C78-4F96BC1DC9BC", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}
