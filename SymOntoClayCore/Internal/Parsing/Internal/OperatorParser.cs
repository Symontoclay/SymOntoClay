using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class OperatorParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public OperatorParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
