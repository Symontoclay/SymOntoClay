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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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

            queryExpression.CheckDirty();

            queryExpression = queryExpression.Normalized;

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
            //options.Logger.Log($"Begin~~~~~~ GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
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
                    FillExecutingCardForRelationLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(processedExpr.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                            FillExecutingCardForAndOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.Or:
                            FillExecutingCardForOrOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(processedExpr.KindOfOperator), processedExpr.KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch(processedExpr.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            FillExecutingCardForNotOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(processedExpr.KindOfOperator), processedExpr.KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    FillExecutingCardForGroupLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void FillExecutingCardForRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"processedExpr.sIsQuestion = {processedExpr.IsQuestion}");
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

        private void NFillExecutingCard(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {            
#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif

            var indexedRulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(processedExpr.Name);

#if DEBUG
            //options.Logger.Log($"indexedRulePartsOfFactsList?.Count = {indexedRulePartsOfFactsList?.Count}");
#endif

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

            queryExecutingCard.UsedKeysList.Add(processedExpr.Name);

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
                    //options.Logger.Log($"processedExpr = {DebugHelperForRuleInstance.ToString(processedExpr)}");
                    //options.Logger.Log($"indexedRulePartsOfFacts = {DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePartsOfFacts)}");
#endif
                    var queryExecutingCardForTargetFact = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetFact.TargetRelation = processedExpr.Name;
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

            var indexedRulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(processedExpr.Name);

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
                    queryExecutingCardForTargetRule.TargetRelation = processedExpr.Name;

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
                }

                var n = 0;

                foreach (var param in processedExpr.ParamsList)
                {
#if DEBUG
                    //options.Logger.Log($"n = {n} param = {param}");
#endif

                    n++;

                    if (param.Kind != KindOfLogicalQueryNode.QuestionVar)
                    {
                        continue;
                    }

                    var foundExpression = targetRelation.ParamsList[n - 1];

                    var questionVarParam = param;

                    var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                    resultOfVarOfQueryToRelation.NameOfVar = questionVarParam.Name;
                    resultOfVarOfQueryToRelation.FoundExpression = foundExpression;
                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

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

        private void FillExecutingCardForAndOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("||||||||||||||||||||||||||||||||||||||||||||||||||||");

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

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if (!leftQueryExecutingCard.IsSuccess)
            {
                return;
            }

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
            rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
            rightQueryExecutingCard.SenderExpressionNode = processedExpr;
#endif
            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if(!rightQueryExecutingCard.IsSuccess)
            {
                return;
            }

            queryExecutingCard.IsSuccess = true;

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList.Concat(rightQueryExecutingCard.UsedKeysList));

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;
            var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

            if(!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                return;
            }

            var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

            if(leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

#if DEBUG
                //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                //throw new NotImplementedException();
#endif

                return;
            }

            if(!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

#if DEBUG
                //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                //throw new NotImplementedException();
#endif

                return;
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
#if DEBUG
                //options.Logger.Log($"leftResultOfQueryToRelation = {leftResultOfQueryToRelation}");
#endif

                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
#if DEBUG
                    //options.Logger.Log($"rightResultOfQueryToRelation = {rightResultOfQueryToRelation}");
#endif

                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var isFit = true;

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
#if DEBUG
                        //options.Logger.Log($"varName = {varName}");
#endif

                        var leftVarsDictContainsKey = leftVarsDict.ContainsKey(varName);
                        var rightVarsDictContainsKey = rightVarsDict.ContainsKey(varName);

                        if (leftVarsDictContainsKey && rightVarsDictContainsKey)
                        {
                            var leftVal = leftVarsDict[varName];
                            var rightVal = rightVarsDict[varName];

#if DEBUG
                            //options.Logger.Log($"leftVal = {leftVal}");
                            //options.Logger.Log($"rightVal = {rightVal}");
#endif

                            var resultOfComparison = ExpressionNodeHelper.Compare(leftVal, rightVal, null, null
#if DEBUG
                                        , options.Logger
#endif
                                        );

                            if (resultOfComparison)
                            {
                                varValuesList.Add((varName, leftVal));
                            }
                            else
                            {
                                isFit = false;
                            }
                        }
                        else
                        {
                            if (leftVarsDictContainsKey)
                            {
                                varValuesList.Add((varName, leftVarsDict[varName]));
                            }
                            else
                            {
                                varValuesList.Add((varName, rightVarsDict[varName]));
                            }
                        }
                    }

#if DEBUG
                    //options.Logger.Log($"isFit = {isFit}");
#endif

                    if(!isFit)
                    {
                        continue;
                    }

#if DEBUG
                    //options.Logger.Log($"varValuesList.Count = {varValuesList.Count}");
                    //foreach (var varValue in varValuesList)
                    //{
                    //    options.Logger.Log("------------");
                    //    options.Logger.Log($"varValue.Item1 = {varValue.Item1}");
                    //    options.Logger.Log($"varValue.Item2 = {varValue.Item2}");
                    //}
#endif

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
                    //options.Logger.Log($"varValuesDict.Count = {varValuesDict.Count}");
#endif

                    var varNamesListEnumerator = varNamesList.GetEnumerator();

                    if (varNamesListEnumerator.MoveNext())
                    {
                        var resultVarValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                        BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(varNamesListEnumerator, varValuesDict, resultVarValuesList, resultsOfQueryToRelationList, options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (!resultsOfQueryToRelationList.Any())
            {
                queryExecutingCard.IsSuccess = false;
            }

#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif
        }

        private void FillExecutingCardForOrOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
            rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
            rightQueryExecutingCard.SenderExpressionNode = processedExpr;
#endif
            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if(!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess)
            {
                return;
            }

            var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;
            var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

                queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess;

                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            if(!leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(rightQueryExecutingCard.UsedKeysList);

                queryExecutingCard.IsSuccess = rightQueryExecutingCard.IsSuccess;

                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            queryExecutingCard.IsSuccess = true;

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList.Concat(rightQueryExecutingCard.UsedKeysList));

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            if(leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            if(!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
#if DEBUG
                //options.Logger.Log($"leftResultOfQueryToRelation = {leftResultOfQueryToRelation}");
#endif

                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
#if DEBUG
                    //options.Logger.Log($"rightResultOfQueryToRelation = {rightResultOfQueryToRelation}");
#endif

                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
#if DEBUG
                        //options.Logger.Log($"varName = {varName}");
#endif

                        var resultOfVarOfQueryToRelationItem = new ResultOfVarOfQueryToRelation();
                        resultOfVarOfQueryToRelationItem.NameOfVar = varName;

                        var leftVarsDictContainsKey = leftVarsDict.ContainsKey(varName);
                        var rightVarsDictContainsKey = rightVarsDict.ContainsKey(varName);

                        if (leftVarsDictContainsKey && rightVarsDictContainsKey)
                        {
                            var leftVal = leftVarsDict[varName];
                            var rightVal = rightVarsDict[varName];

                            var resultOfComparison = ExpressionNodeHelper.Compare(leftVal, rightVal, null, null
#if DEBUG
                                        , options.Logger
#endif
                                        );

                            if(resultOfComparison)
                            {
                                varValuesList.Add((varName, leftVal));
                            }
                            else
                            {
                                varValuesList.Add((varName, leftVal));
                                varValuesList.Add((varName, rightVal));
                            }
                        }
                        else
                        {
                            if(leftVarsDictContainsKey)
                            {
                                varValuesList.Add((varName, leftVarsDict[varName]));
                            }
                            else
                            {
                                varValuesList.Add((varName, rightVarsDict[varName]));
                            }
                        }
                    }

#if DEBUG
                    //options.Logger.Log($"varValuesList.Count = {varValuesList.Count}");
                    //foreach(var varValue in varValuesList)
                    //{
                    //    options.Logger.Log("------------");
                    //    options.Logger.Log($"varValue.Item1 = {varValue.Item1}");
                    //    options.Logger.Log($"varValue.Item2 = {varValue.Item2}");
                    //}
#endif

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
                    //options.Logger.Log($"varValuesDict.Count = {varValuesDict.Count}");
#endif

                    var varNamesListEnumerator = varNamesList.GetEnumerator();

                    if(varNamesListEnumerator.MoveNext())
                    {
                        var resultVarValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                        BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(varNamesListEnumerator, varValuesDict, resultVarValuesList, resultsOfQueryToRelationList, options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif
        }

        private void BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(IEnumerator<StrongIdentifierValue> varNamesListEnumerator, Dictionary<StrongIdentifierValue, List<(StrongIdentifierValue, LogicalQueryNode)>> varValuesDict, List<(StrongIdentifierValue, LogicalQueryNode)> resultVarValuesList, IList<ResultOfQueryToRelation> resultsOfQueryToRelationList, OptionsOfFillExecutingCard options)
        {
            var varName = varNamesListEnumerator.Current;

#if DEBUG
            //options.Logger.Log($"varName = {varName}");
#endif

            var targetVarsValuesList = varValuesDict[varName];

#if DEBUG
            //options.Logger.Log($"targetVarsValuesList.Count = {targetVarsValuesList.Count}");
#endif

            foreach (var varValue in targetVarsValuesList)
            {
#if DEBUG
                //options.Logger.Log($"varValue = {varValue}");
#endif

                var newResultVarValuesList = resultVarValuesList.ToList();

                newResultVarValuesList.Add(varValue);

                if(varNamesListEnumerator.MoveNext())
                {
                    BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(varNamesListEnumerator, varValuesDict, newResultVarValuesList, resultsOfQueryToRelationList, options);
                }
                else
                {
#if DEBUG
                    //options.Logger.Log($"newResultVarValuesList.Count = {newResultVarValuesList.Count}");
#endif

                    var resultOfQueryToRelation = new ResultOfQueryToRelation();

                    foreach (var newResultVarValue in newResultVarValuesList)
                    {
#if DEBUG
                        //options.Logger.Log($"newResultVarValue = {newResultVarValue}");
#endif

                        var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                        resultOfVarOfQueryToRelation.NameOfVar = newResultVarValue.Item1;
                        resultOfVarOfQueryToRelation.FoundExpression = newResultVarValue.Item2;

                        resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);
                    }

                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }
            }
        }

        private void FillExecutingCardForNotOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            queryExecutingCard.IsSuccess = !leftQueryExecutingCard.IsSuccess;
            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif
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
                //options.Logger.Log($"targetRelation.Name = {targetRelation.Name}");
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
                            additionalKeys_2 = inheritanceResolver.GetSuperClassesKeysList(paramOfTargetRelation.Name, options.LocalCodeExecutionContext);
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
                                usedKeysList.Add(paramOfTargetRelation.Name);
                            }
                            //else
                            //{
                            //    throw new NotImplementedException();
                            //}                            

                            var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                            resultOfVarOfQueryToRelation.NameOfVar = varItem.NameOfVar;
                            resultOfVarOfQueryToRelation.FoundExpression = paramOfTargetRelation;
                            resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);
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

        private void FillExecutingCardForGroupLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess;

            foreach (var resultOfQueryToRelation in leftQueryExecutingCard.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }

#if DEBUG
            //if (leftQueryExecutingCard.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}

            options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            //throw new NotImplementedException();
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

            if (targetRelation.ParamsList.Count != queryExecutingCard.CountParams)
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

            var targetRelationVarsInfoDictByPosition = targetRelationVarsInfoList.ToDictionary(p => p.Position, p => p.NameOfVar);

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
