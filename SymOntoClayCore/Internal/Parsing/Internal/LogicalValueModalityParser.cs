using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalValueModalityParser : BaseInternalParser
    {
        public LogicalValueModalityParser(InternalParserContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
#endif

            throw new NotImplementedException();
        }
    }
}
