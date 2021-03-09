/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public LogicalSearchResult Run(LogicalSearchOptions options)
        {
#if DEBUG
            //Log($"options = {options}");
#endif

            var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
            optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
            optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;
            optionsOfFillExecutingCard.UseInheritance = options.UseInheritance;
            optionsOfFillExecutingCard.InheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();
            optionsOfFillExecutingCard.LocalCodeExecutionContext = options.LocalCodeExecutionContext;


#if DEBUG
            optionsOfFillExecutingCard.Logger = Logger;
#endif

#if DEBUG
            //Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

            var result = new LogicalSearchResult();

#if DEBUG
            //var tmpStoragesList = GetStoragesList(options.LocalCodeExecutionContext.Storage);

            //Log($"tmpStoragesList.Count = {tmpStoragesList.Count}");
            //foreach(var tmpStorage in tmpStoragesList)
            //{
            //    Log($"tmpStorage.Storage = {tmpStorage.Storage}");
            //    tmpStorage.Storage.DbgPrintFactsAndRules();
            //}
#endif

            var dataSource = new ConsolidatedDataSource(GetStoragesList(options.LocalCodeExecutionContext.Storage));

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            FillExecutingCard(queryExpression, queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
            //Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif

            var usedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

#if DEBUG
            //Log($"usedKeysList.Count = {usedKeysList.Count}");
            //foreach(var usedKey in usedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

            result.UsedKeysList = usedKeysList;

            result.IsSuccess = queryExecutingCard.IsSuccess;

            var resultItemsList = new List<LogicalSearchResultItem>();

            foreach (var resultOfQueryToRelation in queryExecutingCard.ResultsOfQueryToRelationList)
            {
                var resultItem = new LogicalSearchResultItem();
                resultItem.ResultOfVarOfQueryToRelationList = resultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                resultItemsList.Add(resultItem);
            }

            result.Items = resultItemsList;

#if DEBUG
            //Log("End");
#endif

            return result;
        }

        private void FillExecutingCard(RuleInstance processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin");
#endif

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();

#if DEBUG
            queryExecutingCardForPart_1.SenderIndexedRuleInstance = processedExpr;
#endif

            FillExecutingCard(processedExpr.PrimaryPart, queryExecutingCardForPart_1, dataSource, options);

            queryExecutingCard.IsSuccess = queryExecutingCardForPart_1.IsSuccess;

            foreach (var resultOfQueryToRelation in queryExecutingCardForPart_1.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForPart_1.UsedKeysList);
#if DEBUG
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCard(PrimaryRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"Begin~~~~~~ GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif

#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
#endif
            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

#if DEBUG
            queryExecutingCardForExpression.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            queryExecutingCardForExpression.SenderIndexedRulePart = processedExpr;
#endif
            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            //options.Logger.Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            queryExecutingCard.IsSuccess = queryExecutingCardForExpression.IsSuccess;

            foreach (var resultOfQueryToRelation in queryExecutingCardForExpression.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

#if DEBUG
            //if (queryExecutingCardForExpression.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCard(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var kind = processedExpr.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    FillExecutingCardForRelationIndexedLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(processedExpr.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                            FillExecutingCardForAndOperatorIndexedLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(processedExpr.KindOfOperator), processedExpr.KindOfOperator, null);
                    }
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        //RelationIndexedLogicalQueryNode processedExpr
        private void FillExecutingCardForRelationIndexedLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"IsQuestion = {IsQuestion}");
#endif

            if (processedExpr.IsQuestion)
            {
                FillExecutingCardForQuestion(processedExpr, queryExecutingCard, dataSource, options);
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

            NFillExecutingCard(processedExpr, queryExecutingCard, dataSource, options);

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

        //RelationIndexedLogicalQueryNode processedExpr
        private void NFillExecutingCard(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {            
#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif

            var indexedRulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(processedExpr.Key);

#if DEBUG
            //options.Logger.Log($"indexedRulePartsOfFactsList?.Count = {indexedRulePartsOfFactsList?.Count}");
#endif

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

            queryExecutingCard.UsedKeysList.Add(processedExpr.Key);

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
                    queryExecutingCardForTargetFact.TargetRelation = processedExpr.Key;
                    queryExecutingCardForTargetFact.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetFact.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetFact.KnownInfoList = targetKnownInfoList;
#if DEBUG
                    queryExecutingCardForTargetFact.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    queryExecutingCardForTargetFact.SenderIndexedRulePart = senderIndexedRulePart;
                    queryExecutingCardForTargetFact.SenderExpressionNode = processedExpr;
#endif

                    FillExecutingCardForCallingFromRelationForFact(indexedRulePartsOfFacts, queryExecutingCardForTargetFact, dataSource, options);

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

            var indexedRulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(processedExpr.Key);

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
                    queryExecutingCardForTargetRule.TargetRelation = processedExpr.Key;

#if DEBUG
                    //options.Logger.Log($"Key = {Key}");
                    //options.Logger.Log($"options.EntityDictionary.GetName(Key) = {options.EntityDictionary.GetName(Key)}");
#endif

                    queryExecutingCardForTargetRule.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetRule.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetRule.KnownInfoList = targetKnownInfoList;
#if DEBUG
                    queryExecutingCardForTargetRule.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    queryExecutingCardForTargetRule.SenderIndexedRulePart = senderIndexedRulePart;
                    queryExecutingCardForTargetRule.SenderExpressionNode = processedExpr;
#endif
                    FillExecutingCardForCallingFromRelationForProduction(indexedRulePartsOfRule, queryExecutingCardForTargetRule, dataSource, options);

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

        //RelationIndexedLogicalQueryNode processedExpr
        private void FillExecutingCardForQuestion(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            var hasAnnotations = !processedExpr.Annotations.IsNullOrEmpty();

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
                if (targetRelation.CountParams != processedExpr.CountParams)
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
                    resultOfVarOfQueryToRelation.NameOfVar = processedExpr.Name;
                    resultOfVarOfQueryToRelation.FoundExpression = targetRelation;
                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                    var originInfo = new OriginOfVarOfQueryToRelation();
                    var targetRulePart = targetRelation.RulePart;
                    originInfo.RuleInstance = targetRelation.RuleInstance;
                    originInfo.RulePart = targetRulePart;

                    var keyOfRuleInstance = targetRelation.RuleInstance.Key;

                    originInfo.NameOfRuleInstance = keyOfRuleInstance;

                    resultOfVarOfQueryToRelation.OriginDict[keyOfRuleInstance] = originInfo;
                }

                var n = 0;

                foreach (var param in processedExpr.Params)
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
                    resultOfVarOfQueryToRelation.NameOfVar = questionVarParam.Key;
                    resultOfVarOfQueryToRelation.FoundExpression = foundExpression;
                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                    var originInfo = new OriginOfVarOfQueryToRelation();
                    var targetRulePart = targetRelation.RulePart;
                    originInfo.RuleInstance = targetRelation.RuleInstance;
                    originInfo.RulePart = targetRulePart;

                    var keyOfRuleInstance = targetRelation.RuleInstance.Key;

                    originInfo.NameOfRuleInstance = keyOfRuleInstance;

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

        //AndOperatorIndexedLogicalQueryNode processedExpr
        private void FillExecutingCardForAndOperatorIndexedLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {            
#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
            leftQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            leftQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
            leftQueryExecutingCard.SenderExpressionNode = processedExpr;
#endif
            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //if (leftQueryExecutingCard.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}

            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if (!leftQueryExecutingCard.IsSuccess)
            {
                return;
            }

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

                foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
                    rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
                    rightQueryExecutingCard.SenderExpressionNode = processedExpr;
#endif
                    rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                    FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

                    queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess;

                    queryExecutingCard.UsedKeysList.AddRange(rightQueryExecutingCard.UsedKeysList);

#if DEBUG

                    //if (rightQueryExecutingCard.UsedKeysList.Any())
                    //{
                    //    throw new NotImplementedException();
                    //}
                    //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

                    var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

                    if (rightQueryExecutingCardResultsOfQueryToRelationList.Count == 0)
                    {
                        continue;
                    }

                    var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var leftVarsKeysList = leftVarsList.Select(p => p.NameOfVar).Distinct().ToList();

                    foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                    {
                        var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                        var rightVarsKeysList = rightVarsList.Select(p => p.NameOfVar).Distinct().ToList();
                        var intersectOfVarsKeysList = leftVarsKeysList.Intersect(rightVarsKeysList).ToList();

#if DEBUG
                        //options.Logger.Log($"intersectOfVarsKeysList.Count = {intersectOfVarsKeysList.Count}");
#endif

                        var isFit = true;

                        if (intersectOfVarsKeysList.Count == 0)
                        {
                            var resultItem = new ResultOfQueryToRelation();
                            foreach (var varItem in leftVarsList)
                            {
                                resultItem.ResultOfVarOfQueryToRelationList.Add(varItem);
                            }

                            foreach (var varItem in rightVarsList)
                            {
                                resultItem.ResultOfVarOfQueryToRelationList.Add(varItem);
                            }

                            queryExecutingCard.ResultsOfQueryToRelationList.Add(resultItem);
                        }
                        else
                        {
                            var leftVarsDict = new Dictionary<StrongIdentifierValue, ResultOfVarOfQueryToRelation>();
                            var resultItem = new ResultOfQueryToRelation();
                            foreach (var varItem in leftVarsList)
                            {
                                var keyOfVars = varItem.NameOfVar;
                                if (intersectOfVarsKeysList.Contains(keyOfVars))
                                {
                                    leftVarsDict[keyOfVars] = varItem;
                                }
                                else
                                {
                                    resultItem.ResultOfVarOfQueryToRelationList.Add(varItem);
                                    continue;
                                }
                            }

                            foreach (var varItem in rightVarsList)
                            {
                                var keyOfVars = varItem.NameOfVar;
                                if (intersectOfVarsKeysList.Contains(keyOfVars))
                                {
                                    var leftVarItem = leftVarsDict[keyOfVars];
                                    var resultOfComparison = ExpressionNodeHelper.Compare(varItem.FoundExpression, leftVarItem.FoundExpression, null, null
#if DEBUG
                                        , options.Logger
#endif
                                        );

                                    if (resultOfComparison)
                                    {
                                        var originItemsDict = varItem.OriginDict;
                                        var leftVarOriginItemsDict = leftVarItem.OriginDict;

                                        foreach (var originItems in originItemsDict)
                                        {
                                            var tmpKeyOfOrigin = originItems.Key;

                                            if (!leftVarOriginItemsDict.ContainsKey(tmpKeyOfOrigin))
                                            {
                                                leftVarOriginItemsDict[tmpKeyOfOrigin] = originItems.Value;
                                            }
                                        }

                                        resultItem.ResultOfVarOfQueryToRelationList.Add(leftVarItem);
                                    }
                                    else
                                    {
                                        isFit = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    resultItem.ResultOfVarOfQueryToRelationList.Add(varItem);
                                    continue;
                                }
                            }

                            if (isFit)
                            {
                                resultsOfQueryToRelationList.Add(resultItem);
                            }
                        }
                    }
                }
            }
            else
            {
                var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
                rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
                rightQueryExecutingCard.SenderExpressionNode = processedExpr;
#endif
                rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

                queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess;

                queryExecutingCard.UsedKeysList.AddRange(rightQueryExecutingCard.UsedKeysList);

#if DEBUG
                //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
                //if (rightQueryExecutingCard.UsedKeysList.Any())
                //{
                //    throw new NotImplementedException();
                //}
#endif
            }
        }

        private void FillExecutingCardForCallingFromRelationForFact(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //foreach(var item in queryExecutingCard.KnownInfoList)
            //{
            //    options.Logger.Log($"options.EntityDictionary.GetName(item.Key) = {options.EntityDictionary.GetName(item.Key)}");
            //}
#endif

            var usedKeysList = queryExecutingCard.UsedKeysList;

            var useInheritance = options.UseInheritance;
            var inheritanceResolver = options.InheritanceResolver;

            var targetRelationsList = processedExpr.RelationsDict[queryExecutingCard.TargetRelation];

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
                //options.Logger.Log($"targetRelation.Key = {targetRelation.Key}");
                //options.Logger.Log($"options.EntityDictionary.GetName(targetRelation.Key) = {options.EntityDictionary.GetName(targetRelation.Key)}");
#endif

                usedKeysList.Add(targetRelation.Name);

                var paramsListOfTargetRelation = targetRelation.ParamsList;

                var isFit = true;

                foreach (var knownInfo in queryExecutingCard.KnownInfoList)
                {
#if DEBUG
                    //options.Logger.Log($"knownInfo = {knownInfo}");
#endif

                    List<StrongIdentifierValue> additionalKeys_1 = null;

                    if (useInheritance)
                    {
                        additionalKeys_1 = inheritanceResolver.GetSuperClassesKeysList(knownInfo.Name, options.LocalCodeExecutionContext);
                    }

#if DEBUG
                    //options.Logger.Log($"knownInfo.Key = {knownInfo.Key}");
                    //options.Logger.Log($"options.EntityDictionary.GetName(knownInfo.Key) = {options.EntityDictionary.GetName(knownInfo.Key)}");

                    //options.Logger.Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1, Formatting.Indented)}");

                    //var tmpNamesList_1 = additionalKeys_1.Select(p => options.EntityDictionary.GetName(p)).ToList();

                    //options.Logger.Log($"tmpNamesList_1 = {JsonConvert.SerializeObject(tmpNamesList_1, Formatting.Indented)}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //options.Logger.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
                        //options.Logger.Log($"paramOfTargetRelation.AsKeyRef.Key = {paramOfTargetRelation.AsKeyRef.Key}");
                        //options.Logger.Log($"options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key) = {options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key)}");
#endif

                        List<StrongIdentifierValue> additionalKeys_2 = null;

                        if (useInheritance && paramOfTargetRelation.IsKeyRef)
                        {
                            additionalKeys_2 = inheritanceResolver.GetSuperClassesKeysList(paramOfTargetRelation.AsKeyRef.Key, options.LocalCodeExecutionContext);
                        }

#if DEBUG
                        //options.Logger.Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2, Formatting.Indented)}");

                        //var tmpNamesList_2 = additionalKeys_2.Select(p => options.EntityDictionary.GetName(p)).ToList();

                        //options.Logger.Log($"tmpNamesList_2 = {JsonConvert.SerializeObject(tmpNamesList_2, Formatting.Indented)}");
#endif

                        var resultOfComparison = CompareKnownInfoAndExpressionNode(knownInfo, paramOfTargetRelation, additionalKeys_1, additionalKeys_2
#if DEBUG
                            , options.Logger
#endif
                            );

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

                    if (queryExecutingCard.VarsInfoList.Any())
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
                            //options.Logger.Log($"NEXT options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key) = {options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key)}");
#endif

                            if (paramOfTargetRelation.IsKeyRef)
                            {
                                usedKeysList.Add(paramOfTargetRelation.AsKeyRef.Key);
                            }
                            //else
                            //{
                            //    throw new NotImplementedException();
                            //}                            

                            var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                            resultOfVarOfQueryToRelation.NameOfVar = varItem.NameOfVar;
                            resultOfVarOfQueryToRelation.FoundExpression = paramOfTargetRelation;
                            resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                            var originInfo = new OriginOfVarOfQueryToRelation();
                            originInfo.RuleInstance = processedExpr.Parent;
                            originInfo.RulePart = processedExpr;

                            var keyOfRuleInstance = processedExpr.Parent.Name;

                            originInfo.NameOfRuleInstance = keyOfRuleInstance;

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

        private bool CompareKnownInfoAndExpressionNode(QueryExecutingCardAboutKnownInfo knownInfo, LogicalQueryNode expressionNode, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2
#if DEBUG
            , IEntityLogger logger
#endif
            )
        {
            var knownInfoExpression = knownInfo.Expression;

            return ExpressionNodeHelper.Compare(knownInfoExpression, expressionNode, additionalKeys_1, additionalKeys_2
#if DEBUG
                , logger
#endif
                );
        }

        private void FillExecutingCardForCallingFromRelationForProduction(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif

#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif
            var targetRelationsList = processedExpr.RelationsDict[queryExecutingCard.TargetRelation];

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
            if (targetKnownInfoList.Any())
            {
                throw new NotImplementedException();
            }
#endif

            var nextPartsList = processedExpr.GetNextPartsList();

#if DEBUG
            //options.Logger.Log($"nextPartsList.Count = {nextPartsList.Count}");
#endif

            foreach (var nextPart in nextPartsList)
            {
                var queryExecutingCardForNextPart = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForNextPart.VarsInfoList = targetRelation.VarsInfoList;
                queryExecutingCardForNextPart.KnownInfoList = targetKnownInfoList;
#if DEBUG
                queryExecutingCardForNextPart.SenderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
                queryExecutingCardForNextPart.SenderIndexedRulePart = processedExpr;
#endif
                FillExecutingCardForCallingFromOtherPart(nextPart, queryExecutingCardForNextPart, dataSource, options);

                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForNextPart.UsedKeysList);

#if DEBUG
                //options.Logger.Log($"queryExecutingCardForNextPart = {queryExecutingCardForNextPart}");

                //if (queryExecutingCardForNextPart.UsedKeysList.Any())
                //{
                //    throw new NotImplementedException();
                //}

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

                    var backKeysDict = new Dictionary<StrongIdentifierValue, StrongIdentifierValue>();

                    foreach (var varInfo in varsInfoList)
                    {
#if DEBUG
                        //options.Logger.Log($"varInfo = {varInfo}");
#endif

                        var targetInternalKeyOfVar = targetRelationVarsInfoDictByPosition[varInfo.Position];

#if DEBUG
                        //options.Logger.Log($"targetInternalKeyOfVar = {targetInternalKeyOfVar}");
#endif

                        backKeysDict[targetInternalKeyOfVar] = varInfo.NameOfVar;
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

                            var internalKeyOfVar = resultOfVarOfQueryToRelation.NameOfVar;

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

                                resultOfVarOfQueryToRelation.NameOfVar = externalKeyOfVar;

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

        private void FillExecutingCardForCallingFromOtherPart(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin ^&*^&*");
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif
            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
            queryExecutingCardForExpression.SenderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            queryExecutingCardForExpression.SenderIndexedRulePart = processedExpr;
#endif
            queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

#if DEBUG
            //options.Logger.Log($"%%%%%%%% queryExecutingCardForExpression = {queryExecutingCardForExpression}");
            //if (queryExecutingCardForExpression.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            queryExecutingCard.ResultsOfQueryToRelationList = queryExecutingCardForExpression.ResultsOfQueryToRelationList;
            queryExecutingCard.IsSuccess = queryExecutingCardForExpression.IsSuccess;

#if DEBUG
            //options.Logger.Log("End");
#endif
        }
    }
}
