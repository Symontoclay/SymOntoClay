using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FunctionParametersParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public FunctionParametersParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            throw new NotImplementedException();
        }
    }
}
