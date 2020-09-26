using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public abstract IList<IndexedBaseRulePart> GetNextPartsList();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ Expression.GetLongHashCode();
        }

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
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //foreach(var item in queryExecutingCard.KnownInfoList)
            //{
            //    options.Logger.Log($"options.EntityDictionary.GetName(item.Key) = {options.EntityDictionary.GetName(item.Key)}");
            //}
#endif

            var useInheritance = options.UseInheritance;
            var inheritanceResolver = options.InheritanceResolver;

            var targetRelationsList = RelationsDict[queryExecutingCard.TargetRelation];

#if DEBUG
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            foreach (var targetRelation in targetRelationsList)
            {
                if (targetRelation.CountParams != queryExecutingCard.CountParams)
                {
                    continue;
                }

#if DEBUG
                //options.Logger.Log($"targetRelation = {targetRelation}");
                //options.Logger.Log($"targetRelation = {targetRelation.GetHumanizeDbgString()}");
#endif

                var paramsListOfTargetRelation = targetRelation.Params;

                var isFit = true;

                foreach (var knownInfo in queryExecutingCard.KnownInfoList)
                {
#if DEBUG
                    //options.Logger.Log($"knownInfo = {knownInfo}");
#endif

                    List<ulong> additionalKeys = null;

                    if(useInheritance)
                    {
                        additionalKeys = inheritanceResolver.GetSuperClassesKeysList(knownInfo.Key, options.LocalCodeExecutionContext);
                    }

#if DEBUG
                    //options.Logger.Log($"additionalKeys = {JsonConvert.SerializeObject(additionalKeys, Formatting.Indented)}");

                    //var tmpNamesList = additionalKeys.Select(p => options.EntityDictionary.GetName(p)).ToList();

                    //options.Logger.Log($"tmpNamesList = {JsonConvert.SerializeObject(tmpNamesList, Formatting.Indented)}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //options.Logger.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        var resultOfComparison = CompareKnownInfoAndExpressionNode(knownInfo, paramOfTargetRelation, additionalKeys, options.Logger);

#if DEBUG
                        //options.Logger.Log($"resultOfComparison = {resultOfComparison}");
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
                //options.Logger.Log($"isFit = {isFit}");
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

#if DEBUG
                    //options.Logger.Log($"queryExecutingCard.VarsInfoList.Count = {queryExecutingCard.VarsInfoList.Count}");
#endif

                    if(queryExecutingCard.VarsInfoList.Any())
                    {
                        foreach (var varItem in queryExecutingCard.VarsInfoList)
                        {
#if DEBUG
                            //options.Logger.Log($"varItem = {varItem}");
#endif

                            var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

#if DEBUG
                            //options.Logger.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                            if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                            {
                                continue;
                            }

#if DEBUG
                            //options.Logger.Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation}");
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

                        if (resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Count != queryExecutingCard.VarsInfoList.Count)
                        {
                            continue;
                        }

                        queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);

                        queryExecutingCard.IsSuccess = true;
                    }
                    else
                    {
                        queryExecutingCard.IsSuccess = true;
                    }
                }
            }
        }

        private bool CompareKnownInfoAndExpressionNode(QueryExecutingCardAboutKnownInfo knownInfo, BaseIndexedLogicalQueryNode expressionNode, List<ulong> additionalKeys, IEntityLogger logger)
        {
            var knownInfoExpression = knownInfo.Expression;

            return ExpressionNodeHelper.Compare(knownInfoExpression, expressionNode, additionalKeys, logger);
        }

        public void FillExecutingCardForCallingFromRelationForProduction(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif

            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;

            var targetRelationsList = RelationsDict[queryExecutingCard.TargetRelation];

#if DEBUG
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            if (targetRelationsList.Count != 1)
            {
                return;
            }

            var targetRelation = targetRelationsList.First();

#if DEBUG
            //options.Logger.Log($"targetRelation = {targetRelation}");
#endif

            if (targetRelation.Params.Count != queryExecutingCard.CountParams)
            {
                return;
            }

            var targetRelationVarsInfoList = targetRelation.VarsInfoList;

#if DEBUG
            //options.Logger.Log($"targetRelationVarsInfoList.Count = {targetRelationVarsInfoList.Count}");
            //foreach (var varInfo in targetRelationVarsInfoList)
            //{
            //    options.Logger.Log($"varInfo = {varInfo}");
            //}
#endif

            var targetRelationVarsInfoDictByPosition = targetRelationVarsInfoList.ToDictionary(p => p.Position, p => p.KeyOfVar);

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(targetRelation.KnownInfoList, targetRelationVarsInfoList, queryExecutingCard.KnownInfoList, true);
            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

#if DEBUG
            //options.Logger.Log($"########################targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    options.Logger.Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
#endif

            var nextPartsList = GetNextPartsList();

#if DEBUG
            options.Logger.Log($"nextPartsList.Count = {nextPartsList.Count}");
#endif

            foreach(var nextPart in nextPartsList)
            {
                var queryExecutingCardForNextPart = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForNextPart.VarsInfoList = targetRelation.VarsInfoList;
                queryExecutingCardForNextPart.KnownInfoList = targetKnownInfoList;
                queryExecutingCardForNextPart.SenderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
                queryExecutingCardForNextPart.SenderIndexedRulePart = this;
                nextPart.FillExecutingCardForCallingFromOtherPart(queryExecutingCardForNextPart, dataSource, options);

#if DEBUG
                //options.Logger.Log($"queryExecutingCardForNextPart = {queryExecutingCardForNextPart}");
                //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                //options.Logger.Log($"queryExecutingCardForNextPart.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderExpressionNodeHumanizeDbgString()}");
                //options.Logger.Log($"queryExecutingCardForNextPart.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderIndexedRulePartHumanizeDbgString()}");
                //options.Logger.Log($"queryExecutingCardForNextPart.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
                //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
                //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
                //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
#endif

                queryExecutingCard.IsSuccess = queryExecutingCardForNextPart.IsSuccess;

                var resultsOfQueryToRelationList = queryExecutingCardForNextPart.ResultsOfQueryToRelationList;

                if (resultsOfQueryToRelationList.Count > 0)
                {
                    var varsInfoList = queryExecutingCard.VarsInfoList;

                    var backKeysDict = new Dictionary<ulong, ulong>();

                    foreach (var varInfo in varsInfoList)
                    {
#if DEBUG
                        //options.Logger.Log($"varInfo = {varInfo}");
#endif

                        var targetInternalKeyOfVar = targetRelationVarsInfoDictByPosition[varInfo.Position];

#if DEBUG
                        //options.Logger.Log($"targetInternalKeyOfVar = {targetInternalKeyOfVar}");
#endif

                        backKeysDict[targetInternalKeyOfVar] = varInfo.KeyOfVar;
                    }

                    foreach (var resultOfQueryToRelation in resultsOfQueryToRelationList)
                    {
                        var newResultOfQueryToRelation = new ResultOfQueryToRelation();
                        var newResultOfVarOfQueryToRelationList = new List<ResultOfVarOfQueryToRelation>();

                        foreach (var resultOfVarOfQueryToRelation in resultOfQueryToRelation.ResultOfVarOfQueryToRelationList)
                        {
#if DEBUG
                            //options.Logger.Log($"resultOfQueryToRelation = {resultOfQueryToRelation}");
#endif

                            var internalKeyOfVar = resultOfVarOfQueryToRelation.KeyOfVar;

#if DEBUG
                            //options.Logger.Log($"internalKeyOfVar = {internalKeyOfVar}/'{options.EntityDictionary.GetName(internalKeyOfVar)}'");
#endif

                            if (backKeysDict.ContainsKey(internalKeyOfVar))
                            {
                                var externalKeyOfVar = backKeysDict[internalKeyOfVar];

#if DEBUG
                                //options.Logger.Log($"externalKeyOfVar = {externalKeyOfVar}/'{options.EntityDictionary.GetName(externalKeyOfVar)}'");
                                //options.Logger.Log($"resultOfVarOfQueryToRelation before = {resultOfVarOfQueryToRelation}");
#endif

                                resultOfVarOfQueryToRelation.KeyOfVar = externalKeyOfVar;

#if DEBUG
                                //options.Logger.Log($"resultOfVarOfQueryToRelation after = {resultOfVarOfQueryToRelation}");
#endif

                                newResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);
                            }
                        }

                        if (newResultOfVarOfQueryToRelationList.Count == 0)
                        {
                            continue;
                        }

                        newResultOfQueryToRelation.ResultOfVarOfQueryToRelationList = newResultOfVarOfQueryToRelationList;
                        queryExecutingCard.ResultsOfQueryToRelationList.Add(newResultOfQueryToRelation);                        
                    }
                }
            }

#if DEBUG
            //options.Logger.Log($"+++++++++queryExecutingCard = {queryExecutingCard}");
#endif
#if DEBUG
            //options.Logger.Log("End");
#endif
        }

        public void FillExecutingCardForCallingFromOtherPart(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin ^&*^&*");
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif
            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForExpression.SenderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            queryExecutingCardForExpression.SenderIndexedRulePart = this;
            queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
            Expression.FillExecutingCard(queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            //options.Logger.Log($"%%%%%%%% queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            queryExecutingCard.ResultsOfQueryToRelationList = queryExecutingCardForExpression.ResultsOfQueryToRelationList;
            queryExecutingCard.IsSuccess = queryExecutingCardForExpression.IsSuccess;

#if DEBUG
            //options.Logger.Log("End");
#endif
        }
    }
}
