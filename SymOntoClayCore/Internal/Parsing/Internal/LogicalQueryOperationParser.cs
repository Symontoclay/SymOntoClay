using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryOperationParser : BaseInternalParser
    {
        private enum State
        {
            Init
        }

        public LogicalQueryOperationParser(InternalParserContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            throw new NotImplementedException();
        }

        private State _state = State.Init;

        protected override void OnRun()
        {

        }
    }
}
