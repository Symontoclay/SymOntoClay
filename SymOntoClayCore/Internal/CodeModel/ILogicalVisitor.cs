using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface ILogicalVisitor
    {
        void VisitRuleInstance(RuleInstance ruleInstance);
        void VisitPrimaryRulePart(PrimaryRulePart primaryRulePart);
        void VisitSecondaryRulePart(SecondaryRulePart secondaryRulePart);
        void VisitLogicalQueryNode(LogicalQueryNode logicalQueryNode);
    }
}
