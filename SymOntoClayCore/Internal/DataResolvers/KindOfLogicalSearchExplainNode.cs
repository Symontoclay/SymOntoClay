﻿using System;
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
        RelationQuestionQuery,
        ProcessRelationWithDirectFactsCollector,
        ProcessRelationWithProductionCollector,
        RelationWithProductionQuery,
        MergedKnownInfoCollector,
        KnownInfoResult,
        MergeKnownInfoBlock,
        KnownInfoDataSource,
        RelationWithProductionNextPartsCollector,
        RelationWithProductionNextPart,
        RelationWithDirectFactQuery,
        RelationWithDirectFactQueryProcessTargetRelation,
        PostFilterWithAndStrategy,
        OperatorQuery,
        GroupQuery,
        FetchingAllValuesForResolvingExpressionParam
    }
}
