using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AliasPaser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public AliasPaser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<StrongIdentifierValue> Result { get; private set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
