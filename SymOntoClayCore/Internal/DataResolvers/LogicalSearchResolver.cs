/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly VarsResolver _varsResolver;
        private readonly SynonymsResolver _synonymsResolver;

        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
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

                foreach(var targetStorage in targetStorageList)
                {
                    targetStorage.UseFacts = true;
                    targetStorage.UseInheritanceFacts = true;
                }

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

#if DEBUG
                //Log($"targetStorageList (NEXT) = {targetStorageList.WriteListToString()}");
#endif

                dataSource = new ConsolidatedDataSource(targetStorageList);

#if DEBUG
                //Log($"dataSource = {dataSource}");
#endif
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

            var loggingProvider = _context.LoggingProvider;
            var kindOfLogicalSearchExplain = loggingProvider.KindOfLogicalSearchExplain;

#if DEBUG
            //Log($"queryExpression (after) = {queryExpression}");
            //Log($"DebugHelperForRuleInstance.ToString(queryExpression) (after) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
            //Log($"kindOfLogicalSearchExplain = {kindOfLogicalSearchExplain}");
#endif

            LogicalSearchExplainNode rootExplainNode = null;
            
            try
            {
                var logicalSearchStorageContext = new LogicalSearchStorageContext(_context, options.LocalCodeExecutionContext, queryExpression);

                var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
                optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
                optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;
                optionsOfFillExecutingCard.UseInheritance = options.UseInheritance;
                optionsOfFillExecutingCard.LocalCodeExecutionContext = options.LocalCodeExecutionContext;
                optionsOfFillExecutingCard.MainStorageContext = _context;
                optionsOfFillExecutingCard.LogicalSearchStorageContext = logicalSearchStorageContext;

#if DEBUG
                //Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

                LogicalSearchExplainNode resultExplainNode = null;

                if (kindOfLogicalSearchExplain != KindOfLogicalSearchExplain.None)
                {
                    rootExplainNode = new LogicalSearchExplainNode(null)
                    {
                        Kind = KindOfLogicalSearchExplainNode.Root
                    };

                    rootExplainNode.CommonChildren = new List<LogicalSearchExplainNode>();

                    rootExplainNode.ProcessedRuleInstance = options.QueryExpression;

                    resultExplainNode = new LogicalSearchExplainNode(rootExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.Result
                    };

                    LogicalSearchExplainNode.LinkNodes(rootExplainNode, resultExplainNode);
                }

                queryExecutingCard.RootParentExplainNode = rootExplainNode;
                queryExecutingCard.ParentExplainNode = resultExplainNode;

                FillExecutingCard(queryExpression, queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
                //Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif

                if (queryExecutingCard.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                if (queryExecutingCard.PostFiltersList.Any())
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

                if (resultExplainNode != null)
                {
                    FillUpResultToExplainNode(queryExecutingCard, resultExplainNode);
                }

                if(kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpAlways)
                {
                    var dumpFileName = loggingProvider.DumpToFile(rootExplainNode);

                    Log($"The explanation of query `{queryExpression.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` has been dumped into file `{dumpFileName}`.");
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Log($"e = {e}");
#endif

                var sb = new StringBuilder();
                sb.AppendLine($"Error in query: {queryExpression.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

                if (kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpIfError || kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpAlways)
                {
                    var dumpFileName = loggingProvider.DumpToFile(rootExplainNode);

                    sb.AppendLine($"The explanation has been dumped into file `{dumpFileName}`.");
                }

                Error(sb.ToString());

                Error(e);

                throw;
            }

#if DEBUG
            //Log("End");
#endif

            return result;
        }

        private void AppendResults(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, bool setIsSuccessIfTrue = false)
        {
            if(setIsSuccessIfTrue)
            {
                if(sourceQueryExecutingCard.IsSuccess)
                {
                    destQueryExecutingCard.IsSuccess = true;
                }
            }
            else
            {
                destQueryExecutingCard.IsSuccess = sourceQueryExecutingCard.IsSuccess;
            }            

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
            //Log("Begin");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RuleInstanceQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedRuleInstance = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForPart_1.RootParentExplainNode = rootParentExplainNode;
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
            //Log("End");
#endif
        }

        private void FillExecutingCard(PrimaryRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"Begin ~~~~~~ processedExpr = {processedExpr}");
            //Log($"Begin ~~~~~~ processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;

            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.PrimaryRulePartQuery
                };

                currentExplainNode.ProcessedPrimaryRulePart = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForExpression.RootParentExplainNode = rootParentExplainNode;
            queryExecutingCardForExpression.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            //Log($"~~~~~~ (after) processedExpr = {processedExpr}");
            //Log($"~~~~~~ (after) processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
            //Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCardForExpression, resultExplainNode);
            }

            if (!queryExecutingCardForExpression.IsSuccess)
            {
                if (parentExplainNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

                return;
            }

#if DEBUG
            //Log($"queryExecutingCardForExpression.PostFiltersList.Any() = {queryExecutingCardForExpression.PostFiltersList.Any()}");
#endif

            if (queryExecutingCardForExpression.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.RootParentExplainNode = rootParentExplainNode;
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.ParentExplainNode = parentExplainNode;

                var postFilterNode = FillExecutingCardUsingPostFiltersList(queryExecutingCardForExpression, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

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
            //Log($"*************()()()()()()======== queryExecutingCard = {queryExecutingCard}");
            //if (queryExecutingCardForExpression.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}
            //Log("End");
#endif
        }

        private LogicalSearchExplainNode FillExecutingCardUsingPostFiltersList(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, OptionsOfFillExecutingCard options, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //Log($"sourceQueryExecutingCard = {sourceQueryExecutingCard}");
            //Log($"destQueryExecutingCard = {destQueryExecutingCard}");
#endif

            var postFiltersList = sourceQueryExecutingCard.PostFiltersList;

            if(sourceQueryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            var targetSourceQueryExecutingCard = sourceQueryExecutingCard;

            var parentExplainNode = destQueryExecutingCard.ParentExplainNode;
            var rootParentExplainNode = destQueryExecutingCard.RootParentExplainNode;
            List<(LogicalSearchExplainNode, LogicalSearchExplainNode)> explainNodesList = null;

            if(parentExplainNode != null)
            {
                explainNodesList = new List<(LogicalSearchExplainNode, LogicalSearchExplainNode)>();
            }

            foreach (var postFilter in postFiltersList)
            {
                var kindOfBinaryOperator = postFilter.KindOfBinaryOperator;

#if DEBUG
                //Log($"kindOfBinaryOperator = {kindOfBinaryOperator}");
#endif

                var oldTargetSourceQueryExecutingCard = targetSourceQueryExecutingCard;

                targetSourceQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
                targetSourceQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                switch (kindOfBinaryOperator)
                {
                    case KindOfOperatorOfLogicalQueryNode.And:
                        {
                            var explainNode = FillExecutingCardUsingPostFilterListWithAndStrategy(oldTargetSourceQueryExecutingCard, targetSourceQueryExecutingCard, postFilter, options, dataSource);
#if DEBUG
                            //Log($"targetSourceQueryExecutingCard = {targetSourceQueryExecutingCard}");
                            //Log($"explainNode = {explainNode}");
#endif

                            if(parentExplainNode != null)
                            {
                                var resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"targetSourceQueryExecutingCard (after) = {targetSourceQueryExecutingCard}");
            //Log($"explainNodesList?.Count = {explainNodesList?.Count}");
#endif

            if (targetSourceQueryExecutingCard.IsSuccess)
            {
                AppendResults(targetSourceQueryExecutingCard, destQueryExecutingCard);
            }

#if DEBUG
            //Log($"///////////////// destQueryExecutingCard = {destQueryExecutingCard}");
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
                    //Log($"targetResultItem = {targetResultItem}");
                    //Log($"targetItem = {targetItem}");
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
            //Log($"sourceQueryExecutingCard = {sourceQueryExecutingCard}");
            //Log($"destQueryExecutingCard = {destQueryExecutingCard}");
            //Log($"postFilter = {postFilter}");
#endif

            var processedExpr = postFilter.ProcessedExpr;

#if DEBUG
            //Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = sourceQueryExecutingCard.RootParentExplainNode;

            if (sourceQueryExecutingCard.ParentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.PostFilterWithAndStrategy,
                    ProcessedLogicalQueryNode = processedExpr
                };
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;
            var kindOfOperator = processedExpr.KindOfOperator;

#if DEBUG
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            var resultsOfQueryToRelationList = new List<ResultOfQueryToRelation>();
            var usedKeysList = new List<StrongIdentifierValue>();

            if(leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar && rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var leftVariableName = leftExpr.Name;
                var rightVariableName = rightExpr.Name;

#if DEBUG
                //Log($"leftVariableName = {leftVariableName}");
                //Log($"rightVariableName = {rightVariableName}");
#endif

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
#if DEBUG
                    //Log($"sourceResultOfQueryToRelation = {sourceResultOfQueryToRelation}");
#endif

                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

#if DEBUG
                    //Log($"sourceVarNamesList = {JsonConvert.SerializeObject(sourceVarNamesList.Select(p => p.NameValue))}");
#endif

                    if (sourceVarsDict.ContainsKey(leftVariableName) && sourceVarsDict.ContainsKey(rightVariableName))
                    {
                        var sourceLeftNode = sourceVarsDict[leftVariableName];
                        var sourceRightNode = sourceVarsDict[rightVariableName];

#if DEBUG
                        //Log($"sourceLeftNode = {sourceLeftNode}");
                        //Log($"sourceRightNode = {sourceRightNode}");
#endif

                        var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
                        comparisonQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                        var resultOfComparison = CompareForPostFilter(kindOfOperator, sourceLeftNode, sourceRightNode, options, comparisonQueryExecutingCard, dataSource);

#if DEBUG
                        //Log($"resultOfComparison = {resultOfComparison}");
#endif

                        if (resultOfComparison)
                        {
#if DEBUG
                            //Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
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
                //Log($"variableOfFilter = {variableOfFilter}");
                //Log($"valueOfFilter = {nodeOfFilter}");
#endif

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
#if DEBUG
                    //Log($"sourceResultOfQueryToRelation = {sourceResultOfQueryToRelation}");
#endif

                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

#if DEBUG
                    //Log($"sourceVarNamesList = {JsonConvert.SerializeObject(sourceVarNamesList.Select(p => p.NameValue))}");
#endif

                    if (sourceVarsDict.ContainsKey(variableOfFilter))
                    {
                        var sourceNode = sourceVarsDict[variableOfFilter];

#if DEBUG
                        //Log($"sourceNode = {sourceNode}");
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
                        //Log($"resultOfComparison = {resultOfComparison}");
#endif

                        if (resultOfComparison)
                        {
#if DEBUG
                            //Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
#endif

                            resultsOfQueryToRelationList.Add(sourceResultOfQueryToRelation);

                            usedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                        }
                    }
                }
            }

#if DEBUG
            //Log($"resultsOfQueryToRelationList.Count = {resultsOfQueryToRelationList.Count}");
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
            //Log($"destQueryExecutingCard (after) = {destQueryExecutingCard}");
#endif

            return currentExplainNode;
        }

        private bool CompareForPostFilter(KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
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
            //Log($"leftNode = {leftNode}");
            //Log($"rightNode = {rightNode}");
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
            //Log($"kindOfOperator = {kindOfOperator}");
            //Log($"leftNode = {leftNode}");
            //Log($"rightNode = {rightNode}");
#endif

            var localCodeExecutionContext = options.LocalCodeExecutionContext;
            var numberValueLinearResolver = _numberValueLinearResolver;

            if (leftNode.Kind == KindOfLogicalQueryNode.Value && rightNode.Kind == KindOfLogicalQueryNode.Value)
            {
                var leftValue = leftNode.Value;
                var rightValue = rightNode.Value;

#if DEBUG
                //Log($"leftValue = {leftValue}");
                //Log($"rightValue = {rightValue}");
#endif                

                if(numberValueLinearResolver.CanBeResolved(leftValue) && numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //Log($"leftNumberValue = {leftNumberValue}");
                    //Log($"rightNumberValue = {rightNumberValue}");
#endif

                    var leftSystemNullaleValue = leftNumberValue.SystemValue;
                    var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                    //Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                    //Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
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
                //Log($"leftName = {leftName}");
                //Log($"rightValue = {rightValue}");
#endif

                if(numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //Log($"rightNumberValue = {rightNumberValue}");
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
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftName, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
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
                //Log($"leftSequence = {leftSequence}");
                //Log($"rightValue = {rightValue}");                
#endif

                if (numberValueLinearResolver.CanBeResolved(rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(rightValue, localCodeExecutionContext);

#if DEBUG
                    //Log($"rightNumberValue = {rightNumberValue}");
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
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(leftSequence, rightNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(leftSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
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
                //Log($"leftValue = {leftValue}");
                //Log($"rightName = {rightName}");
#endif

                if (numberValueLinearResolver.CanBeResolved(leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);

#if DEBUG
                    //Log($"leftNumberValue = {leftNumberValue}");
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
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if(!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if(eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif
                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightName, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightName, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
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
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
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
                //Log($"leftValue = {leftValue}");
                //Log($"rightSequence = {rightSequence}");
#endif
                if (numberValueLinearResolver.CanBeResolved(leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(leftValue, localCodeExecutionContext);

#if DEBUG
                    //Log($"leftNumberValue = {leftNumberValue}");
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
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif
                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif
                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(rightSequence, leftNumberValue, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(rightSequence, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
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
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
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
            //Log($"kindOfOperator = {kindOfOperator}");
            //Log($"left = {left}");
            //Log($"right = {right}");
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
            //Log($"queryExecutingCard = {queryExecutingCard}");
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
            //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif
        }

        private void FillExecutingCardForRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"processedExpr.Name = {processedExpr.Name}");
            //Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //Log($"processedExpr = {processedExpr}");
#endif

            if (processedExpr.IsQuestion)
            {
                FillExecutingCardForQuestion(processedExpr, queryExecutingCard, dataSource, options);
                return;
            }

#if DEBUG
            //Log($"processedExpr.Name = {processedExpr.Name}");
            //Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //Log($"processedExpr.Params.Count = {processedExpr.ParamsList.Count}");
            //foreach (var param in processedExpr.ParamsList)
            //{
            //    Log($"param = {param}");
            //}
            //Log($"processedExpr.KnownInfoList.Count = {processedExpr.KnownInfoList.Count}");
            //foreach (var knownInfo in processedExpr.KnownInfoList)
            //{
            //    Log($"knownInfo = {knownInfo}");
            //}
            //Log($"VarsInfoList.Count = {processedExpr.VarsInfoList.Count}");
            //foreach (var varInfo in processedExpr.VarsInfoList)
            //{
            //    Log($"varInfo = {varInfo}");
            //}
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            NFillExecutingCardForRelationLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);

#if DEBUG
            //Log($"^^^^^^queryExecutingCard = {queryExecutingCard}");
            //Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //Log($"processedExpr.GetHumanizeDbgString() (after) = {processedExpr.GetHumanizeDbgString()}");

            //Log("End");
#endif
        }

        private void NFillExecutingCardForRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var relationName = processedExpr.Name;

#if DEBUG
            //Log($"relationName = {relationName}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
            //Log($"queryExecutingCard (777) = {queryExecutingCard}");
#endif

            if (!queryExecutingCard.PostFiltersList.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }
            if (queryExecutingCard.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForExpression.RootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            queryExecutingCardForExpression.ParentExplainNode = queryExecutingCard.ParentExplainNode;
            queryExecutingCardForExpression.VarsInfoList = queryExecutingCard.VarsInfoList;
            queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
            queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

            NFillExecutingCardForConcreteRelationLogicalQueryNode(processedExpr, queryExecutingCardForExpression, dataSource, options);

            if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }
            if (queryExecutingCardForExpression.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            AppendResults(queryExecutingCardForExpression, queryExecutingCard, true);

            var synonymsList = _synonymsResolver.GetSynonyms(relationName, dataSource.StoragesList);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //Log($"synonym = {synonym}");
#endif

                queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForExpression.RootParentExplainNode = queryExecutingCard.RootParentExplainNode;
                queryExecutingCardForExpression.ParentExplainNode = queryExecutingCard.ParentExplainNode;
                queryExecutingCardForExpression.VarsInfoList = queryExecutingCard.VarsInfoList;
                queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
                queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

                var newRelation = processedExpr.Clone();
                newRelation.Name = synonym;

#if DEBUG
                //Log($"newRelation = {newRelation}");
                //Log($"newRelation = {newRelation.ToHumanizedString()}");
#endif

                NFillExecutingCardForConcreteRelationLogicalQueryNode(newRelation, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
                //Log($"queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

                if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
                {
                    throw new NotImplementedException();
                }
                if (queryExecutingCardForExpression.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                AppendResults(queryExecutingCardForExpression, queryExecutingCard, true);
            }

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(relationName, options.LocalCodeExecutionContext);

#if DEBUG
            //Log($"superClassesList = {superClassesList.WriteListToString()}");
#endif

            foreach (var superClass in superClassesList)
            {
#if DEBUG
                //Log($"superClass = {superClass}");
#endif

                queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForExpression.RootParentExplainNode = queryExecutingCard.RootParentExplainNode;
                queryExecutingCardForExpression.ParentExplainNode = queryExecutingCard.ParentExplainNode;
                queryExecutingCardForExpression.VarsInfoList = queryExecutingCard.VarsInfoList;
                queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
                queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;

                var newRelation = processedExpr.Clone();
                newRelation.Name = superClass;

#if DEBUG
                //Log($"newRelation = {newRelation}");
                //Log($"newRelation = {newRelation.ToHumanizedString()}");
#endif

                NFillExecutingCardForConcreteRelationLogicalQueryNode(newRelation, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
                //Log($"queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

                if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
                {
                    throw new NotImplementedException();
                }
                if (queryExecutingCardForExpression.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                AppendResults(queryExecutingCardForExpression, queryExecutingCard, true);
            }

            //NFillExecutingCardForConcreteRelationLogicalQueryNode(processedExpr, queryExecutingCard, dataSource, options);
        }

        private void NFillExecutingCardForConcreteRelationLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"processedExpr.Name = {processedExpr.Name}");            
            //Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
            //Log($"processedExpr = {processedExpr}");

            //if(processedExpr.Name.NameValue == "is")
            //{
            //    Log($"processedExpr = {processedExpr}");
            //    Log($"queryExecutingCard = {queryExecutingCard}");
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

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedLogicalQueryNode = processedExpr;

                directFactsCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.ProcessRelationWithDirectFactsCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, directFactsCollectorExplainNode);

                directFactsResultsCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.ResultCollector
                };

                LogicalSearchExplainNode.LinkNodes(directFactsCollectorExplainNode, directFactsResultsCollectorExplainNode);

                directFactsDataSourceCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceCollector
                };

                LogicalSearchExplainNode.LinkNodes(directFactsCollectorExplainNode, directFactsDataSourceCollectorExplainNode);

                directFactsDataSourceResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                };

                LogicalSearchExplainNode.LinkNodes(directFactsDataSourceCollectorExplainNode, directFactsDataSourceResultExplainNode);

                productionCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.ProcessRelationWithProductionCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, productionCollectorExplainNode);

                productionResultsCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.ResultCollector
                };

                LogicalSearchExplainNode.LinkNodes(productionCollectorExplainNode, productionResultsCollectorExplainNode);

                productionDataSourceCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceCollector
                };

                LogicalSearchExplainNode.LinkNodes(productionCollectorExplainNode, productionDataSourceCollectorExplainNode);

                productionDataSourceResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                };

                LogicalSearchExplainNode.LinkNodes(productionDataSourceCollectorExplainNode, productionDataSourceResultExplainNode);
            }

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

#if DEBUG
            //Log($"mergingResult.IsSuccess = {mergingResult.IsSuccess}");
#endif

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

#if DEBUG
            //Log($"targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //Log($"directFactsDataSourceResultExplainNode = {directFactsDataSourceResultExplainNode}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
            //if(targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var rulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(processedExpr.Name, options.LogicalSearchStorageContext, directFactsDataSourceResultExplainNode, rootParentExplainNode);

            if(directFactsDataSourceResultExplainNode != null)
            {
                directFactsDataSourceResultExplainNode.BaseRulePartList = rulePartsOfFactsList;
            }

#if DEBUG
            //Log($"rulePartsOfFactsList?.Count = {rulePartsOfFactsList?.Count}");
#endif
            queryExecutingCard.UsedKeysList.Add(processedExpr.Name);

            if (!rulePartsOfFactsList.IsNullOrEmpty())
            {
                foreach (var rulePartsOfFacts in rulePartsOfFactsList)
                {
#if DEBUG
                    //Log($"processedExpr = {DebugHelperForRuleInstance.ToString(processedExpr)}");
                    //Log($"rulePartsOfFacts = {DebugHelperForRuleInstance.BaseRulePartToString(rulePartsOfFacts)}");
#endif

                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (directFactsResultsCollectorExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
                    queryExecutingCardForTargetFact.RootParentExplainNode = rootParentExplainNode;
                    queryExecutingCardForTargetFact.ParentExplainNode = localResultExplainNode;

                    FillExecutingCardForCallingFromRelationForFact(rulePartsOfFacts, queryExecutingCardForTargetFact, dataSource, options);

#if DEBUG
                    //Log($"++++++queryExecutingCardForTargetFact.IsSuccess = {queryExecutingCardForTargetFact.IsSuccess}");
                    //Log($"++++++queryExecutingCardForTargetFact = {queryExecutingCardForTargetFact}");
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
            //Log($"~~~~~~~~~~~~~~~~~queryExecutingCard = {queryExecutingCard}");
#endif

            var rulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(processedExpr.Name, options.LogicalSearchStorageContext, productionDataSourceResultExplainNode, rootParentExplainNode);

#if DEBUG
            //Log($"rulePartWithOneRelationsList?.Count = {rulePartWithOneRelationsList?.Count}");
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
                    //Log($"processedExpr = {processedExpr}");
                    //Log($"indexedRulePartsOfRule = {indexedRulePartsOfRule}");
                    //Log($"indexedRulePartsOfRule = {DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePartsOfRule)}");
#endif

                    LogicalSearchExplainNode localResultExplainNode = null;

                    if(productionResultsCollectorExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                        {
                            Kind = KindOfLogicalSearchExplainNode.Result
                        };

                        LogicalSearchExplainNode.LinkNodes(productionResultsCollectorExplainNode, localResultExplainNode);
                    }

                    var queryExecutingCardForTargetRule = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetRule.TargetRelation = processedExpr.Name;

#if DEBUG
                    //Log($"Key = {Key}");
                    //Log($"options.EntityDictionary.GetName(Key) = {options.EntityDictionary.GetName(Key)}");
#endif

                    queryExecutingCardForTargetRule.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetRule.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetRule.KnownInfoList = targetKnownInfoList;
                    queryExecutingCardForTargetRule.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                    queryExecutingCardForTargetRule.RootParentExplainNode = rootParentExplainNode;
                    queryExecutingCardForTargetRule.ParentExplainNode = localResultExplainNode;

                    FillExecutingCardForCallingFromRelationForProduction(indexedRulePartsOfRule, queryExecutingCardForTargetRule, dataSource, options);

#if DEBUG
                    //Log($"&&&&&&&&&&&&&&&&&queryExecutingCardForTargetRule.IsSuccess = {queryExecutingCardForTargetRule.IsSuccess}");
                    //Log($"&&&&&&&&&&&&&&&&&queryExecutingCardForTargetRule = {queryExecutingCardForTargetRule}");
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
                    //Log($"!!!!!!!!!!!!!!!!!!queryExecutingCard = {queryExecutingCard}");
#endif
                }
            }

#if DEBUG
            //Log($"###~~~~~!!!!!!!!!!!!!!!!!!queryExecutingCard = {queryExecutingCard}");
            //Log("End");
#endif
        }

        private void FillExecutingCardForQuestion(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"processedExpr.Name = {processedExpr.Name}");
            //Log($"processedExpr.IsQuestion = {processedExpr.IsQuestion}");
            //Log($"processedExpr.Params.Count = {processedExpr.ParamsList.Count}");
            //foreach (var param in processedExpr.ParamsList)
            //{
            //    Log($"param = {param}");
            //}
            //Log($"processedExpr.VarsInfoList.Count = {processedExpr.VarsInfoList.Count}");
            //foreach (var varInfo in processedExpr.VarsInfoList)
            //{
            //    Log($"varInfo = {varInfo}");
            //}
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode dataSourceResultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationQuestionQuery,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                dataSourceResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, dataSourceResultExplainNode);
            }

#if DEBUG
            //Log($"processedExpr.KnownInfoList.Count = {processedExpr.KnownInfoList.Count}");
            //foreach (var tmpKnownInfo in processedExpr.KnownInfoList)
            //{
            //    Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
            //if(targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

#if DEBUG
            //Log($"mergingResult.IsSuccess = {mergingResult.IsSuccess}");
#endif

            var targetKnownInfoList = mergingResult.KnownInfoList;

#if DEBUG
            //Log($"targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
            //if(targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var hasAnnotations = !processedExpr.Annotations.IsNullOrEmpty();

#if DEBUG
            //Log($"hasAnnotations = {hasAnnotations}");
#endif

            var targetRelationsList = dataSource.AllRelationsForProductions(options.LogicalSearchStorageContext, dataSourceResultExplainNode, rootParentExplainNode);

#if DEBUG
            //Log($"targetRelationsList.Count = {targetRelationsList.Count}");
            //foreach (var targetRelation in targetRelationsList)
            //{
            //    Log($"targetRelation.ToHumanizedString() = {targetRelation.ToHumanizedString()}");
            //}
#endif

            if (dataSourceResultExplainNode != null)
            {
                dataSourceResultExplainNode.RelationsList = targetRelationsList;
            }

            var useInheritance = options.UseInheritance;
            var inheritanceResolver = _inheritanceResolver;

            foreach (var targetRelation in targetRelationsList)
            {
                if (targetRelation.CountParams != processedExpr.CountParams)
                {
                    continue;
                }
#if DEBUG
                //Log($"targetRelation.ToHumanizedString() = {targetRelation.ToHumanizedString()}");
                //Log($"targetRelation = {targetRelation}");
                //Log($"hasAnnotations = {hasAnnotations}");
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
                //Log($"NEXT targetRelation.ToHumanizedString() = {targetRelation.ToHumanizedString()}");
                //Log($"NEXT targetRelation = {targetRelation}");
                //Log($"NEXT targetRelation.RuleInstance.Kind = {targetRelation.RuleInstance.Kind}");
#endif

                var paramsListOfTargetRelation = targetRelation.ParamsList;

                var isFit = true;

                var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                comparisonQueryExecutingCard.KnownInfoList = targetKnownInfoList;
                comparisonQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                comparisonQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Relation;
                reasonOfFuzzyLogicResolving.RelationName = targetRelation.Name;

                var kindOfParentRuleInstance = targetRelation.RuleInstance.KindOfRuleInstance;

                foreach (var knownInfo in targetKnownInfoList)
                {
#if DEBUG
                    //Log($"knownInfo = {knownInfo}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
#if DEBUG
                        //Log($"position.Value = {position.Value}");
#endif

                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        if (paramOfTargetRelation.Kind == KindOfLogicalQueryNode.LogicalVar && kindOfParentRuleInstance != KindOfRuleInstance.Fact)
                        {
                            isFit = false;
                            break;
                        }

                        var resultOfComparison = CompareKnownInfoAndParamOfTargetRelation(knownInfo, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard, dataSource);

#if DEBUG
                        //Log($"resultOfComparison = {resultOfComparison}");
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
                //Log($"isFit = {isFit}");
                //Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
#endif

                if (isFit)
                {
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
                        //Log($"n = {n} param = {param}");
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
                        //Log($"resultOfVarOfQueryToRelation = {resultOfVarOfQueryToRelation}");
                        //throw new NotImplementedException();
#endif
                    }

#if DEBUG
                    //Log($"resultOfQueryToRelation = {resultOfQueryToRelation}");
                    //throw new NotImplementedException();
#endif
                }
            }

#if DEBUG
            //LogInstance.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //LogInstance.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //LogInstance.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //Log($"processedExpr.GetHumanizeDbgString() (after)= {processedExpr.GetHumanizeDbgString()}");
            //LogInstance.Log($"this = {this}");
#endif

#if DEBUG
            //throw new NotImplementedException();
            //Log("End");
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
            //Log("||||||||||||||||||||||||||||||||||||||||||||||||||||");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode leftResultExplainNode = null;
            LogicalSearchExplainNode rightResultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.And,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                leftResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, leftResultExplainNode);

                rightResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, rightResultExplainNode);
            }

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            leftQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            leftQueryExecutingCard.ParentExplainNode = leftResultExplainNode;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

#if DEBUG
            //Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
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
            rightQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            rightQueryExecutingCard.ParentExplainNode = rightResultExplainNode;

            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

            if (rightResultExplainNode != null)
            {
                FillUpResultToExplainNode(rightQueryExecutingCard, rightResultExplainNode);
            }

#if DEBUG
            //Log("||||||||||||||||||||||||||||||||||||||||||||||||||||~~~~~~~~~~~~");
            //Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
            //Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
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
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
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
                    currentExplainNode.AdditionalInformation.Add("leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

#if DEBUG
                //Log($"queryExecutingCard = {queryExecutingCard}");
                //throw new NotImplementedException();
#endif

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

#if DEBUG
                //Log($"queryExecutingCard = {queryExecutingCard}");
                //throw new NotImplementedException();
#endif

                return;
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
#if DEBUG
                //Log($"leftResultOfQueryToRelation = {leftResultOfQueryToRelation}");
#endif

                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
#if DEBUG
                    //Log($"rightResultOfQueryToRelation = {rightResultOfQueryToRelation}");
#endif

                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;

                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var isFit = true;

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
#if DEBUG
                        //Log($"varName = {varName}");
#endif

                        var leftVarsDictContainsKey = leftVarsDict.ContainsKey(varName);
                        var rightVarsDictContainsKey = rightVarsDict.ContainsKey(varName);

                        if (leftVarsDictContainsKey && rightVarsDictContainsKey)
                        {
                            var leftVal = leftVarsDict[varName];
                            var rightVal = rightVarsDict[varName];

#if DEBUG
                            //Log($"leftVal = {leftVal}");
                            //Log($"rightVal = {rightVal}");
#endif

                            var resultOfComparison = EqualityCompare(leftVal, rightVal, null, null, null, options, null, dataSource);

#if DEBUG
                            //Log($"resultOfComparison = {resultOfComparison}");
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
                    //Log($"isFit = {isFit}");
#endif

                    if (!isFit)
                    {
                        continue;
                    }

#if DEBUG
                    //Log($"varValuesList.Count = {varValuesList.Count}");
                    //foreach (var varValue in varValuesList)
                    //{
                    //    Log("------------");
                    //    Log($"varValue.Item1 = {varValue.Item1}");
                    //    Log($"varValue.Item2 = {varValue.Item2}");
                    //}
#endif

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
                    //Log($"varValuesDict.Count = {varValuesDict.Count}");
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
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log("End");
#endif
        }

        private void FillExecutingCardForOrOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode leftResultExplainNode = null;
            LogicalSearchExplainNode rightResultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.Or,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                leftResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, leftResultExplainNode);

                rightResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, rightResultExplainNode);
            }

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            leftQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            leftQueryExecutingCard.ParentExplainNode = leftResultExplainNode;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (leftResultExplainNode != null)
            {
                FillUpResultToExplainNode(leftQueryExecutingCard, leftResultExplainNode);
            }

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            rightQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            rightQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            rightQueryExecutingCard.ParentExplainNode = rightResultExplainNode;

            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

            if (rightResultExplainNode != null)
            {
                FillUpResultToExplainNode(rightQueryExecutingCard, rightResultExplainNode);
            }

#if DEBUG
            //Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if (!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess");
                }

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
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess");
                }

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
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess");
                }

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
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            if(!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                return;
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
#if DEBUG
                //Log($"leftResultOfQueryToRelation = {leftResultOfQueryToRelation}");
#endif

                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
#if DEBUG
                    //Log($"rightResultOfQueryToRelation = {rightResultOfQueryToRelation}");
#endif

                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
#if DEBUG
                        //Log($"varName = {varName}");
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
                    //Log($"varValuesList.Count = {varValuesList.Count}");
                    //foreach(var varValue in varValuesList)
                    //{
                    //    Log("------------");
                    //    Log($"varValue.Item1 = {varValue.Item1}");
                    //    Log($"varValue.Item2 = {varValue.Item2}");
                    //}
#endif

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
                    //Log($"varValuesDict.Count = {varValuesDict.Count}");
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
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log("End");
#endif
        }

        private void FillExecutingCardForIsOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.Is,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
            //Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.ToString(processedExpr)}");
            //Log($"DebugHelperForRuleInstance.ToString(leftExpr) = {DebugHelperForRuleInstance.ToString(leftExpr)}");
            //Log($"DebugHelperForRuleInstance.ToString(rightExpr) = {DebugHelperForRuleInstance.ToString(rightExpr)}");
#endif

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);

#if DEBUG
                //Log($"resultOfComparison = {resultOfComparison}");
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
                //Log($"DebugHelperForRuleInstance.ToString(processedExpr) has been added to queryExecutingCard.PostFiltersList = {DebugHelperForRuleInstance.ToString(processedExpr)}");
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForIsNotOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.IsNot,
                    ProcessedLogicalQueryNode = processedExpr
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
#endif

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);

#if DEBUG
                //Log($"resultOfComparison = {resultOfComparison}");
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
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
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
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
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
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
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
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.ToHumanizedString()}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"leftExpr = {leftExpr}");
            //Log($"rightExpr = {rightExpr}");
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
                //Log($"queryExecutingCard (after) = {queryExecutingCard}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(IEnumerator<StrongIdentifierValue> varNamesListEnumerator, Dictionary<StrongIdentifierValue, List<(StrongIdentifierValue, LogicalQueryNode)>> varValuesDict, List<(StrongIdentifierValue, LogicalQueryNode)> resultVarValuesList, IList<ResultOfQueryToRelation> resultsOfQueryToRelationList, OptionsOfFillExecutingCard options)
        {
            var varName = varNamesListEnumerator.Current;

#if DEBUG
            //Log($"varName = {varName}");
#endif

            var targetVarsValuesList = varValuesDict[varName];

#if DEBUG
            //Log($"targetVarsValuesList.Count = {targetVarsValuesList.Count}");
#endif

            foreach (var varValue in targetVarsValuesList)
            {
#if DEBUG
                //Log($"varValue = {varValue}");
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
                    //Log($"newResultVarValuesList.Count = {newResultVarValuesList.Count}");
#endif

                    var resultOfQueryToRelation = new ResultOfQueryToRelation();

                    foreach (var newResultVarValue in newResultVarValuesList)
                    {
#if DEBUG
                        //Log($"newResultVarValue = {newResultVarValue}");
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
            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.OperatorQuery,
                    KindOfOperator = KindOfOperatorOfLogicalQueryNode.Not,
                    ProcessedLogicalQueryNode = processedExpr
                };

                currentExplainNode.ProcessedLogicalQueryNode = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            leftQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            leftQueryExecutingCard.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(leftQueryExecutingCard, resultExplainNode);
            }

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
            //Log($"queryExecutingCard = {queryExecutingCard}");
#endif
            if (parentExplainNode != null)
            {
                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }
        }

        private void FillExecutingCardForCallingFromRelationForFact(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {processedExpr}");
            //Log($"DebugHelperForRuleInstance.ToString(processedExpr) = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
            //foreach (var item in queryExecutingCard.KnownInfoList)
            //{
            //    Log($"item = {item}");
            //}
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
            //Log($"targetRelationName = {targetRelationName}");
            //Log($"targetRelationName = {targetRelationName.NameValue}");
            //Log($"targetRelationsList.Count = {targetRelationsList.Count}");
#endif

            var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
            reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Relation;
            reasonOfFuzzyLogicResolving.RelationName = targetRelationName;

            foreach (var targetRelation in targetRelationsList)
            {
#if DEBUG          
                //Log($"targetRelation = {targetRelation.ToHumanizedString()}");
                //Log($"targetRelation.CountParams = {targetRelation.CountParams}");
                //Log($"queryExecutingCard.CountParams = {queryExecutingCard.CountParams}");
#endif

                LogicalSearchExplainNode relationWithDirectFactQueryProcessTargetRelationExplainNode = null;

                if (currentExplainNode != null)
                {
                    relationWithDirectFactQueryProcessTargetRelationExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
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
                //Log($"DebugHelperForRuleInstance.ToString(targetRelation) = {DebugHelperForRuleInstance.ToString(targetRelation)}");
                //Log($"targetRelation = {targetRelation}");
                //Log($"targetRelation = {targetRelation.ToHumanizedString()}");
                //Log($"targetRelation.Name = {targetRelation.Name}");
#endif

                usedKeysList.Add(targetRelation.Name);

                var paramsListOfTargetRelation = targetRelation.ParamsList;

                var isFit = true;

                var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                comparisonQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                comparisonQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                comparisonQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                foreach (var knownInfo in queryExecutingCard.KnownInfoList)
                {
#if DEBUG
                    //Log($"knownInfo = {knownInfo}");
#endif

                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                        //Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                        var resultOfComparison = CompareKnownInfoAndParamOfTargetRelation(knownInfo, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard, dataSource);

#if DEBUG
                        //Log($"resultOfComparison = {resultOfComparison}");
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
                //Log($"isFit = {isFit}");
                //Log($"comparisonQueryExecutingCard = {comparisonQueryExecutingCard}");
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
                    //Log($"queryExecutingCard.VarsInfoList.Count = {queryExecutingCard.VarsInfoList.Count}");
#endif

                    if(queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam)
                    {
                        var resultOfQueryToRelation = new ResultOfQueryToRelation();

                        foreach (var paramOfTargetRelation in targetRelation.ParamsList)
                        {
#if DEBUG
                            //Log($"paramOfTargetRelation = {paramOfTargetRelation}");
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
                                    //Log($"varItem ($%^) = {varItem}");
#endif

                                    var resultCacheItem = new List<ResultOfVarOfQueryToRelation>();
                                    resultCache.Add(resultCacheItem);

                                    var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

#if DEBUG
                                    //Log($"paramOfTargetRelation ($%^) = {paramOfTargetRelation}");
#endif

                                    if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                                    {
                                        continue;
                                    }

#if DEBUG
                                    //Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation}");
                                    //Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation?.ToHumanizedString()}");
                                    //Log($"NEXT options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key) = {options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key)}");
                                    //Log($"paramOfTargetRelation.IsExpression = {paramOfTargetRelation.IsExpression}");
#endif

                                    if (paramOfTargetRelation.IsExpression)
                                    {
                                        LogicalSearchExplainNode fetchingAllValuesForResolvingExpressionParamExplainNode = null;
                                        LogicalSearchExplainNode fetchingAllValuesForResolvingExpressionParamResultExplainNode = null;

                                        if (relationWithDirectFactQueryProcessTargetRelationExplainNode != null)
                                        {
                                            fetchingAllValuesForResolvingExpressionParamExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                            {
                                                Kind = KindOfLogicalSearchExplainNode.FetchingAllValuesForResolvingExpressionParam
                                            };

                                            LogicalSearchExplainNode.LinkNodes(relationWithDirectFactQueryProcessTargetRelationExplainNode, fetchingAllValuesForResolvingExpressionParamExplainNode);

                                            fetchingAllValuesForResolvingExpressionParamResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                            {
                                                Kind = KindOfLogicalSearchExplainNode.Result
                                            };

                                            LogicalSearchExplainNode.LinkNodes(fetchingAllValuesForResolvingExpressionParamExplainNode, fetchingAllValuesForResolvingExpressionParamResultExplainNode);
                                        }

                                        var queryExecutingCardForExprInParameter = new QueryExecutingCardForIndexedPersistLogicalData();

                                        //queryExecutingCardForGroup.KnownInfoList = queryExecutingCard?.KnownInfoList;
                                        queryExecutingCardForExprInParameter.IsFetchingAllValuesForResolvingExpressionParam = true;
                                        queryExecutingCardForExprInParameter.RootParentExplainNode = rootParentExplainNode;
                                        queryExecutingCardForExprInParameter.ParentExplainNode = fetchingAllValuesForResolvingExpressionParamResultExplainNode;

                                        FillExecutingCard(paramOfTargetRelation, queryExecutingCardForExprInParameter, dataSource, options);

                                        if (fetchingAllValuesForResolvingExpressionParamResultExplainNode != null)
                                        {
                                            FillUpResultToExplainNode(queryExecutingCardForExprInParameter, fetchingAllValuesForResolvingExpressionParamResultExplainNode);
                                        }

                                        queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExprInParameter.UsedKeysList);

#if DEBUG
                                        //Log($"%22%^%^ paramOfTargetRelation = {paramOfTargetRelation}");
                                        //Log($"%22%^%^ queryExecutingCardForExprInParameter = {queryExecutingCardForExprInParameter}");
#endif

                                        if(queryExecutingCardForExprInParameter.IsSuccess && queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList.Any())
                                        {
                                            foreach(var resultItem in queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList)
                                            {
                                                foreach(var resultVarItem in resultItem.ResultOfVarOfQueryToRelationList)
                                                {
#if DEBUG
                                                    //Log($"resultVarItem = {resultVarItem}");
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
                                    //Log($"linearizedItem = {linearizedItem.WriteListToString()}");
#endif

                                    var resultOfQueryToRelation = new ResultOfQueryToRelation();
                                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList = linearizedItem;

                                    queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                                }

                                queryExecutingCard.IsSuccess = true;

#if DEBUG
                                //Log($"/////////////////////////////////////////");
#endif

                                //throw new NotImplementedException();
                            }
                            else
                            {
                                var resultOfQueryToRelation = new ResultOfQueryToRelation();

                                foreach (var varItem in queryExecutingCard.VarsInfoList)
                                {
#if DEBUG
                                    //Log($"varItem = {varItem}");
#endif

                                    var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

#if DEBUG
                                    //Log($"paramOfTargetRelation = {paramOfTargetRelation}");
#endif

                                    if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                                    {
                                        continue;
                                    }

#if DEBUG
                                    //Log($"NEXT paramOfTargetRelation = {paramOfTargetRelation}");
                                    //Log($"NEXT options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key) = {options.EntityDictionary.GetName(paramOfTargetRelation.AsKeyRef.Key)}");
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
                    //Log($"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& queryExecutingCard = {queryExecutingCard}");
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
            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.GroupQuery
                };

                currentExplainNode.ProcessedLogicalQueryNode = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            leftQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            leftQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            leftQueryExecutingCard.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(leftQueryExecutingCard, resultExplainNode);
            }

            if (!leftQueryExecutingCard.IsSuccess)
            {
                if (parentExplainNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

                return;
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.RootParentExplainNode = rootParentExplainNode;
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.ParentExplainNode = parentExplainNode;

                var postFilterNode = FillExecutingCardUsingPostFiltersList(leftQueryExecutingCard, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

                AppendResults(queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);

                if (postFilterNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(postFilterNode, currentExplainNode);
                }
            }
            else
            {
                if (parentExplainNode != null)
                {
                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                AppendResults(leftQueryExecutingCard, queryExecutingCard);
            }

#if DEBUG
            //if (leftQueryExecutingCard.UsedKeysList.Any())
            //{
            //    throw new NotImplementedException();
            //}

            //Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            //throw new NotImplementedException();
        }

        private bool CompareKnownInfoAndParamOfTargetRelation(QueryExecutingCardAboutKnownInfo knownInfo, LogicalQueryNode paramOfTargetRelation, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            if(CompareKnownInfoAndExpressionNode(knownInfo.Expression, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource))
            {
                return true;
            }

            var additionalKnownInfoExpressions = knownInfo.AdditionalExpressions ?? new List<LogicalQueryNode>();

            var additionalParamOfTargetRelationExpressions = new List<LogicalQueryNode>();
            var varNames = new List<StrongIdentifierValue>();

            if (paramOfTargetRelation.IsExpression)
            {
                LogicalQueryNodeHelper.FillUpInfoAboutComplexExpression(paramOfTargetRelation, additionalParamOfTargetRelationExpressions, varNames);
            }

#if DEBUG
            //Log($"additionalKnownInfoExpressions = {additionalKnownInfoExpressions.Select(p => p.ToHumanizedString()).WritePODListToString()}");
            //Log($"additionalParamOfTargetRelationExpressions = {additionalParamOfTargetRelationExpressions.Select(p => p.ToHumanizedString()).WritePODListToString()}");
            //Log($"additionalParamOfTargetRelationExpressions = {varNames.Select(p => p.NameValue).WritePODListToString()}");
#endif

            if (varNames.Any())
            {
                throw new NotImplementedException();
            }

            if (additionalParamOfTargetRelationExpressions.Any() || additionalKnownInfoExpressions.Any())
            {
                var knownInfoExpressions = new List<LogicalQueryNode>() { knownInfo.Expression };
                knownInfoExpressions.AddRange(additionalKnownInfoExpressions);

                var paramOfTargetRelationExpressions = new List<LogicalQueryNode>() { paramOfTargetRelation };
                paramOfTargetRelationExpressions.AddRange(additionalParamOfTargetRelationExpressions);

                foreach(var knownInfoItem in knownInfoExpressions)
                {
                    foreach(var paramOfTargetRelationItem in paramOfTargetRelationExpressions)
                    {
#if DEBUG
                        //Log($"knownInfoItem = {knownInfoItem.ToHumanizedString()}");
                        //Log($"paramOfTargetRelationItem = {paramOfTargetRelationItem.ToHumanizedString()}");
#endif

                        if (CompareKnownInfoAndExpressionNode(knownInfoItem, paramOfTargetRelationItem, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CompareKnownInfoAndExpressionNode(LogicalQueryNode knownInfo, LogicalQueryNode expressionNode, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            var useInheritance = options.UseInheritance;

            List<StrongIdentifierValue> additionalKeys_1 = null;

            if (useInheritance)
            {
                var knownInfoKind = knownInfo.Kind;

                switch (knownInfoKind)
                {
                    case KindOfLogicalQueryNode.Concept:
                    case KindOfLogicalQueryNode.Entity:
                        additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(knownInfo.Name, options.LocalCodeExecutionContext);
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
            //Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            List<StrongIdentifierValue> additionalKeys_2 = null;

            if (useInheritance && expressionNode.IsKeyRef)
            {
                additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(expressionNode.Name, options.LocalCodeExecutionContext);
            }

#if DEBUG
            //Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            return EqualityCompare(knownInfo, expressionNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource);
        }

        private void FillExecutingCardForCallingFromRelationForProduction(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"processedExpr = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionQuery
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedBaseRulePart = processedExpr;
                currentExplainNode.TargetRelation = queryExecutingCard.TargetRelation;
            }

            var targetRelationsList = processedExpr.RelationsDict[queryExecutingCard.TargetRelation];

#if DEBUG
            //Log($"targetRelationsList.Count = {targetRelationsList.Count}");
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
            //Log($"targetRelation = {targetRelation}");
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
            //Log($"targetRelationVarsInfoList.Count = {targetRelationVarsInfoList.Count}");
            //foreach (var varInfo in targetRelationVarsInfoList)
            //{
            //    Log($"varInfo = {varInfo}");
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
            //Log($"########################targetKnownInfoList.Count = {targetKnownInfoList.Count}");
            //foreach (var tmpKnownInfo in targetKnownInfoList)
            //{
            //    Log($"tmpKnownInfo = {tmpKnownInfo}");
            //}
            //if (targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            LogicalSearchExplainNode relationWithProductionNextPartsCollectorExplainNode = null;

            if (currentExplainNode != null)
            {
                relationWithProductionNextPartsCollectorExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionNextPartsCollector
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, relationWithProductionNextPartsCollectorExplainNode);
            }

            var nextPartsList = processedExpr.GetNextPartsList();

#if DEBUG
            //Log($"nextPartsList.Count = {nextPartsList.Count}");
#endif

            foreach (var nextPart in nextPartsList)
            {
#if DEBUG
                //Log($"nextPart = {DebugHelperForRuleInstance.BaseRulePartToString(nextPart)}");
#endif

                LogicalSearchExplainNode nextPartLocalResultExplainNode = null;

                if (relationWithProductionNextPartsCollectorExplainNode != null)
                {
                    nextPartLocalResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.Result
                    };

                    LogicalSearchExplainNode.LinkNodes(relationWithProductionNextPartsCollectorExplainNode, nextPartLocalResultExplainNode);
                }

                var queryExecutingCardForNextPart = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForNextPart.VarsInfoList = targetRelation.VarsInfoList;
                queryExecutingCardForNextPart.KnownInfoList = targetKnownInfoList;
                queryExecutingCardForNextPart.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                queryExecutingCardForNextPart.RootParentExplainNode = rootParentExplainNode;
                queryExecutingCardForNextPart.ParentExplainNode = nextPartLocalResultExplainNode;

                FillExecutingCardForCallingFromOtherPart(nextPart, queryExecutingCardForNextPart, dataSource, options);

                if(nextPartLocalResultExplainNode != null)
                {
                    FillUpResultToExplainNode(queryExecutingCardForNextPart, nextPartLocalResultExplainNode);
                }

                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForNextPart.UsedKeysList);

#if DEBUG
                //Log($"queryExecutingCardForNextPart = {queryExecutingCardForNextPart}");

                //if (queryExecutingCardForNextPart.UsedKeysList.Any())
                //{
                //    throw new NotImplementedException();
                //}

                //Log($"queryExecutingCard = {queryExecutingCard}");
                //Log($"queryExecutingCardForNextPart.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderExpressionNodeHumanizeDbgString()}");
                //Log($"queryExecutingCardForNextPart.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderIndexedRulePartHumanizeDbgString()}");
                //Log($"queryExecutingCardForNextPart.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCardForNextPart.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
                //Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
                //Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
                //Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
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
                    //Log($"queryExecutingCard = {queryExecutingCard}");
                    //Log($"processedExpr = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
#endif

                    foreach (var varInfo in varsInfoList)
                    {
#if DEBUG
                        //Log($"varInfo = {varInfo}");
#endif

                        var targetInternalKeyOfVar = targetRelationVarsInfoDictByPosition[varInfo.Position];

#if DEBUG
                        //Log($"targetInternalKeyOfVar = {targetInternalKeyOfVar}");
#endif

                        backKeysDict[targetInternalKeyOfVar] = varInfo.NameOfVar;
                    }

                    foreach (var resultOfQueryToRelation in resultsOfQueryToRelationList)
                    {
#if DEBUG
                        //Log($"resultOfQueryToRelation = {resultOfQueryToRelation}");
#endif

                        var newResultOfQueryToRelation = new ResultOfQueryToRelation();
                        var newResultOfVarOfQueryToRelationList = new List<ResultOfVarOfQueryToRelation>();

                        foreach (var resultOfVarOfQueryToRelation in resultOfQueryToRelation.ResultOfVarOfQueryToRelationList)
                        {
#if DEBUG
                            //Log($"resultOfVarOfQueryToRelation = {resultOfVarOfQueryToRelation}");
#endif

                            var internalKeyOfVar = resultOfVarOfQueryToRelation.NameOfVar;

#if DEBUG
                            //Log($"internalKeyOfVar = {internalKeyOfVar}");
#endif

                            if (backKeysDict.ContainsKey(internalKeyOfVar))
                            {
                                var externalKeyOfVar = backKeysDict[internalKeyOfVar];

#if DEBUG
                                //Log($"externalKeyOfVar = {externalKeyOfVar}");
                                //Log($"resultOfVarOfQueryToRelation before = {resultOfVarOfQueryToRelation}");
#endif

                                resultOfVarOfQueryToRelation.NameOfVar = externalKeyOfVar;

#if DEBUG
                                //Log($"resultOfVarOfQueryToRelation after = {resultOfVarOfQueryToRelation}");
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
            //Log($"+++++++++queryExecutingCard = {queryExecutingCard}");
#endif
#if DEBUG
            //Log("End");
#endif
        }

        private void FillExecutingCardForCallingFromOtherPart(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //Log("Begin ^&*^&*");
            //Log($"queryExecutingCard = {queryExecutingCard}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;
            LogicalSearchExplainNode resultExplainNode = null;

            var rootParentExplainNode = queryExecutingCard.RootParentExplainNode;
            var parentExplainNode = queryExecutingCard.ParentExplainNode;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.RelationWithProductionNextPart
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                currentExplainNode.ProcessedBaseRulePart = processedExpr;

                resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.Result
                };

                LogicalSearchExplainNode.LinkNodes(currentExplainNode, resultExplainNode);
            }

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

            queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
            queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            queryExecutingCardForExpression.RootParentExplainNode = rootParentExplainNode;
            queryExecutingCardForExpression.ParentExplainNode = resultExplainNode;

            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(queryExecutingCardForExpression, resultExplainNode);
            }

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

#if DEBUG
            //Log($"%%%%%%%% queryExecutingCardForExpression = {queryExecutingCardForExpression}");
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
            //Log("End  ^&*^&*");
#endif
        }

        private bool EqualityCompare(LogicalQueryNode expressionNode1, LogicalQueryNode expressionNode2, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reason, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
#if DEBUG
            //Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            //Log($"expressionNode1 = {expressionNode1}");
            //Log($"expressionNode2 = {expressionNode2}");
            //Log($"queryExecutingCard = {queryExecutingCard}");
            //Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
            //Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            if (expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar && (expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Entity))
            {
#if DEBUG
                //Log($"%%%%%% expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar && expressionNode2.IsKeyRef");
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
                //Log($"key_1 = {key_1}");
                //Log($"key_2 = {key_2}");
                //if((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                //{
                    //Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
                    //Log($"expressionNode1 = {expressionNode1}");
                    //Log($"expressionNode2 = {expressionNode2}");
                    //Log($"key_1 = {key_1}");
                    //Log($"key_2 = {key_2}");
                    //Log($"queryExecutingCard = {queryExecutingCard}");
                    //Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
                    //Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
                //}
#endif

                if (key_1 == key_2)
                {
#if DEBUG
                    //if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    //{
                    //    //Log($"key_1 == key_2 = {key_1 == key_2}");
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
                    //    //Log("additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2)");
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
                    //    //Log("additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1)");
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

                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                var synonymsList1 = _synonymsResolver.GetSynonyms(key_1, localCodeExecutionContext);

#if DEBUG
                //Log($"synonymsList1 = {synonymsList1.WriteListToString()}");
#endif

                if(synonymsList1.Contains(key_2))
                {
                    return true;
                }

                var synonymsList2 = _synonymsResolver.GetSynonyms(key_2, localCodeExecutionContext);

#if DEBUG
                //Log($"synonymsList2 = {synonymsList2.WriteListToString()}");
#endif

                if (synonymsList2.Contains(key_1))
                {
                    return true;
                }

                if (synonymsList1.Intersect(synonymsList2).Any())
                {
                    return true;
                }

                var fuzzyResult = _fuzzyLogicResolver.Equals(key_1, key_2, localCodeExecutionContext);

#if DEBUG
                //Log($"fuzzyResult = {fuzzyResult}");
#endif

                return fuzzyResult;
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Value && expressionNode2.Kind == KindOfLogicalQueryNode.Value)
            {
                var sysValue1 = expressionNode1.Value.GetSystemValue();
                var sysValue2 = expressionNode2.Value.GetSystemValue();

#if DEBUG
                //Log($"sysValue1 = {sysValue1}");
                //Log($"sysValue1?.GetType().FullName = {sysValue1?.GetType().FullName}");
                //Log($"sysValue2 = {sysValue2}");
                //Log($"sysValue2?.GetType().FullName = {sysValue2?.GetType().FullName}");
                //Log($"sysValue1.Equals(sysValue2) = {sysValue1.Equals(sysValue2)}");
#endif

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            if ((expressionNode1.IsKeyRef && expressionNode2.Kind == KindOfLogicalQueryNode.Value && (expressionNode2.Value.IsLogicalValue || expressionNode2.Value.IsNumberValue)) || (expressionNode2.IsKeyRef && expressionNode1.Kind == KindOfLogicalQueryNode.Value && (expressionNode1.Value.IsNumberValue || expressionNode1.Value.IsLogicalValue)))
            {
#if DEBUG
                //Log("Try to check fuzzy logic!");
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
                //Log($"conceptNode = {conceptNode}");
                //Log($"valueNode = {valueNode}");
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
                //Log("Try to check fuzzy logic!");
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
                //Log($"sequenceNode = {sequenceNode}");
                //Log($"valueNode = {valueNode}");
#endif

                var value = valueNode.Value;
                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                if (value.GetSystemValue() == null || !_numberValueLinearResolver.CanBeResolved(value))
                {
                    return false;
                }

                return _fuzzyLogicResolver.Equals(sequenceNode.FuzzyLogicNonNumericSequenceValue, _numberValueLinearResolver.Resolve(value, localCodeExecutionContext), reason, localCodeExecutionContext);
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Relation && expressionNode2.Kind == KindOfLogicalQueryNode.Relation)
            {
#if DEBUG
                //Log("Try to compare relations!");
#endif

                if (expressionNode1.Name != expressionNode2.Name)
                {
                    return false;
                }

                var paramsList1 = expressionNode1.ParamsList;
                var paramsList2 = expressionNode2.ParamsList;

                if (paramsList1.Count != paramsList2.Count)
                {
                    return false;
                }

                var paramsList2Enumerator = paramsList2.GetEnumerator();

                foreach (var param1 in paramsList1)
                {
                    if (!paramsList2Enumerator.MoveNext())
                    {
                        return false;
                    }

                    var param2 = paramsList2Enumerator.Current;

#if DEBUG
                    //Log($"param1 = {param1}");
                    //Log($"param2 = {param2}");
                    //Log($"EqualityCompare(param1, param2, null, null, reason, options) = {EqualityCompare(param1, param2, null, null, reason, options, queryExecutingCard)}");
                    //Log($"?????????????????>>>>>>>>>>queryExecutingCard (after) = {queryExecutingCard}");
#endif

                    if (!EqualityCompare(param1, param2, null, null, reason, options, queryExecutingCard, dataSource))
                    {
                        return false;
                    }
                }

                return true;
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.Group && expressionNode2.Kind != KindOfLogicalQueryNode.Group) || (expressionNode2.Kind == KindOfLogicalQueryNode.Group && expressionNode1.Kind != KindOfLogicalQueryNode.Group))
            {
                return false;
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.Relation && expressionNode2.Kind != KindOfLogicalQueryNode.Relation) || (expressionNode2.Kind == KindOfLogicalQueryNode.Relation && expressionNode1.Kind != KindOfLogicalQueryNode.Relation))
            {
                return false;
            }

            Log($"expressionNode1 = {expressionNode1}");
            Log($"expressionNode2 = {expressionNode2}");

            throw new NotImplementedException();
        }
    }
}
