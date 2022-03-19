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

            var result = parser.Result;

#if DEBUG
            Log($"result = {result}");
#endif

            var kindOfStatement = result;

            switch (kindOfStatement)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStatement), kindOfStatement, null);
            }

            throw new NotImplementedException();
        }        
    }
}
