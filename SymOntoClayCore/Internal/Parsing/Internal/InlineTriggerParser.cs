using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InlineTriggerParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public InlineTriggerParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public InlineTrigger Result { get; set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new InlineTrigger();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            switch (_state)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
