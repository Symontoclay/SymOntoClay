using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class IndexedBaseRulePart: IndexedAnnotatedItem
    {
        public abstract BaseRulePart OriginRulePart { get; }

        public IndexedRuleInstance Parent { get; set; }

        public bool IsActive { get; set; }
        public bool HasVars { get; set; }
        public bool HasQuestionVars { get; set; }

        public BaseIndexedLogicalQueryNode Expression { get; set; }

        public IDictionary<ulong, IList<RelationIndexedLogicalQueryNode>> RelationsDict { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            var nextN = n + 4;
            var nextNSpace = DisplayHelper.Spaces(nextN);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.PrintObjProp(n, nameof(Expression), Expression);

            if (RelationsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(RelationsDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(RelationsDict)}");
                var nextNextN = nextN + 4;
                foreach (var relationsKVPItem in RelationsDict)
                {
                    sb.AppendLine($"{nextNSpace}key of relation = {relationsKVPItem.Key}");
                    var tmpRelationsList = relationsKVPItem.Value;
                    sb.AppendLine($"{nextNSpace}count of relations = {tmpRelationsList.Count}");
                    foreach (var relation in tmpRelationsList)
                    {
                        sb.Append(relation.ToShortString(nextNextN));
                    }
                }
                sb.AppendLine($"{spaces}End {nameof(RelationsDict)}");
            }

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.PrintShortObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public void FillExecutingCardForCallingFromRelationForFact(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //LogInstance.Log($"queryExecutingCard = {queryExecutingCard}");
#endif

            var targetRelationsList = RelationsDict[queryExecutingCard.TargetRelation];

#if DEBUG
            //LogInstance.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            foreach (var targetRelation in targetRelationsList)
            {
                if (targetRelation.CountParams != queryExecutingCard.CountParams)
                {
                    continue;
                }

#if DEBUG
                //LogInstance.Log($"targetRelation = {targetRelation}");
#endif

                var paramsListOfTargetRelation = targetRelation.Params;

                var isFit = true;

                foreach (var knownInfo in queryExecutingCard.KnownInfoList)
                {
#if DEBUG
                    //LogInstance.Log($"knownInfo = {knownInfo}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //LogInstance.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        var resultOfComparison = CompareKnownInfoAndExpressionNode(knownInfo, paramOfTargetRelation, options.Logger);

#if DEBUG
                        //LogInstance.Log($"resultOfComparison = {resultOfComparison}");
#endif

                        if (!resultOfComparison)
                        {
                            isFit = false;
                            break;
                        }
                    }
                    else
                    {
                        isFit = false;
                        break;
                    }
                }

#if DEBUG
                //LogInstance.Log($"isFit = {isFit}");
#endif

                if (isFit)
                {
                    var resultOfQueryToRelation = new ResultOfQueryToRelation();

                    var isEntityIdOnly = options.EntityIdOnly;
                    var useAccessPolicy = options.UseAccessPolicy;

                    if (useAccessPolicy)
                    {
                        throw new NotImplementedException();

//                        foreach (var accessPolicy in options.AccessPolicyToFactModalityList)
//                        {
//#if DEBUG
//                            //LogInstance.Log($"accessPolicy = {accessPolicy}");
//#endif

//                            if (!Parent.AccessPolicyToFactModality.Any(p => p.Kind == accessPolicy.Kind))
//                            {
//                                return;
//                            }
//                        }
                    }

                    foreach (var varItem in queryExecutingCard.VarsInfoList)
                    {
#if DEBUG
                        //LogInstance.Log($"varItem = {varItem}");
#endif

                        var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

#if DEBUG
                        //LogInstance.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                        {
                            continue;
                        }

#if DEBUG
                        //LogInstance.Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                        resultOfVarOfQueryToRelation.KeyOfVar = varItem.KeyOfVar;
                        resultOfVarOfQueryToRelation.FoundExpression = paramOfTargetRelation;
                        resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                        var originInfo = new OriginOfVarOfQueryToRelation();
                        originInfo.IndexedRuleInstance = Parent;
                        originInfo.IndexedRulePart = this;

                        var keyOfRuleInstance = Parent.Key;

                        originInfo.KeyOfRuleInstance = keyOfRuleInstance;

                        resultOfVarOfQueryToRelation.OriginDict[keyOfRuleInstance] = originInfo;
                    }

                    if (resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Count == 0)
                    {
                        continue;
                    }

                    queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }
            }
        }

        private bool CompareKnownInfoAndExpressionNode(QueryExecutingCardAboutKnownInfo knownInfo, BaseIndexedLogicalQueryNode expressionNode, IEntityLogger logger)
        {
            var knownInfoExpression = knownInfo.Expression;

            return ExpressionNodeHelper.Compare(knownInfoExpression, expressionNode, logger);
        }
    }
}
