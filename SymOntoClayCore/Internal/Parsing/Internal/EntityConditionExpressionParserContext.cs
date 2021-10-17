using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EntityConditionExpressionParserContext
    {
        public EntityConditionExpressionParserContext(InternalParserContext internalParserContext)
        {
            _internalParserContext = internalParserContext;
        }

        private InternalParserContext _internalParserContext;

        public InternalParserContext InternalParserContext => _internalParserContext;

        public Dictionary<StrongIdentifierValue, EntityConditionExpressionNode> AliasesDict { get; set; } = new Dictionary<StrongIdentifierValue, EntityConditionExpressionNode>();
    }
}
