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
        ResultCollector,
        DataSourceResult,
        DataSourceCollector,
        ConsolidatedDataSource,
        LogicalStorage,
        LogicalStorageFilter,
        ExcludeRejectedFacts,
        RuleInstanceQuery,
        PrimaryRulePartQuery,
        RelationQuery,
        ProcessRelationWithDirectFactsCollector,
        ProcessRelationWithProductionCollector,
        RelationWithProductionQuery
    }
}
