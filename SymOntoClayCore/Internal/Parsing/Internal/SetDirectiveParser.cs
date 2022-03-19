using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class SetDirectiveParser : BaseInternalParser
    {
        public SetDirectiveParser(InternalParserContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
#endif
            _context.Recovery(_currToken);
            var parser = new SetStatementParser(_context);
            parser.Run();

#if DEBUG
            Log($"parser.Result = {parser.Result}");
#endif

            throw new NotImplementedException();
        }
    }
}
