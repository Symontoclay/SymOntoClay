using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class SecondaryRulePartParser : RulePartParser
    {
        public SecondaryRulePartParser(InternalParserContext context, TokenKind terminatingTokenKind)
            : base(context, terminatingTokenKind)
        {
        }

        public SecondaryRulePart Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new SecondaryRulePart();
            _baseRulePart = Result;
        }
    }
}
