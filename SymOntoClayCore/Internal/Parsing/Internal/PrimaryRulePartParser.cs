using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimaryRulePartParser: RulePartParser
    {
        public PrimaryRulePartParser(InternalParserContext context, TokenKind terminatingTokenKind)
            : base(context, terminatingTokenKind)
        {
        }

        public PrimaryRulePart Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new PrimaryRulePart();
            _baseRulePart = Result;
        }
    }
}
