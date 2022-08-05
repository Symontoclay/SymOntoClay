using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public enum KindOfLogicalSearchExplainNode
    {
        Unknown,
        Root,
        Result,
        RuleInstanceQuery,
        PrimaryRulePartQuery
    }
}
