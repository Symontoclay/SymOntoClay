/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly VarsResolver _varsResolver;

        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        public bool IsTruth(LogicalSearchOptions options)
        {
            var result = Run(options);

#if DEBUG
            //Log($"result = {result}");
#endif

            return result.IsSuccess;
        }

        public LogicalSearchResult Run(LogicalSearchOptions options)
        {
#if DEBUG
            //Log($"options = {options}");
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

            ConsolidatedDataSource dataSource = null;

            if(options.TargetStorage == null)
            {
                dataSource = new ConsolidatedDataSource(GetStoragesList(options.LocalCodeExecutionContext.Storage));
            }
            else
            {
                var targetStorageList = GetStoragesList(options.TargetStorage);

#if DEBUG
                //Log($"targetStorageList = {targetStorageList.WriteListToString()}");
#endif

                var maxPriority = targetStorageList.Max(p => p.Priority);

#if DEBUG
                //Log($"maxPriority = {maxPriority}");
#endif

                var collectChainOfStoragesOptions = new CollectChainOfStoragesOptions();
                collectChainOfStoragesOptions.InitialPriority = maxPriority;
                collectChainOfStoragesOptions.UseFacts = false;


#if DEBUG
                //Log($"collectChainOfStoragesOptions = {collectChainOfStoragesOptions}");
#endif

                var additinalStoragesList = GetStoragesList(options.LocalCodeExecutionContext.Storage, collectChainOfStoragesOptions);

#if DEBUG
                //Log($"additinalStoragesList = {additinalStoragesList.WriteListToString()}");
#endif

                targetStorageList.AddRange(additinalStoragesList);

                dataSource = new ConsolidatedDataSource(targetStorageList);
            }

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            queryExpression.CheckDirty();

#if DEBUG
            //Log($"queryExpression = {queryExpression}");
            //Log($"DebugHelperForRuleInstance.ToString(queryExpression) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

            if(queryExpression.IsParameterized)
            {
                queryExpression = queryExpression.Clone();

#if DEBUG
                //Log($"queryExpression (1) = {queryExpression}");
                //Log($"DebugHelperForRuleInstance.ToString(queryExpression) (1) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

                var packedVarsResolver = new PackedVarsResolver(_varsResolver, options.LocalCodeExecutionContext);

                queryExpression.ResolveVariables(packedVarsResolver);

#if DEBUG
                //Log($"queryExpression (2) = {queryExpression}");
                //Log($"DebugHelperForRuleInstance.ToString(queryExpression) (2) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

                queryExpression.CheckDirty();
            }

            queryExpression = queryExpression.Normalized;

#if DEBUG
            //Log($"queryExpression (after) = {queryExpression}");
            //Log($"DebugHelperForRuleInstance.ToString(queryExpression) (after) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

#if DEBUG
            //throw new NotImplementedException();
#endif

            var logicalSearchStorageContext = new LogicalSearchStorageContext(_context, options.LocalCodeExecutionContext, queryExpression);

            var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
            optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
            optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;
            optionsOfFillExecutingCard.UseInheritance = options.UseInheritance;
            optionsOfFillExecutingCard.LocalCodeExecutionContext = options.LocalCodeExecutionContext;
            optionsOfFillExecutingCard.MainStorageContext = _context;
            optionsOfFillExecutingCard.LogicalSearchStorageContext = logicalSearchStorageContext;

#if DEBUG
            optionsOfFillExecutingCard.Logger = Logger;
#endif

#if DEBUG
            //Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

#if DEBUG
            var tmpExplainNode = new LogicalSearchExplainNode()
            {
                Kind = KindOfLogicalSearchExplainNode.Root
            };

            tmpExplainNode.ProcessedRuleInstance = options.QueryExpression;
#endif

            var resultExplainNode = new LogicalSearchExplainNode()
            {
                Kind = KindOfLogicalSearchExplainNode.Result
            };

#if DEBUG
            LogicalSearchExplainNode.LinkNodes(tmpExplainNode, resultExplainNode);
#endif

            queryExecutingCard.ParentExplainNode = resultExplainNode;

            FillExecutingCard(queryExpression, queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
            //Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif

            if(queryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if(queryExecutingCard.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            var usedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

#if DEBUG
            //Log($"usedKeysList.Count = {usedKeysList.Count}");
            //foreach(var usedKey in usedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
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

            if(resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCard, resultExplainNode);
            }

#if DEBUG
            var dotStr = DebugHelperForLogicalSearchExplainNode.ToDot(tmpExplainNode);

            //Log($"dotStr = '{dotStr}'");

            File.WriteAllText("logicalSearch.dot", dotStr);

            //Log("End");
#endif

            return result;
        }

        private void AppendResults(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard)
        {
            destQueryExecutingCard.IsSuccess = sourceQueryExecutingCard.IsSuccess;

            CopyResultsOfQueryToRelationList(sourceQueryExecutingCard, destQueryExecutingCard);

            destQueryExecutingCard.UsedKeysList.AddRange(sourceQueryExecutingCard.UsedKeysList);
        }

        private void CopyResultsOfQueryToRelationList(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard)
        {
            var destList = destQueryExecutingCard.ResultsOfQueryToRelationList;

            foreach (var resultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
            {
                destList.Add(resultOfQueryToRelation);
            }
        }

        private void FillUpResultToExplainNode(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, LogicalSearchExplainNode resultExplainNode)
        {
            resultExplainNode.IsSuccess = queryExecutingCard.IsSuccess;
            resultExplainNode.ResultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;
        }

        private void FillExecutingCard(RuleInstance processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RuleInstanceQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedRuleInstance = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForPart_1.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.PrimaryPart, queryExecutingCardForPart_1, dataSource, options);

            if(queryExecutingCardForPart_1.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (queryExecutingCardForPart_1.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            AppendResults(queryExecutingCardForPart_1, queryExecutingCard);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCard, resultExplainNode);
            }

#if DEBUG
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCard(PrimaryRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"Begin ~~~~~~ processedExpr = {processedExpr}");
            //options.Logger.Log($"Begin ~~~~~~ processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.PrimaryRulePartQuery
                };

                currentExplainNode.ProcessedPrimaryRulePart = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForExpression.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            //options.Logger.Log($"~~~~~~ (after) processedExpr = {processedExpr}");
            //options.Logger.Log($"~~~~~~ (after) processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
            //options.Logger.Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCardForExpression, resultExplainNode);
            }

            if (!queryExecutingCardForExpression.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

                return;
            }

            if (queryExecutingCardForExpression.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.ParentExplainNode = parentExplainNode;

                var postFilterNode = FillExecutingCardUsingPostFiltersList(queryExecutingCardForExpression, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

#if DLSR
                throw new NotImplementedException();
#endif

                AppendResults(queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);

                if (postFilterNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(postFilterNode, currentExplainNode);
                }
            }
            else
            {
                if(parentExplainNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }                

                AppendResults(queryExecutingCardForExpression, queryExecutingCard);
            }

#if DEBUG
            //options.Logger.Log($"*************()()()()()()======== queryExecutingCard = {queryExecutingCard}");
            //if (queryExecutingCardForExpression.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}
            //options.Logger.Log("End");
#endif
        }

        private LogicalSearchExplainNode FillExecutingCardUsingPostFiltersList(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, OptionsOfFillExecutingCard options, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //options.Logger.Log($"sourceQueryExecutingCard = {sourceQueryExecutingCard}");
            //options.Logger.Log($"destQueryExecutingCard = {destQueryExecutingCard}");
#endif

            var postFiltersList = sourceQueryExecutingCard.PostFiltersList;

            if(sourceQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            var targetSourceQueryExecutingCard = sourceQueryExecutingCard;

            var parentExplainNode = destQueryExecutingCard.ParentExplainNode;
            List<(LogicalSearchExplainNode, LogicalSearchExplainNode)> explainNodesList = null;

            if(parentExplainNode != null)
            {
                explainNodesList = new List<(LogicalSearchExplainNode, LogicalSearchExplainNode)>();
            }

            foreach (var postFilter in postFiltersList)
            {
                var kindOfBinaryOperator = postFilter.KindOfBinaryOperator;

#if DEBUG
                //options.Logger.Log($"kindOfBinaryOperator = {kindOfBinaryOperator}");
#endif

                var oldTargetSourceQueryExecutingCard = targetSourceQueryExecutingCard;

                targetSourceQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                switch (kindOfBinaryOperator)
                {
                    case KindOfOperatorOfLogicalQueryNode.And:
                        {
                            var explainNode = FillExecutingCardUsingPostFilterListWithAndStrategy(oldTargetSourceQueryExecutingCard, targetSourceQueryExecutingCard, postFilter, options, dataSource);
#if DEBUG
                            //options.Logger.Log($"targetSourceQueryExecutingCard = {targetSourceQueryExecutingCard}");
                            //options.Logger.Log($"explainNode = {explainNode}");
#endif

                            if(parentExplainNode != null)
                            {
                                var resultExplainNode = new LogicalSearchExplainNode()
                                {
                                    Kind = KindOfLogicalSearchExplainNode.Result
                                };

                                FillUpResultToExplainNode(targetSourceQueryExecutingCard, resultExplainNode);

                                explainNodesList.Add((resultExplainNode, explainNode));
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfBinaryOperator), kindOfBinaryOperator, null);
                }
            }

#if DEBUG
            //options.Logger.Log($"targetSourceQueryExecutingCard (after) = {targetSourceQueryExecutingCard}");
            //options.Logger.Log($"explainNodesList?.Count = {explainNodesList?.Count}");
#endif

            if (targetSourceQueryExecutingCard.IsSuccess)
            {
                AppendResults(targetSourceQueryExecutingCard, destQueryExecutingCard);
            }

#if DEBUG
            //options.Logger.Log($"///////////////// destQueryExecutingCard = {destQueryExecutingCard}");
#endif

            if (explainNodesList == null)
            {
                return null;
            }
            else 
            { 
                if(explainNodesList.Count == 1)
                {
                    var explainNodesListItem = explainNodesList.Single();

                    var targetResultItem = explainNodesListItem.Item1;
                    var targetItem = explainNodesListItem.Item2;

#if DEBUG
                    //options.Logger.Log($"targetResultItem = {targetResultItem}");
                    //options.Logger.Log($"targetItem = {targetItem}");
#endif

                    var realParentExplainNode = parentExplainNode.Parent;

                    LogicalSearchExplainNode.ResetParent(parentExplainNode);

                    LogicalSearchExplainNode.LinkNodes(realParentExplainNode, targetResultItem);

                    LogicalSearchExplainNode.LinkNodes(targetResultItem, targetItem);

                    LogicalSearchExplainNode.LinkNodes(targetItem, parentExplainNode);

                    return parentExplainNode;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private LogicalSearchExplainNode FillExecutingCardUsingPostFilterListWithAndStrategy(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, PostFilterOfQueryExecutingCardForPersistLogicalData postFilter, OptionsOfFillExecutingCard options, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //options.Logger.Log($"sourceQueryExecutingCard = {sourceQueryExecutingCard}");
            //options.Logger.Log($"destQueryExecutingCard = {destQueryExecutingCard}");
            options.Logger.Log($"postFilter = {postFilter}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            if(sourceQueryExecutingCard.ParentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.PostFilterWithAndStrategy
                };
            }

#if DLSR
            throw new NotImplementedException();
#endif

            var processedExpr = postFilter.ProcessedExpr;

#if DEBUG
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;
            var kindOfOperator = processedExpr.KindOfOperator;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
            options.Logger.Log($"kindOfOperator = {kindOfOperator}");
#endif

            var resultsOfQueryToRelationList = new List<ResultOfQueryToRelation>();
            var usedKeysList = new List<StrongIdentifierValue>();

            if(leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar && rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var leftVariableName = leftExpr.Name;
                var rightVariableName = rightExpr.Name;

#if DEBUG
                options.Logger.Log($"leftVariableName = {leftVariableName}");
                options.Logger.Log($"rightVariableName = {rightVariableName}");
#endif

#if DLSR
                throw new NotImplementedException();
#endif

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
#if DEBUG
                    //options.Logger.Log($"sourceResultOfQueryToRelation = {sourceResultOfQueryToRelation}");
#endif

                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

#if DEBUG
                    //options.Logger.Log($"sourceVarNamesList = {JsonConvert.SerializeObject(sourceVarNamesList.Select(p => p.NameValue))}");
#endif

                    if (sourceVarsDict.ContainsKey(leftVariableName) && sourceVarsDict.ContainsKey(rightVariableName))
                    {
                        var sourceLeftNode = sourceVarsDict[leftVariableName];
                        var sourceRightNode = sourceVarsDict[rightVariableName];

#if DEBUG
                        //options.Logger.Log($"sourceLeftNode = {sourceLeftNode}");
                        //options.Logger.Log($"sourceRightNode = {sourceRightNode}");
#endif

                        var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                        var resultOfComparison = CompareForPostFilter(kindOfOperator, sourceLeftNode, sourceRightNode, options, comparisonQueryExecutingCard, dataSource);

#if DEBUG
                        //options.Logger.Log($"resultOfComparison = {resultOfComparison}");
#endif

                        if (resultOfComparison)
                        {
#if DEBUG
                            //options.Logger.Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
#endif

                            resultsOfQueryToRelationList.Add(sourceResultOfQueryToRelation);

                            usedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                        }
                    }                    
                }
            }
            else
            {
                StrongIdentifierValue variableOfFilter = null;
                LogicalQueryNode nodeOfFilter = null;

                var isLeftRight = true;

                if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
                {
                    variableOfFilter = leftExpr.Name;
                    nodeOfFilter = rightExpr;
                }
                else
                {
                    variableOfFilter = rightExpr.Name;
                    nodeOfFilter = leftExpr;
                    isLeftRight = false;
                }

#if DEBUG
                options.Logger.Log($"variableOfFilter = {variableOfFilter}");
                options.Logger.Log($"valueOfFilter = {nodeOfFilter}");
#endif

#if DLSR
                throw new NotImplementedException();
#endif

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
#if DEBUG
                    //options.Logger.Log($"sourceResultOfQueryToRelation = {sourceResultOfQueryToRelation}");
#endif

                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

#if DEBUG
                    //options.Logger.Log($"sourceVarNamesList = {JsonConvert.SerializeObject(sourceVarNamesList.Select(p => p.NameValue))}");
#endif

                    if (sourceVarsDict.ContainsKey(variableOfFilter))
                    {
                        var sourceNode = sourceVarsDict[variableOfFilter];

#if DEBUG
                        //options.Logger.Log($"sourceNode = {sourceNode}");
#endif

                        var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                        bool resultOfComparison;

                        if (isLeftRight)
                        {
                            resultOfComparison = CompareForPostFilter(kindOfOperator, sourceNode, nodeOfFilter, options, comparisonQueryExecutingCard, dataSource);
                        }
                        else
                        {
                            resultOfComparison = CompareForPostFilter(kindOfOperator, nodeOfFilter, sourceNode, options, comparisonQueryExecutingCard, dataSource);
                        }

#if DEBUG
                        options.Logger.Log($"resultOfComparison = {resultOfComparison}");
#endif

                        if (resultOfComparison)
                        {
#if DEBUG
                            //options.Logger.Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
#endif

                            resultsOfQueryToRelationList.Add(sourceResultOfQueryToRelation);

                            usedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                        }
                    }
                }
            }

#if DEBUG
            //options.Logger.Log($"resultsOfQueryToRelationList.Count = {resultsOfQueryToRelationList.Count}");
#endif

            if (resultsOfQueryToRelationList.Any())
            {
                destQueryExecutingCard.IsSuccess = true;

                destQueryExecutingCard.ResultsOfQueryToRelationList = resultsOfQueryToRelationList;
                destQueryExecutingCard.UsedKeysList = sourceQueryExecutingCard.UsedKeysList.ToList();
                destQueryExecutingCard.UsedKeysList.AddRange(usedKeysList);

                destQueryExecutingCard.UsedKeysList = destQueryExecutingCard.UsedKeysList.Distinct().ToList();
            }
            else
            {
                destQueryExecutingCard.IsSuccess = false;
            }

#if DEBUG
            //options.Logger.Log($"destQueryExecutingCard (after) = {destQueryExecutingCard}");
#endif

            return currentExplainNode;
        }

        private bool CompareForPostFilter(KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            options.Logger.Log($"kindOfOperator = {kindOfOperator}");
#endif

            switch (kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Is:
                    return CompareForPostFilterByOperatorIs(leftNode, rightNode, options, queryExecutingCard, dataSource);

                case KindOfOperatorOfLogicalQueryNode.IsNot:
                    return !CompareForPostFilterByOperatorIs(leftNode, rightNode, options, queryExecutingCard, dataSource);

                case KindOfOperatorOfLogicalQueryNode.More:
                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                case KindOfOperatorOfLogicalQueryNode.Less:
                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    return CompareForPostFilterByOperatorsMoreOrLess(kindOfOperator, leftNode, rightNode, options, queryExecutingCard);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }
        }

        private bool CompareForPostFilterByOperatorIs(LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //options.Logger.Log($"leftNode = {leftNode}");
            //options.Logger.Log($"rightNode = {rightNode}");
#endif

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            List<StrongIdentifierValue> additionalKeys_1 = null;
            List<StrongIdentifierValue> additionalKeys_2 = null;

            var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
            reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

            if (options.UseInheritance)
            {
                var inheritanceResolver = _inheritanceResolver;

                var kindOfSourceNode = leftNode.Kind;

                switch(kindOfSourceNode)
                {
                    case KindOfLogicalQueryNode.Value:
                        break;

                    case KindOfLogicalQueryNode.Entity:
                    case KindOfLogicalQueryNode.Concept:
                        additionalKeys_1 = inheritanceResolver.GetSuperClassesKeysList(leftNode.Name, options.LocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSourceNode), kindOfSourceNode, null);
                }

                var kindOfNodeOfFilter = rightNode.Kind;

                switch (kindOfNodeOfFilter)
                {
                    case KindOfLogicalQueryNode.Value:
                        break;

                    case KindOfLogicalQueryNode.Entity:
                    case KindOfLogicalQueryNode.Concept:
                        additionalKeys_2 = inheritanceResolver.GetSuperClassesKeysList(rightNode.Name, options.LocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfNodeOfFilter), kindOfNodeOfFilter, null);
                }
            }

            return EqualityCompare(leftNode, rightNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource);
        }

        private bool CompareForPostFilterByOperatorsMoreOrLess(KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
#if DEBUG
            //options.Logger.Log($"kindOfOperator = {kindOfOperator}");
            //options.Logger.Log($"leftNode = {leftNode}");
            //options.Logger.Log($"rightNode = {rightNode}");
#endif

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var localCodeExecutionContext = options.LocalCodeExecutionContext;
            var numberValueLinearResolver = _numberValueLinearResolver;

            if (leftNode.Kind == KindOfLogicalQueryNode.Value && rightNode.Kind == KindOfLogicalQueryNode.Value)
            {
                var leftValue = leftNode.Value;
                var rightValue = rightNode.Value;

#if DEBUG
                //options.Logger.Log($"leftValue = {leftValue}");
                //options.Logger.Log($"rightValue = {rightValue}");
#endif                

                if(numberValueLinearResolver.CanBeResolved(leftValue) && numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //options.Logger.Log($"leftNumberValue = {leftNumberValue}");
                    //options.Logger.Log($"rightNumberValue = {rightNumberValue}");
#endif

                    var leftSystemNullaleValue = leftNumberValue.SystemValue;
                    var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                    //options.Logger.Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                    //options.Logger.Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
#endif

                    if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                    {
                        return CompareSystemValues(kindOfOperator, leftSystemNullaleValue.Value, rightSystemNullaleValue.Value, options);
                    }
                    else
                    {
                        if(!leftSystemNullaleValue.HasValue && !rightSystemNullaleValue.HasValue)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return false;
            }

            if ((leftNode.Kind == KindOfLogicalQueryNode.Concept || leftNode.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence) && (rightNode.Kind == KindOfLogicalQueryNode.Concept || rightNode.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence))
            {
                throw new NotImplementedException();
            }

            if(leftNode.Kind == KindOfLogicalQueryNode.Concept && rightNode.Kind == KindOfLogicalQueryNode.Value)
            {
                var leftName = leftNode.Name;
                var rightValue = rightNode.Value;

#if DEBUG
                //options.Logger.Log($"leftName = {leftName}");
                //options.Logger.Log($"rightValue = {rightValue}");
#endif

                if(numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //options.Logger.Log($"rightNumberValue = {rightNumberValue}");
#endif

                    if (!rightNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value <= rightNumberValue.SystemValue.Value;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                }

                throw new NotImplementedException();                           
            }

            if (leftNode.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence && rightNode.Kind == KindOfLogicalQueryNode.Value)
            {
                var leftSequence = leftNode.FuzzyLogicNonNumericSequenceValue;
                var rightValue = rightNode.Value;
#if DEBUG
                //options.Logger.Log($"leftSequence = {leftSequence}");
                //options.Logger.Log($"rightValue = {rightValue}");                
#endif

                if (numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //options.Logger.Log($"rightNumberValue = {rightNumberValue}");
#endif

                    if (!rightNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value <= rightNumberValue.SystemValue.Value;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                }

                throw new NotImplementedException();
            }

            if (leftNode.Kind == KindOfLogicalQueryNode.Value && rightNode.Kind == KindOfLogicalQueryNode.Concept)
            {
                var leftValue = leftNode.Value;
                var rightName = rightNode.Name;

#if DEBUG
                //options.Logger.Log($"leftValue = {leftValue}");
                //options.Logger.Log($"rightName = {rightName}");
#endif

                if (numberValueLinearResolver.CanBeResolved(leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);

#if DEBUG
                    //options.Logger.Log($"leftNumberValue = {leftNumberValue}");
#endif

                    if (!leftNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if(!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if(eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif
                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value <= systemDeffuzzificatedValue.Value;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                }

                throw new NotImplementedException();
            }

            if (leftNode.Kind == KindOfLogicalQueryNode.Value && rightNode.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence)
            {
                var leftValue = leftNode.Value;
                var rightSequence = rightNode.FuzzyLogicNonNumericSequenceValue;

#if DEBUG
                //options.Logger.Log($"leftValue = {leftValue}");
                //options.Logger.Log($"rightSequence = {rightSequence}");
#endif
                if (numberValueLinearResolver.CanBeResolved(leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);

#if DEBUG
                    //options.Logger.Log($"leftNumberValue = {leftNumberValue}");
#endif

                    if (!leftNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif
                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"eqResult = {eqResult}");
#endif

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //options.Logger.Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //options.Logger.Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value <= systemDeffuzzificatedValue.Value;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private bool CompareSystemValues(KindOfOperatorOfLogicalQueryNode kindOfOperator, double left, double right, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"kindOfOperator = {kindOfOperator}");
            //options.Logger.Log($"left = {left}");
            //options.Logger.Log($"right = {right}");
#endif

            switch (kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Is:
                    return left == right;

                case KindOfOperatorOfLogicalQueryNode.More:
                    return left > right;

                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                    return left >= right;

                case KindOfOperatorOfLogicalQueryNode.Less:
                    return left < right;

                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    return left <= right;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }
        }

        private void FillExecutingCard(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
#endif

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

                        case KindOfOperatorOfLogicalQueryNode.Is:
                            FillExecutingCardForIsOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                            FillExecutingCardForIsNotOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.More:
                            FillExecutingCardForMoreOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            FillExecutingCardForLessOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
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

#if DEBUG
            //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif
        }

        private void FillExecutingCardForRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"processedExpr.Name = {processedExpr.Name}");
            //options.Logger.Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
#endif

            if (processedExpr.IsQuestion)
            {
                FillExecutingCardForQuestion(processedExpr, queryExecutingCard, dataSource, options);
                return;
            }

#if DEBUG
            //options.Logger.Log($"processedExpr.Name = {processedExpr.Name}");
            //options.Logger.Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //options.Logger.Log($"processedExpr.Params.Count = {processedExpr.ParamsList.Count}");
            //foreach (var param in processedExpr.ParamsList)
            //{
            //    options.Logger.Log($"param = {param}");
            //}
            //options.Logger.Log($"processedExpr.KnownInfoList.Count = {processedExpr.KnownInfoList.Count}");
            //foreach (var knownInfo in processedExpr.KnownInfoList)
            //{
            //    options.Logger.Log($"knownInfo = {knownInfo}");
            //}
            //options.Logger.Log($"VarsInfoList.Count = {processedExpr.VarsInfoList.Count}");
            //foreach (var varInfo in processedExpr.VarsInfoList)
            //{
            //    options.Logger.Log($"varInfo = {varInfo}");
            //}
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            NFillExecutingCardForRelationLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"^^^^^^queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() (after) = {processedExpr.GetHumanizeDbgString()}");

            //options.Logger.Log("End");
#endif
        }

        private void NFillExecutingCardForRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"processedExpr.Name = {processedExpr.Name}");
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");

            //if(processedExpr.Name.NameValue == "is")
            //{
            //    options.Logger.Log($"processedExpr = {processedExpr}");
            //    options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //}
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            LogicalSearchExplainNode directFactsCollectorExplainNode = null;
            LogicalSearchExplainNode directFactsResultsCollectorExplainNode = null;
            LogicalSearchExplainNode directFactsDataSourceCollectorExplainNode = null;
            LogicalSearchExplainNode directFactsDataSourceResultExplainNode = null;

            LogicalSearchExplainNode productionCollectorExplainNode = null;
            LogicalSearchExplainNode productionResultsCollectorExplainNode = null;
            LogicalSearchExplainNode productionDataSourceCollectorExplainNode = null;
            LogicalSearchExplainNode productionDataSourceResultExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedLogicalQueryNode = processedExpr;

                directFactsCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.ProcessRelationWithDirectFactsCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, directFactsCollectorExplainNode);

                directFactsResultsCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.ResultCollector
                };

                LogicalSearchExplainNode.LinkNodes(directFactsCollectorExplainNode, directFactsResultsCollectorExplainNode);

                directFactsDataSourceCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceCollector
                };

                LogicalSearchExplainNode.LinkNodes(directFactsCollectorExplainNode, directFactsDataSourceCollectorExplainNode);

                directFactsDataSourceResultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                };

                LogicalSearchExplainNode.LinkNodes(directFactsDataSourceCollectorExplainNode, directFactsDataSourceResultExplainNode);

                productionCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.ProcessRelationWithProductionCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, productionCollectorExplainNode);

                productionResultsCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.ResultCollector
                };

                LogicalSearchExplainNode.LinkNodes(productionCollectorExplainNode, productionResultsCollectorExplainNode);

                productionDataSourceCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceCollector
                };

                LogicalSearchExplainNode.LinkNodes(productionCollectorExplainNode, productionDataSourceCollectorExplainNode);

                productionDataSourceResultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                };

                LogicalSearchExplainNode.LinkNodes(productionDataSourceCollectorExplainNode, productionDataSourceResultExplainNode);
            }

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

#if DEBUG
            //options.Logger.Log($"mergingResult.IsSuccess = {mergingResult.IsSuccess}");
#endif

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

#if DEBUG
            //options.Logger.Log($"targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    options.Logger.Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
            //if(targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var rulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(processedExpr.Name, options.LogicalSearchStorageContext, directFactsDataSourceResultExplainNode);

            if(directFactsDataSourceResultExplainNode != null)
            {
                directFactsDataSourceResultExplainNode.BaseRulePartList = rulePartsOfFactsList;
            }

#if DEBUG
            //options.Logger.Log($"rulePartsOfFactsList?.Count = {rulePartsOfFactsList?.Count}");
#endif
            queryExecutingCard.UsedKeysList.Add(processedExpr.Name);

            if (!rulePartsOfFactsList.IsNullOrEmpty())
            {
                foreach (var rulePartsOfFacts in rulePartsOfFactsList)
                {
#if DEBUG
                    //options.Logger.Log($"processedExpr = {DebugHelperForRuleInstance.ToString(processedExpr)}");
                    //options.Logger.Log($"rulePartsOfFacts = {DebugHelperForRuleInstance.BaseRulePartToString(rulePartsOfFacts)}");
#endif

                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (directFactsResultsCollectorExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode()
                        {
                            Kind = KindOfLogicalSearchExplainNode.Result
                        };

                        LogicalSearchExplainNode.LinkNodes(directFactsResultsCollectorExplainNode, localResultExplainNode);
                    }

                    var queryExecutingCardForTargetFact = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetFact.TargetRelation = processedExpr.Name;
                    queryExecutingCardForTargetFact.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetFact.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetFact.KnownInfoList = targetKnownInfoList;
                    queryExecutingCardForTargetFact.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                    queryExecutingCardForTargetFact.ParentExplainNode = localResultExplainNode;


                    FillExecutingCardForCallingFromRelationForFact(rulePartsOfFacts, queryExecutingCardForTargetFact, dataSource, options);

#if DEBUG
                    //options.Logger.Log($"++++++queryExecutingCardForTargetFact.IsSuccess = {queryExecutingCardForTargetFact.IsSuccess}");
                    //options.Logger.Log($"++++++queryExecutingCardForTargetFact = {queryExecutingCardForTargetFact}");
                    //if (queryExecutingCardForTargetFact.UsedKeysList.Any())
                    //{
                    //    throw new NotImplementedException();
                    //}                    
#endif

                    if (queryExecutingCardForTargetFact.IsSuccess)
                    {
                        if (queryExecutingCardForTargetFact.IsPostFiltersListOnly)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetFact.PostFiltersList.Any())
                        {
                            throw new NotImplementedException();
                        }

                        AppendResults(queryExecutingCardForTargetFact, queryExecutingCard);
                    }

                    if (localResultExplainNode != null)
                    {
                        FillUpResultToExplainNode(queryExecutingCardForTargetFact, localResultExplainNode);
                    }
                }
            }

#if DEBUG
            //options.Logger.Log($"~~~~~~~~~~~~~~~~~queryExecutingCard = {queryExecutingCard}");
#endif

            var rulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(processedExpr.Name, options.LogicalSearchStorageContext, productionDataSourceResultExplainNode);

#if DEBUG
            //options.Logger.Log($"rulePartWithOneRelationsList?.Count = {rulePartWithOneRelationsList?.Count}");
#endif

            if(productionDataSourceResultExplainNode != null)
            {
                productionDataSourceResultExplainNode.BaseRulePartList = rulePartWithOneRelationsList;
            }

            if (!rulePartWithOneRelationsList.IsNullOrEmpty())
            {
                foreach (var indexedRulePartsOfRule in rulePartWithOneRelationsList)
                {
#if DEBUG
                    //options.Logger.Log($"processedExpr = {processedExpr}");
                    //options.Logger.Log($"indexedRulePartsOfRule = {indexedRulePartsOfRule}");
                    //options.Logger.Log($"indexedRulePartsOfRule = {DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePartsOfRule)}");
#endif

                    LogicalSearchExplainNode localResultExplainNode = null;

                    if(productionResultsCollectorExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode()
                        {
                            Kind = KindOfLogicalSearchExplainNode.Result
                        };

                        LogicalSearchExplainNode.LinkNodes(productionResultsCollectorExplainNode, localResultExplainNode);
                    }

                    var queryExecutingCardForTargetRule = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetRule.TargetRelation = processedExpr.Name;

#if DEBUG
                    //options.Logger.Log($"Key = {Key}");
                    //options.Logger.Log($"options.EntityDictionary.GetName(Key) = {options.EntityDictionary.GetName(Key)}");
#endif

                    queryExecutingCardForTargetRule.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetRule.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetRule.KnownInfoList = targetKnownInfoList;
                    queryExecutingCardForTargetRule.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                    queryExecutingCardForTargetRule.ParentExplainNode = localResultExplainNode;

                    FillExecutingCardForCallingFromRelationForProduction(indexedRulePartsOfRule, queryExecutingCardForTargetRule, dataSource, options);

#if DEBUG
                    //options.Logger.Log($"&&&&&&&&&&&&&&&&&queryExecutingCardForTargetRule.IsSuccess = {queryExecutingCardForTargetRule.IsSuccess}");
                    //options.Logger.Log($"&&&&&&&&&&&&&&&&&queryExecutingCardForTargetRule = {queryExecutingCardForTargetRule}");
#endif

                    if (queryExecutingCardForTargetRule.IsSuccess)
                    {
                        if (queryExecutingCardForTargetRule.IsPostFiltersListOnly)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetRule.PostFiltersList.Any())
                        {
                            throw new NotImplementedException();
                        }

                        AppendResults(queryExecutingCardForTargetRule, queryExecutingCard);
                    }

                    if (localResultExplainNode != null)
                    {
                        FillUpResultToExplainNode(queryExecutingCardForTargetRule, localResultExplainNode);
                    }

#if DEBUG
                    //options.Logger.Log($"!!!!!!!!!!!!!!!!!!queryExecutingCard = {queryExecutingCard}");
#endif
                }
            }

#if DEBUG
            //options.Logger.Log($"###~~~~~!!!!!!!!!!!!!!!!!!queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCardForQuestion(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"processedExpr.Name = {processedExpr.Name}");
            //options.Logger.Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //options.Logger.Log($"processedExpr.Params.Count = {processedExpr.ParamsList.Count}");
            //foreach (var param in processedExpr.ParamsList)
            //{
            //    options.Logger.Log($"param = {param}");
            //}
            //options.Logger.Log($"processedExpr.VarsInfoList.Count = {processedExpr.VarsInfoList.Count}");
            //foreach (var varInfo in processedExpr.VarsInfoList)
            //{
            //    options.Logger.Log($"varInfo = {varInfo}");
            //}
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var hasAnnotations = !processedExpr.Annotations.IsNullOrEmpty();

#if DEBUG
            //options.Logger.Log($"hasAnnotations = {hasAnnotations}");
#endif

            var targetRelationsList = dataSource.AllRelationsForProductions(options.LogicalSearchStorageContext);

#if DEBUG
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
            //foreach (var targetRelation in targetRelationsList)
            //{
            //    options.Logger.Log($"targetRelation.ToHumanizedString() = {targetRelation.ToHumanizedString()}");
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
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() (after)= {processedExpr.GetHumanizeDbgString()}");
            //LogInstance.Log($"this = {this}");
#endif

#if DEBUG
            //throw new NotImplementedException();
            //options.Logger.Log("End");
#endif
        }

        private void CopyPostFilters(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, KindOfOperatorOfLogicalQueryNode kindOfBinaryOperator)
        {
            var destList = destQueryExecutingCard.PostFiltersList;

            if (sourceQueryExecutingCard.PostFiltersList.Any())
            {
                foreach (var postFilter in sourceQueryExecutingCard.PostFiltersList)
                {
                    if (postFilter.KindOfBinaryOperator == KindOfOperatorOfLogicalQueryNode.Unknown)
                    {
                        postFilter.KindOfBinaryOperator = kindOfBinaryOperator;
                    }

                    destList.Add(postFilter);
                }
            }
        }

        private void FillExecutingCardForAndOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("||||||||||||||||||||||||||||||||||||||||||||||||||||");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode leftResultExplainNode = null;
            LogicalSearchExplainNode rightResultExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.And,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                leftResultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, leftResultExplainNode);

                rightResultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, rightResultExplainNode);
            }

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            leftQueryExecutingCard.ParentExplainNode = leftResultExplainNode;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if (leftResultExplainNode != null)
            {
                FillUpResultToExplainNode(leftQueryExecutingCard, leftResultExplainNode);
            }

            if (!leftQueryExecutingCard.IsSuccess)
            {
                if (leftResultExplainNode != null)
                {
                    leftResultExplainNode.AdditionalInformation.Add("!leftQueryExecutingCard.IsSuccess");
                }

                return;
            }

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            rightQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            rightQueryExecutingCard.ParentExplainNode = rightResultExplainNode;

            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

            if (rightResultExplainNode != null)
            {
                FillUpResultToExplainNode(rightQueryExecutingCard, rightResultExplainNode);
            }

#if DEBUG
            //options.Logger.Log("||||||||||||||||||||||||||||||||||||||||||||||||||||~~~~~~~~~~~~");
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
            //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if (!rightQueryExecutingCard.IsSuccess)
            {
                if(rightResultExplainNode != null)
                {
                    rightResultExplainNode.AdditionalInformation.Add("!rightQueryExecutingCard.IsSuccess");
                }

                return;
            }

            queryExecutingCard.IsSuccess = true;

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList.Concat(rightQueryExecutingCard.UsedKeysList));

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;
            var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCard.IsPostFiltersListOnly && rightQueryExecutingCard.IsPostFiltersListOnly)
            {
                if(currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftQueryExecutingCard.IsPostFiltersListOnly && rightQueryExecutingCard.IsPostFiltersListOnly");
                }

                queryExecutingCard.IsPostFiltersListOnly = true;

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: !leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                //throw new NotImplementedException();
#endif

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: !leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

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

                            var resultOfComparison = EqualityCompare(leftVal, rightVal, null, null, null, options, null, dataSource);

#if DEBUG
                            //options.Logger.Log($"resultOfComparison = {resultOfComparison}");
#endif

                            if (resultOfComparison)
                            {
                                if(leftVal.Kind == KindOfLogicalQueryNode.Relation && rightVal.Kind != KindOfLogicalQueryNode.Relation)
                                {
                                    varValuesList.Add((varName, rightVal));
                                }
                                else
                                {
                                    varValuesList.Add((varName, leftVal));
                                }                                
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

                    if (!isFit)
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

            if (resultsOfQueryToRelationList.Any())
            {
                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
            }
            else
            {
                queryExecutingCard.IsSuccess = false;
            }

#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCardForOrOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            rightQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if(!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess)
            {
                return;
            }

            if (leftQueryExecutingCard.IsPostFiltersListOnly && rightQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;
            var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            if (rightQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (rightQueryExecutingCard.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

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

                            var resultOfComparison = EqualityCompare(leftVal, rightVal, null, null, null, options, null, dataSource);

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
            //options.Logger.Log("End");
#endif
        }

        private void FillExecutingCardForIsOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(leftExpr) = {DebugHelperForRuleInstance.ToString(leftExpr)}");
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(rightExpr) = {DebugHelperForRuleInstance.ToString(rightExpr)}");
#endif

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);

#if DEBUG
                //options.Logger.Log($"resultOfComparison = {resultOfComparison}");
#endif

                if(resultOfComparison == true)
                {
                    queryExecutingCard.IsSuccess = true;
                    queryExecutingCard.UsedKeysList.Add(leftExpr.Name);
                    queryExecutingCard.UsedKeysList.Add(rightExpr.Name);
                }

                return;
            }

            if(leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"DebugHelperForRuleInstance.ToString(processedExpr) has been added to queryExecutingCard.PostFiltersList = {DebugHelperForRuleInstance.ToString(processedExpr)}");
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForIsNotOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);

#if DEBUG
                //options.Logger.Log($"resultOfComparison = {resultOfComparison}");
#endif

                if (resultOfComparison == false)
                {
                    queryExecutingCard.IsSuccess = true;
                    queryExecutingCard.UsedKeysList.Add(leftExpr.Name);
                    queryExecutingCard.UsedKeysList.Add(rightExpr.Name);
                }

                return;
            }

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.More,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar");
                }

                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.MoreOrEqual,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar");
                }

                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.Less,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar");
                }

                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.LessOrEqual,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("It will be processed in post filters: leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar || rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar");
                }

                var postFilter = new PostFilterOfQueryExecutingCardForPersistLogicalData();
                postFilter.ProcessedExpr = processedExpr;

                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.IsPostFiltersListOnly = true;
                queryExecutingCard.PostFiltersList.Add(postFilter);

#if DEBUG
                //options.Logger.Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
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
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if (leftQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            if (leftQueryExecutingCard.ResultsOfQueryToRelationList.Any())
            {
                throw new NotImplementedException();
            }

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
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
            //foreach (var item in queryExecutingCard.KnownInfoList)
            //{
            //    options.Logger.Log($"item = {item}");
            //}
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithDirectFactQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedBaseRulePart = processedExpr;
                currentExplainNode.TargetRelation = queryExecutingCard.TargetRelation;
            }

            var targetRelationName = queryExecutingCard.TargetRelation;

            var usedKeysList = queryExecutingCard.UsedKeysList;

            var useInheritance = options.UseInheritance;
            var inheritanceResolver = _inheritanceResolver;

            var targetRelationsList = processedExpr.RelationsDict[targetRelationName];

#if DEBUG
            //options.Logger.Log($"targetRelationName = {targetRelationName}");
            //options.Logger.Log($"targetRelationName = {targetRelationName.NameValue}");
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
            reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Relation;
            reasonOfFuzzyLogicResolving.RelationName = targetRelationName;

            foreach (var targetRelation in targetRelationsList)
            {
#if DEBUG          
                //options.Logger.Log($"targetRelation = {targetRelation.ToHumanizedString()}");
                //options.Logger.Log($"targetRelation.CountParams = {targetRelation.CountParams}");
                //options.Logger.Log($"queryExecutingCard.CountParams = {queryExecutingCard.CountParams}");
#endif

                LogicalSearchExplainNode relationWithDirectFactQueryProcessTargetRelationExplainNode = null;

                if (currentExplainNode != null)
                {
                    relationWithDirectFactQueryProcessTargetRelationExplainNode = new LogicalSearchExplainNode()
                    {
                        Kind = KindOfLogicalSearchExplainNode.RelationWithDirectFactQueryProcessTargetRelation,
                        ProcessedLogicalQueryNode = targetRelation
                    };

                    LogicalSearchExplainNode.LinkNodes(currentExplainNode, relationWithDirectFactQueryProcessTargetRelationExplainNode);
                }

                if (targetRelation.CountParams != queryExecutingCard.CountParams)
                {
                    if (relationWithDirectFactQueryProcessTargetRelationExplainNode != null)
                    {
                        relationWithDirectFactQueryProcessTargetRelationExplainNode.AdditionalInformation.Add($"targetRelation.ParamsList.Count != queryExecutingCard.CountParams: {targetRelation.ParamsList.Count} != {queryExecutingCard.CountParams}");
                    }

                    continue;
                }

#if DEBUG
                //options.Logger.Log($"DebugHelperForRuleInstance.ToString(targetRelation) = {DebugHelperForRuleInstance.ToString(targetRelation)}");
                //options.Logger.Log($"targetRelation = {targetRelation}");
                //options.Logger.Log($"targetRelation = {targetRelation.GetHumanizeDbgString()}");
                //options.Logger.Log($"targetRelation.Name = {targetRelation.Name}");
#endif

                usedKeysList.Add(targetRelation.Name);

                var paramsListOfTargetRelation = targetRelation.ParamsList;

                var isFit = true;

                var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                comparisonQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                comparisonQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;


                foreach (var knownInfo in queryExecutingCard.KnownInfoList)
                {
#if DEBUG
                    //options.Logger.Log($"knownInfo = {knownInfo}");
#endif

                    List<StrongIdentifierValue> additionalKeys_1 = null;

                    if (useInheritance)
                    {
                        var knownInfoKind = knownInfo.Kind;

                        switch(knownInfoKind)
                        {
                            case KindOfLogicalQueryNode.Concept:
                            case KindOfLogicalQueryNode.Entity:                            
                                additionalKeys_1 = inheritanceResolver.GetSuperClassesKeysList(knownInfo.Expression.Name, options.LocalCodeExecutionContext);
                                break;

                            case KindOfLogicalQueryNode.Value:
                            case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                            case KindOfLogicalQueryNode.Relation:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(knownInfoKind), knownInfoKind, null);
                        }                     
                    }

#if DEBUG
                    //options.Logger.Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //options.Logger.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        List<StrongIdentifierValue> additionalKeys_2 = null;

                        if (useInheritance && paramOfTargetRelation.IsKeyRef)
                        {
                            additionalKeys_2 = inheritanceResolver.GetSuperClassesKeysList(paramOfTargetRelation.Name, options.LocalCodeExecutionContext);
                        }

#if DEBUG
                        //options.Logger.Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

                        var resultOfComparison = CompareKnownInfoAndExpressionNode(knownInfo, paramOfTargetRelation, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard, dataSource);

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
                //options.Logger.Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
#endif

                if(relationWithDirectFactQueryProcessTargetRelationExplainNode != null)
                {
                    relationWithDirectFactQueryProcessTargetRelationExplainNode.IsFit = isFit;
                }

                if (isFit)
                {
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

                    if(queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam)
                    {
                        var resultOfQueryToRelation = new ResultOfQueryToRelation();

                        foreach (var paramOfTargetRelation in targetRelation.ParamsList)
                        {
#if DEBUG
                            //options.Logger.Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                            if(paramOfTargetRelation.IsExpression)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                                resultOfVarOfQueryToRelation.NameOfVar = NameHelper.CreateLogicalVarName();
                                resultOfVarOfQueryToRelation.FoundExpression = paramOfTargetRelation;
                                resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);
                            }
                        }

                        queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                        queryExecutingCard.IsSuccess = true;
                    }
                    else
                    {
                        if (queryExecutingCard.VarsInfoList.Any())
                        {
                            if (DetectExpressionInParamOfRelation(queryExecutingCard.VarsInfoList, paramsListOfTargetRelation))
                            {
                                var resultCache = new List<List<ResultOfVarOfQueryToRelation>>();

                                foreach (var varItem in queryExecutingCard.VarsInfoList)
                                {
#if DEBUG
                                    //options.Logger.Log($"varItem ($%^) = {varItem}");
#endif

                                    var resultCacheItem = new List<ResultOfVarOfQueryToRelation>();
                                    resultCache.Add(resultCacheItem);

                                    var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

#if DEBUG
                                    //options.Logger.Log($"paramOfTargetRelation ($%^) = {paramOfTargetRelation}");
#endif

                                    if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                                    {
                                        continue;
                                    }

#if DEBUG
                                    //options.Logger.Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation}");
                                    //options.Logger.Log($"NEXT options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key) = {options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key)}");
#endif

                                    if (paramOfTargetRelation.IsExpression)
                                    {
#if DLSR
                throw new NotImplementedException();
#endif

                                        var queryExecutingCardForExprInParameter = new QueryExecutingCardForIndexedPersistLogicalData();

                                        //queryExecutingCardForGroup.KnownInfoList = queryExecutingCard?.KnownInfoList;
                                        queryExecutingCardForExprInParameter.IsFetchingAllValuesForResolvingExpressionParam = true;

                                        FillExecutingCard(paramOfTargetRelation, queryExecutingCardForExprInParameter, dataSource, options);

                                        queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExprInParameter.UsedKeysList);

#if DEBUG
                                        //options.Logger.Log($"%22%^%^ paramOfTargetRelation = {paramOfTargetRelation}");
                                        //options.Logger.Log($"%22%^%^ queryExecutingCardForExprInParameter = {queryExecutingCardForExprInParameter}");
#endif

                                        if(queryExecutingCardForExprInParameter.IsSuccess && queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList.Any())
                                        {
                                            foreach(var resultItem in queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList)
                                            {
                                                foreach(var resultVarItem in resultItem.ResultOfVarOfQueryToRelationList)
                                                {
#if DEBUG
                                                    //options.Logger.Log($"resultVarItem = {resultVarItem}");
#endif

                                                    var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                                                    resultOfVarOfQueryToRelation.NameOfVar = varItem.NameOfVar;
                                                    resultOfVarOfQueryToRelation.FoundExpression = resultVarItem.FoundExpression;
                                                    resultCacheItem.Add(resultOfVarOfQueryToRelation);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
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
                                        resultCacheItem.Add(resultOfVarOfQueryToRelation);
                                    }
                                }

                                var linearizedItems = CollectionCombinationHelper.Combine(resultCache);

                                foreach(var linearizedItem in linearizedItems)
                                {
#if DEBUG
                                    //options.Logger.Log($"linearizedItem = {linearizedItem.WriteListToString()}");
#endif

                                    var resultOfQueryToRelation = new ResultOfQueryToRelation();
                                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList = linearizedItem;

                                    queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                                }

                                queryExecutingCard.IsSuccess = true;

#if DEBUG
                                //options.Logger.Log($"/////////////////////////////////////////");
#endif

                                //throw new NotImplementedException();
                            }
                            else
                            {
                                var resultOfQueryToRelation = new ResultOfQueryToRelation();

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

                                    if (paramOfTargetRelation.IsExpression)
                                    {
                                        throw new NotImplementedException();
                                    }
                                    else
                                    {
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
                                }

                                if (resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Count != queryExecutingCard.VarsInfoList.Count)
                                {
                                    continue;
                                }

                                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);

                                queryExecutingCard.IsSuccess = true;
                            }
                        }
                        else
                        {
                            queryExecutingCard.IsSuccess = true;
                        }
                    }

                    queryExecutingCard.UsedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                    queryExecutingCard.ResultsOfQueryToRelationList.AddRange(comparisonQueryExecutingCard.ResultsOfQueryToRelationList);

#if DEBUG
                    //options.Logger.Log($"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& queryExecutingCard = {queryExecutingCard}");
#endif
                }
            }
        }

        private bool DetectExpressionInParamOfRelation(IList<QueryExecutingCardAboutVar> varsInfoList, IList<LogicalQueryNode> paramsListOfTargetRelation)
        {
            foreach (var varItem in varsInfoList)
            {
                var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

                if(paramOfTargetRelation.IsExpression)
                {
                    return true;
                }
            }

            return false;
        }

        private void FillExecutingCardForGroupLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

#if DEBUG
            if (parentExplainNode == null)
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
            else
            {
#if DLSR
                throw new NotImplementedException();
#endif
            }
#endif

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (!leftQueryExecutingCard.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

                return;
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();

                FillExecutingCardUsingPostFiltersList(leftQueryExecutingCard, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

                AppendResults(queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);
            }
            else
            {
                AppendResults(leftQueryExecutingCard, queryExecutingCard);
            }

#if DEBUG
            //if (leftQueryExecutingCard.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}

            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            //throw new NotImplementedException();
        }

        private bool CompareKnownInfoAndExpressionNode(QueryExecutingCardAboutKnownInfo knownInfo, LogicalQueryNode expressionNode, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            var knownInfoExpression = knownInfo.Expression;

            return EqualityCompare(knownInfoExpression, expressionNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource);
        }

        private void FillExecutingCardForCallingFromRelationForProduction(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedBaseRulePart = processedExpr;
                currentExplainNode.TargetRelation = queryExecutingCard.TargetRelation;
            }

            var targetRelationsList = processedExpr.RelationsDict[queryExecutingCard.TargetRelation];

#if DEBUG
            //options.Logger.Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            if (targetRelationsList.Count != 1)
            {
                if(currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add($"{nameof(targetRelationsList)} should has 1 item instead of {targetRelationsList.Count}");
                }

                return;
            }

            var targetRelation = targetRelationsList.First();

#if DEBUG
            //options.Logger.Log($"targetRelation = {targetRelation}");
#endif

            if (targetRelation.ParamsList.Count != queryExecutingCard.CountParams)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add($"targetRelation.ParamsList.Count != queryExecutingCard.CountParams: {targetRelation.ParamsList.Count} != {queryExecutingCard.CountParams}");
                }

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
            //if (targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            LogicalSearchExplainNode relationWithProductionNextPartsCollectorExplainNode = null;

            if (currentExplainNode != null)
            {
                relationWithProductionNextPartsCollectorExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionNextPartsCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, relationWithProductionNextPartsCollectorExplainNode);
            }

            var nextPartsList = processedExpr.GetNextPartsList();

#if DEBUG
            //options.Logger.Log($"nextPartsList.Count = {nextPartsList.Count}");
#endif

            foreach (var nextPart in nextPartsList)
            {
#if DEBUG
                //options.Logger.Log($"nextPart = {DebugHelperForRuleInstance.BaseRulePartToString(nextPart)}");
#endif

                LogicalSearchExplainNode nextPartLocalResultExplainNode = null;

                if (relationWithProductionNextPartsCollectorExplainNode != null)
                {
                    nextPartLocalResultExplainNode = new LogicalSearchExplainNode()
                    {
                        Kind = KindOfLogicalSearchExplainNode.Result
                    };

                    LogicalSearchExplainNode.LinkNodes(relationWithProductionNextPartsCollectorExplainNode, nextPartLocalResultExplainNode);
                }

                var queryExecutingCardForNextPart = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForNextPart.VarsInfoList = targetRelation.VarsInfoList;
                queryExecutingCardForNextPart.KnownInfoList = targetKnownInfoList;
                queryExecutingCardForNextPart.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                queryExecutingCardForNextPart.ParentExplainNode = nextPartLocalResultExplainNode;

                FillExecutingCardForCallingFromOtherPart(nextPart, queryExecutingCardForNextPart, dataSource, options);

                if(nextPartLocalResultExplainNode != null)
                {
                    FillUpResultToExplainNode(queryExecutingCardForNextPart, nextPartLocalResultExplainNode);
                }

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

                if (queryExecutingCardForNextPart.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                if (queryExecutingCardForNextPart.PostFiltersList.Any())
                {
                    throw new NotImplementedException();
                }

                queryExecutingCard.IsSuccess = queryExecutingCardForNextPart.IsSuccess;

                var resultsOfQueryToRelationList = queryExecutingCardForNextPart.ResultsOfQueryToRelationList;

                if (resultsOfQueryToRelationList.Count > 0)
                {
                    var varsInfoList = queryExecutingCard.VarsInfoList;

                    var backKeysDict = new Dictionary<StrongIdentifierValue, StrongIdentifierValue>();

#if DEBUG
                    //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                    //options.Logger.Log($"processedExpr = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
#endif

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
#if DEBUG
                        //options.Logger.Log($"resultOfQueryToRelation = {resultOfQueryToRelation}");
#endif

                        var newResultOfQueryToRelation = new ResultOfQueryToRelation();
                        var newResultOfVarOfQueryToRelationList = new List<ResultOfVarOfQueryToRelation>();

                        foreach (var resultOfVarOfQueryToRelation in resultOfQueryToRelation.ResultOfVarOfQueryToRelationList)
                        {
#if DEBUG
                            //options.Logger.Log($"resultOfVarOfQueryToRelation = {resultOfVarOfQueryToRelation}");
#endif

                            var internalKeyOfVar = resultOfVarOfQueryToRelation.NameOfVar;

#if DEBUG
                            //options.Logger.Log($"internalKeyOfVar = {internalKeyOfVar}");
#endif

                            if (backKeysDict.ContainsKey(internalKeyOfVar))
                            {
                                var externalKeyOfVar = backKeysDict[internalKeyOfVar];

#if DEBUG
                                //options.Logger.Log($"externalKeyOfVar = {externalKeyOfVar}");
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

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionNextPart
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedBaseRulePart = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode()
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

            queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
            queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            queryExecutingCardForExpression.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCardForExpression, resultExplainNode);
            }

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

#if DEBUG
            //options.Logger.Log($"%%%%%%%% queryExecutingCardForExpression = {queryExecutingCardForExpression}");
            //if (queryExecutingCardForExpression.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            if (queryExecutingCardForExpression.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (queryExecutingCardForExpression.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            queryExecutingCard.ResultsOfQueryToRelationList = queryExecutingCardForExpression.ResultsOfQueryToRelationList;
            queryExecutingCard.IsSuccess = queryExecutingCardForExpression.IsSuccess;

#if DEBUG
            //options.Logger.Log("End  ^&*^&*");
#endif
        }

        private bool EqualityCompare(LogicalQueryNode expressionNode1, LogicalQueryNode expressionNode2, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reason, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //options.Logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            //options.Logger.Log($"expressionNode1 = {expressionNode1}");
            //options.Logger.Log($"expressionNode2 = {expressionNode2}");
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
            //options.Logger.Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            if (expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar && (expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Entity))
            {
#if DEBUG
                //options.Logger.Log($"%%%%%% expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar && expressionNode2.IsKeyRef");
#endif

                var resultOfQueryToRelation = new ResultOfQueryToRelation();

                var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                resultOfVarOfQueryToRelation.NameOfVar = expressionNode1.Name;
                resultOfVarOfQueryToRelation.FoundExpression = expressionNode2;
                resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);

                queryExecutingCard.UsedKeysList.Add(expressionNode2.Name);

                return true;
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.Concept || expressionNode1.Kind == KindOfLogicalQueryNode.Entity) && (expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Entity))
            {
                var key_1 = expressionNode1.Name;
                var key_2 = expressionNode2.Name;

#if DEBUG
                //if((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                //{
                    //options.Logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
                    //options.Logger.Log($"expressionNode1 = {expressionNode1}");
                    //options.Logger.Log($"expressionNode2 = {expressionNode2}");
                    //options.Logger.Log($"key_1 = {key_1}");
                    //options.Logger.Log($"key_2 = {key_2}");
                    //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                    //options.Logger.Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
                    //options.Logger.Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
                //}
#endif

                if (key_1 == key_2)
                {
#if DEBUG
                    //if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    //{
                    //    //options.Logger.Log($"key_1 == key_2 = {key_1 == key_2}");
                    //}                    
#endif

                    if(queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }

                if (additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2))
                {
#if DEBUG
                    //if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    //{
                    //    //options.Logger.Log("additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2)");
                    //}                    
#endif

                    if (queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_1);
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }

                if (additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1))
                {
#if DEBUG
                    //if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    //{
                    //    //options.Logger.Log("additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1)");
                    //}                    
#endif

                    if (queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_1);
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }

                //if (additionalKeys_1 != null && additionalKeys_2 != null && additionalKeys_1.Intersect(additionalKeys_2).Any())
                //{
                //    if (queryExecutingCard != null)
                //    {
                //        queryExecutingCard.UsedKeysList.Add(key_1);
                //        queryExecutingCard.UsedKeysList.Add(key_2);
                //    }

                //    return true;
                //}

                return false;
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Value && expressionNode2.Kind == KindOfLogicalQueryNode.Value)
            {
                var sysValue1 = expressionNode1.Value.GetSystemValue();
                var sysValue2 = expressionNode2.Value.GetSystemValue();

#if DEBUG
                //options.Logger.Log($"sysValue1 = {sysValue1}");
                //options.Logger.Log($"sysValue1?.GetType().FullName = {sysValue1?.GetType().FullName}");
                //options.Logger.Log($"sysValue2 = {sysValue2}");
                //options.Logger.Log($"sysValue2?.GetType().FullName = {sysValue2?.GetType().FullName}");
                //options.Logger.Log($"sysValue1.Equals(sysValue2) = {sysValue1.Equals(sysValue2)}");
#endif

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            if ((expressionNode1.IsKeyRef && expressionNode2.Kind == KindOfLogicalQueryNode.Value && (expressionNode2.Value.IsLogicalValue || expressionNode2.Value.IsNumberValue)) || (expressionNode2.IsKeyRef && expressionNode1.Kind == KindOfLogicalQueryNode.Value && (expressionNode1.Value.IsNumberValue || expressionNode1.Value.IsLogicalValue)))
            {
#if DEBUG
                //options.Logger.Log("Try to check fuzzy logic!");
#endif

                LogicalQueryNode conceptNode = null;
                LogicalQueryNode valueNode = null;

                if (expressionNode1.IsKeyRef)
                {
                    conceptNode = expressionNode1;
                    valueNode = expressionNode2;
                }
                else
                {
                    conceptNode = expressionNode2;
                    valueNode = expressionNode1;
                }

#if DEBUG
                //options.Logger.Log($"conceptNode = {conceptNode}");
                //options.Logger.Log($"valueNode = {valueNode}");
#endif

                var value = valueNode.Value;
                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                if (value.GetSystemValue() == null || !_numberValueLinearResolver.CanBeResolved(value))
                {
                    return false;
                }

                return _fuzzyLogicResolver.Equals(conceptNode.Name, _numberValueLinearResolver.Resolve(value, localCodeExecutionContext), reason, localCodeExecutionContext);
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence && expressionNode2.Kind == KindOfLogicalQueryNode.Value && (expressionNode2.Value.IsLogicalValue || expressionNode2.Value.IsNumberValue)) || (expressionNode2.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence && expressionNode1.Kind == KindOfLogicalQueryNode.Value && (expressionNode1.Value.IsNumberValue || expressionNode1.Value.IsLogicalValue)))
            {
#if DEBUG
                //options.Logger.Log("Try to check fuzzy logic!");
#endif

                LogicalQueryNode sequenceNode = null;
                LogicalQueryNode valueNode = null;

                if (expressionNode1.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence)
                {
                    sequenceNode = expressionNode1;
                    valueNode = expressionNode2;
                }
                else
                {
                    sequenceNode = expressionNode2;
                    valueNode = expressionNode1;
                }

#if DEBUG
                //options.Logger.Log($"sequenceNode = {sequenceNode}");
                //options.Logger.Log($"valueNode = {valueNode}");
#endif

                var value = valueNode.Value;
                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                if (value.GetSystemValue() == null || !_numberValueLinearResolver.CanBeResolved(value))
                {
                    return false;
                }

                return _fuzzyLogicResolver.Equals(sequenceNode.FuzzyLogicNonNumericSequenceValue, _numberValueLinearResolver.Resolve(value, localCodeExecutionContext), reason, localCodeExecutionContext);
            }

            options.Logger.Log($"expressionNode1 = {expressionNode1}");
            options.Logger.Log($"expressionNode2 = {expressionNode2}");

            throw new NotImplementedException();
        }
    }
}
