using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalExpressionParserContext
    {
        public LogicalExpressionParserContext(InternalParserContext internalParserContext)
        {
            _internalParserContext = internalParserContext;
        }

        private InternalParserContext _internalParserContext;

        public InternalParserContext InternalParserContext => _internalParserContext;

        public Dictionary<StrongIdentifierValue, LogicalQueryNode> AliasesDict { get; set; } = new Dictionary<StrongIdentifierValue, LogicalQueryNode>();
    }
}
