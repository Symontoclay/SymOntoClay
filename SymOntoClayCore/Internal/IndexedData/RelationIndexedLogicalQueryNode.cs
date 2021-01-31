/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class RelationIndexedLogicalQueryNode: BaseIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.Relation;

        public ulong Key { get; set; }
        public int CountParams { get; set; }
        public bool IsQuestion { get; set; }
        public IList<BaseIndexedLogicalQueryNode> Params { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseFunctionWeight ^ Key;

            foreach(var param in Params)
            {
                result ^= LongHashCodeWeights.BaseParamWeight ^ param.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        public override void CalculateUsedKeys(List<ulong> usedKeysList)
        {
            usedKeysList.Add(Key);

            foreach (var param in Params)
            {
                param.CalculateUsedKeys(usedKeysList);
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintObjListProp(n, nameof(Params), Params);
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintShortObjListProp(n, nameof(Params), Params);
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintBriefObjListProp(n, nameof(Params), Params);
            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"IsQuestion = {IsQuestion}");
#endif

            if (IsQuestion)
            {
                FillExecutingCardForQuestion(queryExecutingCard, dataSource, options);
                return;
            }

#if DEBUG
            //options.Logger.Log($"Key = {Key}");
            //options.Logger.Log($"IsQuestion = {IsQuestion}");
            //options.Logger.Log($"Params.Count = {Params.Count}");
            //foreach (var param in Params)
            //{
            //    options.Logger.Log($"param = {param}");
            //}
            //options.Logger.Log($"VarsInfoList.Count = {VarsInfoList.Count}");
            //foreach (var varInfo in VarsInfoList)
            //{
            //    options.Logger.Log($"varInfo = {varInfo}");
            //}
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif

            NFillExecutingCard(queryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"^^^^^^queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"GetHumanizeDbgString() = {GetHumanizeDbgString()}");

            ////throw new NotImplementedException();

            //options.Logger.Log("End");
#endif
        }

        private void NFillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif

            var indexedRulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(Key);

#if DEBUG
            //options.Logger.Log($"indexedRulePartsOfFactsList?.Count = {indexedRulePartsOfFactsList?.Count}");
#endif

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(KnownInfoList, VarsInfoList, queryExecutingCard.KnownInfoList, false);

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

            queryExecutingCard.UsedKeysList.Add(Key);

#if DEBUG
            //options.Logger.Log($"targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    options.Logger.Log($"tmpKnownInfo = {tmpKnownInfo}");
            //    options.Logger.Log($"options.EntityDictionary.GetName(tmpKnownInfo.Key) = {options.EntityDictionary.GetName(tmpKnownInfo.Key)}");
            //}
            //if(targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            if (!indexedRulePartsOfFactsList.IsNullOrEmpty())
            {
                foreach (var indexedRulePartsOfFacts in indexedRulePartsOfFactsList)
                {
#if DEBUG
                    //options.Logger.Log($"this = {this}");
                    //options.Logger.Log($"indexedRulePartsOfFacts = {indexedRulePartsOfFacts}");
#endif
                    var queryExecutingCardForTargetFact = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetFact.TargetRelation = Key;
                    queryExecutingCardForTargetFact.CountParams = CountParams;
                    queryExecutingCardForTargetFact.VarsInfoList = VarsInfoList;
                    queryExecutingCardForTargetFact.KnownInfoList = targetKnownInfoList;
#if DEBUG
                    queryExecutingCardForTargetFact.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    queryExecutingCardForTargetFact.SenderIndexedRulePart = senderIndexedRulePart;
                    queryExecutingCardForTargetFact.SenderExpressionNode = this;
#endif

                    indexedRulePartsOfFacts.FillExecutingCardForCallingFromRelationForFact(queryExecutingCardForTargetFact, dataSource, options);

#if DEBUG
                    //options.Logger.Log($"++++++queryExecutingCardForTargetFact = {queryExecutingCardForTargetFact}");
                    //if (queryExecutingCardForTargetFact.UsedKeysList.Any())
                    //{
                    //    throw new NotImplementedException();
                    //}                    
#endif
                    queryExecutingCard.IsSuccess = queryExecutingCardForTargetFact.IsSuccess;

                    foreach (var resultOfQueryToRelation in queryExecutingCardForTargetFact.ResultsOfQueryToRelationList)
                    {
                        queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                    }

                    queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForTargetFact.UsedKeysList);
                }
            }

#if DEBUG
            //options.Logger.Log($"~~~~~~~~~~~~~~~~~queryExecutingCard = {queryExecutingCard}");
#endif

            var indexedRulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(Key);

#if DEBUG
            //options.Logger.Log($"indexedRulePartWithOneRelationsList?.Count = {indexedRulePartWithOneRelationsList?.Count}");
#endif

            if (!indexedRulePartWithOneRelationsList.IsNullOrEmpty())
            {
                foreach (var indexedRulePartsOfRule in indexedRulePartWithOneRelationsList)
                {
#if DEBUG
                    //options.Logger.Log($"this = {this}");
                    //options.Logger.Log($"indexedRulePartsOfRule = {indexedRulePartsOfRule}");
#endif
                    var queryExecutingCardForTargetRule = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetRule.TargetRelation = Key;

#if DEBUG
                    //options.Logger.Log($"Key = {Key}");
                    //options.Logger.Log($"options.EntityDictionary.GetName(Key) = {options.EntityDictionary.GetName(Key)}");
#endif

                    queryExecutingCardForTargetRule.CountParams = CountParams;
                    queryExecutingCardForTargetRule.VarsInfoList = VarsInfoList;
                    queryExecutingCardForTargetRule.KnownInfoList = targetKnownInfoList;
#if DEBUG
                    queryExecutingCardForTargetRule.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    queryExecutingCardForTargetRule.SenderIndexedRulePart = senderIndexedRulePart;
                    queryExecutingCardForTargetRule.SenderExpressionNode = this;
#endif
                    indexedRulePartsOfRule.FillExecutingCardForCallingFromRelationForProduction(queryExecutingCardForTargetRule, dataSource, options);

                    queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForTargetRule.UsedKeysList);

#if DEBUG
                    //options.Logger.Log($"&&&&&&&&&&&&&&&&&queryExecutingCardForTargetRule = {queryExecutingCardForTargetRule}");
#endif
#if DEBUG
                    //options.Logger.Log($"!!!!!!!!!!!!!!!!!!queryExecutingCard = {queryExecutingCard}");
                    //if (queryExecutingCardForTargetRule.UsedKeysList.Any())
                    //{
                    //    throw new NotImplementedException();
                    //}
#endif

                    queryExecutingCard.IsSuccess = queryExecutingCardForTargetRule.IsSuccess;

                    foreach (var resultOfQueryToRelation in queryExecutingCardForTargetRule.ResultsOfQueryToRelationList)
                    {
                        queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                    }
                }
            }

#if DEBUG
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCardForQuestion(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"Key = {Key}");
            //options.Logger.Log($"IsQuestion = {IsQuestion}");
            //options.Logger.Log($"Params.Count = {Params.Count}");
            //foreach (var param in Params)
            //{
            //    options.Logger.Log($"param = {param}");
            //}
            //options.Logger.Log($"VarsInfoList.Count = {VarsInfoList.Count}");
            //foreach (var varInfo in VarsInfoList)
            //{
            //    options.Logger.Log($"varInfo = {varInfo}");
            //}
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif

            var hasAnnotations = !Annotations.IsNullOrEmpty();

#if DEBUG
            //options.Logger.Log($"hasAnnotations = {hasAnnotations}");
#endif

            var targetRelationsList = dataSource.AllRelationsForProductions;

#if DEBUG
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
            //foreach (var targetRelation in targetRelationsList)
            //{
            //    options.Logger.Log($"targetRelation.GetHumanizeDbgString() = {targetRelation.GetHumanizeDbgString()}");
            //}
#endif

            foreach (var targetRelation in targetRelationsList)
            {
                if (targetRelation.CountParams != CountParams)
                {
                    continue;
                }
#if DEBUG
                //options.Logger.Log($"targetRelation.GetHumanizeDbgString() = {targetRelation.GetHumanizeDbgString()}");
                //options.Logger.Log($"targetRelation = {targetRelation}");
                //options.Logger.Log($"hasAnnotations = {hasAnnotations}");
#endif

                var isCheckAnnotation = false;

                if (hasAnnotations)
                {
                    if (targetRelation.Annotations.IsNullOrEmpty())
                    {
                        continue;
                    }

                    throw new NotImplementedException();
                }

                if (hasAnnotations && !isCheckAnnotation)
                {
                    continue;
                }

#if DEBUG
                //options.Logger.Log($"NEXT targetRelation.GetHumanizeDbgString() = {targetRelation.GetHumanizeDbgString()}");
                //options.Logger.Log($"NEXT targetRelation = {targetRelation}");
#endif

                var resultOfQueryToRelation = new ResultOfQueryToRelation();
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                queryExecutingCard.IsSuccess = true;

                {
                    var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                    resultOfVarOfQueryToRelation.KeyOfVar = Key;
                    resultOfVarOfQueryToRelation.FoundExpression = targetRelation;
                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                    var originInfo = new OriginOfVarOfQueryToRelation();
                    var targetRulePart = targetRelation.RulePart;
                    originInfo.IndexedRuleInstance = targetRelation.RuleInstance;
                    originInfo.IndexedRulePart = targetRulePart;

                    var keyOfRuleInstance = targetRelation.RuleInstance.Key;

                    originInfo.KeyOfRuleInstance = keyOfRuleInstance;

                    resultOfVarOfQueryToRelation.OriginDict[keyOfRuleInstance] = originInfo;
                }

                var n = 0;

                foreach (var param in Params)
                {
#if DEBUG
                    //options.Logger.Log($"n = {n} param = {param}");
#endif

                    n++;

                    if (param.Kind != KindOfLogicalQueryNode.QuestionVar)
                    {
                        continue;
                    }

                    var foundExpression = targetRelation.Params[n - 1];

                    var questionVarParam = param.AsQuestionVar;

                    var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                    resultOfVarOfQueryToRelation.KeyOfVar = questionVarParam.Key;
                    resultOfVarOfQueryToRelation.FoundExpression = foundExpression;
                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                    var originInfo = new OriginOfVarOfQueryToRelation();
                    var targetRulePart = targetRelation.RulePart;
                    originInfo.IndexedRuleInstance = targetRelation.RuleInstance;
                    originInfo.IndexedRulePart = targetRulePart;

                    var keyOfRuleInstance = targetRelation.RuleInstance.Key;

                    originInfo.KeyOfRuleInstance = keyOfRuleInstance;

                    resultOfVarOfQueryToRelation.OriginDict[keyOfRuleInstance] = originInfo;

#if DEBUG
                    //options.Logger.Log($"resultOfVarOfQueryToRelation = {resultOfVarOfQueryToRelation}");
                    //throw new NotImplementedException();
#endif
                }

#if DEBUG
                //options.Logger.Log($"resultOfQueryToRelation = {resultOfQueryToRelation}");
                //throw new NotImplementedException();
#endif
            }

#if DEBUG
            //LogInstance.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //LogInstance.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //LogInstance.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //LogInstance.Log($"GetHumanizeDbgString() = {GetHumanizeDbgString()}");
            //LogInstance.Log($"this = {this}");
#endif

#if DEBUG
            //throw new NotImplementedException();
            //options.Logger.Log("End");
#endif
        }
    }
}
