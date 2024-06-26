/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Visitors;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly VarsResolver _varsResolver;
        private readonly SynonymsResolver _synonymsResolver;
        private readonly LogicalSearchVarResultsItemInvertor _logicalSearchVarResultsItemInvertor;

        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
            _logicalSearchVarResultsItemInvertor = dataResolversFactory.GetLogicalSearchVarResultsItemInvertor();
        }

        public bool IsTruth(IMonitorLogger logger, LogicalSearchOptions options)
        {
            var result = Run(logger, options);

            return result.IsSuccess;
        }

        public LogicalSearchResult Run(IMonitorLogger logger, LogicalSearchOptions options)
        {
            var result = new LogicalSearchResult();

            ConsolidatedDataSource dataSource = null;
            List<StorageUsingOptions> storagesList = null;

            if (options.TargetStorage == null)
            {
                storagesList = GetStoragesList(logger, options.LocalCodeExecutionContext.Storage);
                dataSource = new ConsolidatedDataSource(storagesList);
            }
            else
            {
                var targetStorageList = GetStoragesList(logger, options.TargetStorage);

                foreach(var targetStorage in targetStorageList)
                {
                    targetStorage.UseFacts = true;
                    targetStorage.UseInheritanceFacts = true;
                }

                var maxPriority = targetStorageList.Max(p => p.Priority);

                var collectChainOfStoragesOptions = new CollectChainOfStoragesOptions();
                collectChainOfStoragesOptions.InitialPriority = maxPriority;
                collectChainOfStoragesOptions.UseFacts = false;

                var additinalStoragesList = GetStoragesList(logger, options.LocalCodeExecutionContext.Storage, collectChainOfStoragesOptions);

                targetStorageList.AddRange(additinalStoragesList);

                storagesList = targetStorageList;

                dataSource = new ConsolidatedDataSource(targetStorageList);
            }

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            queryExpression.CheckDirty();

            if (queryExpression.IsParameterized)
            {
                queryExpression = queryExpression.Clone();

                var packedVarsResolver = new PackedVarsResolver(_varsResolver, options.LocalCodeExecutionContext);

                var resolveVariablesLogicalVisitor = new ResolveVariablesLogicalVisitor(logger);
                resolveVariablesLogicalVisitor.Run(queryExpression, packedVarsResolver);

                queryExpression.CheckDirty();

                if (options.IgnoreIfNullValueInImperativeVariables)
                {
                    var containsNullValueLogicalVisitor = new ContainsNullValueLogicalVisitor(logger);

                    if(containsNullValueLogicalVisitor.Run(queryExpression))
                    {
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }

            queryExpression = queryExpression.Normalized;

#if DEBUG
            //Log($"queryExpression = {queryExpression.ToHumanizedString()}");
#endif

            var kindOfLogicalSearchExplain = logger.KindOfLogicalSearchExplain;

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
                optionsOfFillExecutingCard.ReplacingNotResultsStrategy = options.ReplacingNotResultsStrategy;

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

                FillExecutingCard(logger, queryExpression, queryExecutingCard, dataSource, optionsOfFillExecutingCard);

                if (queryExecutingCard.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                if (queryExecutingCard.PostFiltersList.Any())
                {
                    throw new NotImplementedException();
                }

                var usedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

                result.UsedKeysList = usedKeysList;

                if (queryExecutingCard.IsNegative)
                {
                    var resolvingNotResultsStrategy = options.ResolvingNotResultsStrategy;

                    switch(resolvingNotResultsStrategy)
                    {
                        case ResolvingNotResultsStrategy.NotSupport:
                            if(queryExecutingCard.ResultsOfQueryToRelationList.Any())
                            {
                                throw new NotSupportedException();
                            }

                            result.IsSuccess = queryExecutingCard.IsSuccess;
                            result.Items = new List<LogicalSearchResultItem>();
                            break;

                        case ResolvingNotResultsStrategy.Ignore:
                            result.IsSuccess = queryExecutingCard.IsSuccess;
                            result.Items = new List<LogicalSearchResultItem>();
                            break;

                        case ResolvingNotResultsStrategy.InResolver:
                            {
                                var newItems = _logicalSearchVarResultsItemInvertor.Invert<LogicalSearchResultItem>(logger, queryExecutingCard.ResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), storagesList, options.ReplacingNotResultsStrategy);

                                result.IsSuccess = queryExecutingCard.IsSuccess;
                                result.Items = newItems;
                            }
                            break;

                        case ResolvingNotResultsStrategy.InConsumer:
                            result.IsSuccess = queryExecutingCard.IsSuccess;
                            result.IsNegative = true;
                            var resultItemsList = new List<LogicalSearchResultItem>();

                            foreach (var resultOfQueryToRelation in queryExecutingCard.ResultsOfQueryToRelationList)
                            {
                                var resultItem = new LogicalSearchResultItem();
                                resultItem.ResultOfVarOfQueryToRelationList = resultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                                resultItemsList.Add(resultItem);
                            }

                            result.Items = resultItemsList;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(resolvingNotResultsStrategy), resolvingNotResultsStrategy, null);
                    }
                }
                else
                {
                    result.IsSuccess = queryExecutingCard.IsSuccess;

                    var resultItemsList = new List<LogicalSearchResultItem>();

                    foreach (var resultOfQueryToRelation in queryExecutingCard.ResultsOfQueryToRelationList)
                    {
                        var resultItem = new LogicalSearchResultItem();
                        resultItem.ResultOfVarOfQueryToRelationList = resultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                        resultItemsList.Add(resultItem);
                    }

                    result.Items = resultItemsList;
                }

                if (resultExplainNode != null)
                {
                    FillUpResultToExplainNode(logger, queryExecutingCard, resultExplainNode);
                }

                if(kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpAlways)
                {
                    LogLogicalSearchExplain(logger, "43E6CC35-BFAD-441B-8C50-7A9E027EB544", rootExplainNode, queryExpression);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                logger.Info("8E45F313-D548-4D34-BC1F-249525BC1D26", $"e = {e}");
#endif

                var sb = new StringBuilder();
                sb.AppendLine($"Error in query: {queryExpression.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

                if (kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpIfError || kindOfLogicalSearchExplain == KindOfLogicalSearchExplain.DumpAlways)
                {
                    var messageNumber = LogLogicalSearchExplain(logger, "A8F977DE-F56A-45E5-B422-49FB0C1AEC44", rootExplainNode, queryExpression);

                    sb.AppendLine($"The explanation has been dumped into message msg::ref({messageNumber}).");
                }

                logger.Error("01C3750D-32DB-4563-840D-3A865B22304D", sb.ToString());

                logger.Error("F5F36104-7980-4589-9BEF-3C4D93433BCA", e);

                throw;
            }

            return result;
        }

        private ulong LogLogicalSearchExplain(IMonitorLogger logger, string messagePointId, LogicalSearchExplainNode explainNode, RuleInstance queryExpression)
        {
            var dotStr = DebugHelperForLogicalSearchExplainNode.ToDot(explainNode);

            return logger.LogicalSearchExplain(messagePointId, dotStr, queryExpression.ToLabel(logger));
        }

        private void AppendResults(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, bool setIsSuccessIfTrue = false)
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

            CopyResultsOfQueryToRelationList(logger, sourceQueryExecutingCard, destQueryExecutingCard);

            destQueryExecutingCard.UsedKeysList.AddRange(sourceQueryExecutingCard.UsedKeysList);
        }

        private void CopyResultsOfQueryToRelationList(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard)
        {
            var destList = destQueryExecutingCard.ResultsOfQueryToRelationList;

            foreach (var resultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
            {
                destList.Add(resultOfQueryToRelation);
            }
        }

        private void FillUpResultToExplainNode(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, LogicalSearchExplainNode resultExplainNode)
        {
            resultExplainNode.IsSuccess = queryExecutingCard.IsSuccess;
            resultExplainNode.ResultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;
        }

        private void FillExecutingCard(IMonitorLogger logger, RuleInstance processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            FillExecutingCard(logger, processedExpr.PrimaryPart, queryExecutingCardForPart_1, dataSource, options);

            if(queryExecutingCardForPart_1.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (queryExecutingCardForPart_1.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

            AppendResults(logger, queryExecutingCardForPart_1, queryExecutingCard);
            queryExecutingCard.IsNegative = queryExecutingCardForPart_1.IsNegative;

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, queryExecutingCard, resultExplainNode);
            }

        }

        private void FillExecutingCard(IMonitorLogger logger, PrimaryRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            FillExecutingCard(logger, processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, queryExecutingCardForExpression, resultExplainNode);
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

            if (queryExecutingCardForExpression.PostFiltersList.Any())
            {
                if (queryExecutingCardForExpression.IsNegative)
                {
                    throw new NotImplementedException();
                }

                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.RootParentExplainNode = rootParentExplainNode;
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.ParentExplainNode = parentExplainNode;

                var postFilterNode = FillExecutingCardUsingPostFiltersList(logger, queryExecutingCardForExpression, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

                AppendResults(logger, queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);

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

                AppendResults(logger, queryExecutingCardForExpression, queryExecutingCard);
                queryExecutingCard.IsNegative = queryExecutingCardForExpression.IsNegative;
            }

        }

        private LogicalSearchExplainNode FillExecutingCardUsingPostFiltersList(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, OptionsOfFillExecutingCard options, ConsolidatedDataSource dataSource)
        {
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

                var oldTargetSourceQueryExecutingCard = targetSourceQueryExecutingCard;

                targetSourceQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
                targetSourceQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                switch (kindOfBinaryOperator)
                {
                    case KindOfOperatorOfLogicalQueryNode.And:
                        {
                            var explainNode = FillExecutingCardUsingPostFilterListWithAndStrategy(logger, oldTargetSourceQueryExecutingCard, targetSourceQueryExecutingCard, postFilter, options, dataSource);
                            if(parentExplainNode != null)
                            {
                                var resultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.Result
                                };

                                FillUpResultToExplainNode(logger, targetSourceQueryExecutingCard, resultExplainNode);

                                explainNodesList.Add((resultExplainNode, explainNode));
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfBinaryOperator), kindOfBinaryOperator, null);
                }
            }

            if (targetSourceQueryExecutingCard.IsSuccess)
            {
                AppendResults(logger, targetSourceQueryExecutingCard, destQueryExecutingCard);
            }

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

        private LogicalSearchExplainNode FillExecutingCardUsingPostFilterListWithAndStrategy(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, PostFilterOfQueryExecutingCardForPersistLogicalData postFilter, OptionsOfFillExecutingCard options, ConsolidatedDataSource dataSource)
        {
            var processedExpr = postFilter.ProcessedExpr;

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

            var resultsOfQueryToRelationList = new List<ResultOfQueryToRelation>();
            var usedKeysList = new List<StrongIdentifierValue>();

            if (sourceQueryExecutingCard.IsNegative)
            {
                throw new NotImplementedException();
            }

            if (leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar && rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var leftVariableName = leftExpr.Name;
                var rightVariableName = rightExpr.Name;

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

                    if (sourceVarsDict.ContainsKey(leftVariableName) && sourceVarsDict.ContainsKey(rightVariableName))
                    {
                        var sourceLeftNode = sourceVarsDict[leftVariableName];
                        var sourceRightNode = sourceVarsDict[rightVariableName];

                        var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
                        comparisonQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

                        var resultOfComparison = CompareForPostFilter(logger, kindOfOperator, sourceLeftNode, sourceRightNode, options, comparisonQueryExecutingCard, dataSource);

                        if (resultOfComparison)
                        {
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

                foreach (var sourceResultOfQueryToRelation in sourceQueryExecutingCard.ResultsOfQueryToRelationList)
                {
                    var sourceVarsList = sourceResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var sourceVarsDict = sourceVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                    var sourceVarNamesList = sourceVarsList.Select(p => p.NameOfVar);

                    if (sourceVarsDict.ContainsKey(variableOfFilter))
                    {
                        var sourceNode = sourceVarsDict[variableOfFilter];

                        var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                        bool resultOfComparison;

                        if (isLeftRight)
                        {
                            resultOfComparison = CompareForPostFilter(logger, kindOfOperator, sourceNode, nodeOfFilter, options, comparisonQueryExecutingCard, dataSource);
                        }
                        else
                        {
                            resultOfComparison = CompareForPostFilter(logger, kindOfOperator, nodeOfFilter, sourceNode, options, comparisonQueryExecutingCard, dataSource);
                        }

                        if (resultOfComparison)
                        {
                            resultsOfQueryToRelationList.Add(sourceResultOfQueryToRelation);

                            usedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                        }
                    }
                }
            }

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

            return currentExplainNode;
        }

        private bool CompareForPostFilter(IMonitorLogger logger, KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            switch (kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Is:
                    return CompareForPostFilterByOperatorIs(logger, leftNode, rightNode, options, queryExecutingCard, dataSource);

                case KindOfOperatorOfLogicalQueryNode.IsNot:
                    return !CompareForPostFilterByOperatorIs(logger, leftNode, rightNode, options, queryExecutingCard, dataSource);

                case KindOfOperatorOfLogicalQueryNode.More:
                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                case KindOfOperatorOfLogicalQueryNode.Less:
                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    return CompareForPostFilterByOperatorsMoreOrLess(logger, kindOfOperator, leftNode, rightNode, options, queryExecutingCard);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }
        }

        private bool CompareForPostFilterByOperatorIs(IMonitorLogger logger, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
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
                        additionalKeys_1 = inheritanceResolver.GetSuperClassesKeysList(logger, leftNode.Name, options.LocalCodeExecutionContext);
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
                        additionalKeys_2 = inheritanceResolver.GetSuperClassesKeysList(logger, rightNode.Name, options.LocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfNodeOfFilter), kindOfNodeOfFilter, null);
                }
            }

            return EqualityCompare(logger, leftNode, rightNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource);
        }

        private bool CompareForPostFilterByOperatorsMoreOrLess(IMonitorLogger logger, KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
            var localCodeExecutionContext = options.LocalCodeExecutionContext;
            var numberValueLinearResolver = _numberValueLinearResolver;

            if (leftNode.Kind == KindOfLogicalQueryNode.Value && rightNode.Kind == KindOfLogicalQueryNode.Value)
            {
                var leftValue = leftNode.Value;
                var rightValue = rightNode.Value;

                if(numberValueLinearResolver.CanBeResolved(logger, leftValue) && numberValueLinearResolver.CanBeResolved(logger, rightValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(logger, leftValue, localCodeExecutionContext);
                    var rightNumberValue = numberValueLinearResolver.Resolve(logger, rightValue, localCodeExecutionContext);

                    var leftSystemNullaleValue = leftNumberValue.SystemValue;
                    var rightSystemNullaleValue = rightNumberValue.SystemValue;

                    if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                    {
                        return CompareSystemValues(logger, kindOfOperator, leftSystemNullaleValue.Value, rightSystemNullaleValue.Value, options);
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

                if(numberValueLinearResolver.CanBeResolved(logger, rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(logger, rightValue, localCodeExecutionContext);

                    if (!rightNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftName, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftName, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftName, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftName, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

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
                if (numberValueLinearResolver.CanBeResolved(logger, rightValue))
                {
                    var rightNumberValue = numberValueLinearResolver.Resolve(logger, rightValue, localCodeExecutionContext);

                    if (!rightNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftSequence, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value > rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftSequence, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value >= rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftSequence, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return systemDeffuzzificatedValue.Value < rightNumberValue.SystemValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, leftSequence, rightNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, leftSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

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

                if (numberValueLinearResolver.CanBeResolved(logger, leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(logger, leftValue, localCodeExecutionContext);

                    if (!leftNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightName, leftNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if(!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightName, leftNumberValue, localCodeExecutionContext);

                                if(eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightName, leftNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightName, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightName, leftNumberValue, localCodeExecutionContext);

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightName, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

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

                if (numberValueLinearResolver.CanBeResolved(logger, leftValue))
                {
                    var leftNumberValue = numberValueLinearResolver.Resolve(logger, leftValue, localCodeExecutionContext);

                    if (!leftNumberValue.SystemValue.HasValue)
                    {
                        return false;
                    }

                    var fuzzyLogicResolver = _fuzzyLogicResolver;

                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.More:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightSequence, leftNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value > systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightSequence, leftNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value >= systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightSequence, leftNumberValue, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return false;
                                }

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightSequence, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

                                return leftNumberValue.SystemValue.Value < systemDeffuzzificatedValue.Value;
                            }

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            {
                                var eqResult = fuzzyLogicResolver.Equals(logger, rightSequence, leftNumberValue, localCodeExecutionContext);

                                var deffuzzificatedValue = fuzzyLogicResolver.Resolve(logger, rightSequence, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return true;
                                }

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return false;
                                }

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

        private bool CompareSystemValues(IMonitorLogger logger, KindOfOperatorOfLogicalQueryNode kindOfOperator, double left, double right, OptionsOfFillExecutingCard options)
        {
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

        private void FillExecutingCard(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var kind = processedExpr.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    FillExecutingCardForRelationLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(processedExpr.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                            FillExecutingCardForAndOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.Or:
                            FillExecutingCardForOrOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.Is:
                            FillExecutingCardForIsOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                            FillExecutingCardForIsNotOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.More:
                            FillExecutingCardForMoreOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                            FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.Less:
                            FillExecutingCardForLessOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(processedExpr.KindOfOperator), processedExpr.KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch(processedExpr.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            FillExecutingCardForNotOperatorLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(processedExpr.KindOfOperator), processedExpr.KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    FillExecutingCardForGroupLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

        }

        private void FillExecutingCardForRelationLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            if (processedExpr.IsQuestion)
            {
                FillExecutingCardForQuestion(logger, processedExpr, queryExecutingCard, dataSource, options);
                return;
            }

            NFillExecutingCardForRelationLogicalQueryNode(logger, processedExpr, queryExecutingCard, dataSource, options);

#if DEBUG

#endif
        }

        private void NFillExecutingCardForRelationLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var relationName = processedExpr.Name;

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
            queryExecutingCardForExpression.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

            NFillExecutingCardForConcreteRelationLogicalQueryNode(logger, processedExpr, queryExecutingCardForExpression, dataSource, options);

            if (queryExecutingCardForExpression.IsNegative)
            {
                throw new NotImplementedException();
            }

            if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }
            if (queryExecutingCardForExpression.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            AppendResults(logger, queryExecutingCardForExpression, queryExecutingCard, true);

            var synonymsList = _synonymsResolver.GetSynonyms(logger, relationName, dataSource.StoragesList);

            foreach (var synonym in synonymsList)
            {
                queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForExpression.RootParentExplainNode = queryExecutingCard.RootParentExplainNode;
                queryExecutingCardForExpression.ParentExplainNode = queryExecutingCard.ParentExplainNode;
                queryExecutingCardForExpression.VarsInfoList = queryExecutingCard.VarsInfoList;
                queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
                queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                queryExecutingCardForExpression.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

                var newRelation = processedExpr.Clone();
                newRelation.Name = synonym;

                NFillExecutingCardForConcreteRelationLogicalQueryNode(logger, newRelation, queryExecutingCardForExpression, dataSource, options);

                if (queryExecutingCardForExpression.IsNegative)
                {
                    throw new NotImplementedException();
                }

                if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
                {
                    throw new NotImplementedException();
                }
                if (queryExecutingCardForExpression.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                AppendResults(logger, queryExecutingCardForExpression, queryExecutingCard, true);
            }

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(logger, relationName, options.LocalCodeExecutionContext);

            foreach (var superClass in superClassesList)
            {
                queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForExpression.RootParentExplainNode = queryExecutingCard.RootParentExplainNode;
                queryExecutingCardForExpression.ParentExplainNode = queryExecutingCard.ParentExplainNode;
                queryExecutingCardForExpression.VarsInfoList = queryExecutingCard.VarsInfoList;
                queryExecutingCardForExpression.KnownInfoList = queryExecutingCard.KnownInfoList;
                queryExecutingCardForExpression.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                queryExecutingCardForExpression.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

                var newRelation = processedExpr.Clone();
                newRelation.Name = superClass;

                NFillExecutingCardForConcreteRelationLogicalQueryNode(logger, newRelation, queryExecutingCardForExpression, dataSource, options);

                if (queryExecutingCardForExpression.IsNegative)
                {
                    throw new NotImplementedException();
                }

                if (!queryExecutingCardForExpression.PostFiltersList.IsNullOrEmpty())
                {
                    throw new NotImplementedException();
                }

                if (queryExecutingCardForExpression.IsPostFiltersListOnly)
                {
                    throw new NotImplementedException();
                }

                AppendResults(logger, queryExecutingCardForExpression, queryExecutingCard, true);
            }

        }

        private void NFillExecutingCardForConcreteRelationLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            if (queryExecutingCard.UsedRelations.Contains(processedExpr))
            {
                return;
            }

#if DEBUG
            //Log($"processedExpr.Name = {processedExpr.Name}");
#endif

            queryExecutingCard.UsedRelations.Add(processedExpr);

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

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(logger, processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetRelationName = processedExpr.Name;

            var targetKnownInfoList = mergingResult.KnownInfoList;

            var rulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(logger, targetRelationName, options.LogicalSearchStorageContext, directFactsDataSourceResultExplainNode, rootParentExplainNode);

            if(directFactsDataSourceResultExplainNode != null)
            {
                directFactsDataSourceResultExplainNode.BaseRulePartList = rulePartsOfFactsList;
            }

            queryExecutingCard.UsedKeysList.Add(targetRelationName);            

            if (!rulePartsOfFactsList.IsNullOrEmpty())
            {
                foreach (var rulePartsOfFacts in rulePartsOfFactsList)
                {
#if DEBUG
                    //Log($"rulePartsOfFacts = {rulePartsOfFacts.ToHumanizedString()}");
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
                    queryExecutingCardForTargetFact.TargetRelationName = targetRelationName;
                    queryExecutingCardForTargetFact.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetFact.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetFact.KnownInfoList = targetKnownInfoList;
                    queryExecutingCardForTargetFact.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                    queryExecutingCardForTargetFact.RootParentExplainNode = rootParentExplainNode;
                    queryExecutingCardForTargetFact.ParentExplainNode = localResultExplainNode;

                    FillExecutingCardForCallingFromRelationForFact(logger, rulePartsOfFacts, queryExecutingCardForTargetFact, dataSource, options);

                    if (queryExecutingCardForTargetFact.IsSuccess)
                    {
                        if (queryExecutingCardForTargetFact.IsNegative)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetFact.IsPostFiltersListOnly)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetFact.PostFiltersList.Any())
                        {
                            throw new NotImplementedException();
                        }

                        AppendResults(logger, queryExecutingCardForTargetFact, queryExecutingCard);
                    }

                    if (localResultExplainNode != null)
                    {
                        FillUpResultToExplainNode(logger, queryExecutingCardForTargetFact, localResultExplainNode);
                    }
                }
            }

            var rulePartWithOneRelationsList = dataSource.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, processedExpr.Name, options.LogicalSearchStorageContext, productionDataSourceResultExplainNode, rootParentExplainNode);

            if(productionDataSourceResultExplainNode != null)
            {
                productionDataSourceResultExplainNode.BaseRulePartList = rulePartWithOneRelationsList;
            }

#if DEBUG
            //Log($"targetKnownInfoList = {targetKnownInfoList.WriteListToString()}");
#endif

            if (!rulePartWithOneRelationsList.IsNullOrEmpty())
            {
                foreach (var indexedRulePartsOfRule in rulePartWithOneRelationsList)
                {
#if DEBUG
                    //Log($"indexedRulePartsOfRule = {indexedRulePartsOfRule.ToHumanizedString()}");
                    //Log($"indexedRulePartsOfRule.Parent = {indexedRulePartsOfRule.Parent?.ToHumanizedString()}");
#endif

                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (productionResultsCollectorExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                        {
                            Kind = KindOfLogicalSearchExplainNode.Result
                        };

                        LogicalSearchExplainNode.LinkNodes(productionResultsCollectorExplainNode, localResultExplainNode);
                    }

                    var queryExecutingCardForTargetRule = new QueryExecutingCardForIndexedPersistLogicalData();
                    queryExecutingCardForTargetRule.TargetRelationName = targetRelationName;

                    queryExecutingCardForTargetRule.CountParams = processedExpr.CountParams;
                    queryExecutingCardForTargetRule.VarsInfoList = processedExpr.VarsInfoList;
                    queryExecutingCardForTargetRule.KnownInfoList = targetKnownInfoList;
                    queryExecutingCardForTargetRule.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
                    queryExecutingCardForTargetRule.RootParentExplainNode = rootParentExplainNode;
                    queryExecutingCardForTargetRule.ParentExplainNode = localResultExplainNode;
                    queryExecutingCardForTargetRule.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

                    FillExecutingCardForCallingFromRelationForProduction(logger, indexedRulePartsOfRule, queryExecutingCardForTargetRule, dataSource, options);

                    if (queryExecutingCardForTargetRule.IsSuccess)
                    {
                        if (queryExecutingCardForTargetRule.IsNegative)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetRule.IsPostFiltersListOnly)
                        {
                            throw new NotImplementedException();
                        }

                        if (queryExecutingCardForTargetRule.PostFiltersList.Any())
                        {
                            throw new NotImplementedException();
                        }

                        AppendResults(logger, queryExecutingCardForTargetRule, queryExecutingCard);
                    }

                    if (localResultExplainNode != null)
                    {
                        FillUpResultToExplainNode(logger, queryExecutingCardForTargetRule, localResultExplainNode);
                    }

                }
            }

        }

        private void FillExecutingCardForQuestion(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(logger, processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

            var targetKnownInfoList = mergingResult.KnownInfoList;

            var hasAnnotations = !processedExpr.Annotations.IsNullOrEmpty();

            var targetRelationsList = dataSource.AllRelationsForProductions(logger, options.LogicalSearchStorageContext, dataSourceResultExplainNode, rootParentExplainNode);

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
                    var position = knownInfo.Position;

                    if (position.HasValue)
                    {
                        var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

                        if (paramOfTargetRelation.Kind == KindOfLogicalQueryNode.LogicalVar && kindOfParentRuleInstance != KindOfRuleInstance.Fact)
                        {
                            isFit = false;
                            break;
                        }

                        var resultOfComparison = CompareKnownInfoAndParamOfTargetRelation(logger, knownInfo, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard, dataSource);

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

                    }

                }
            }

        }

        private void CopyPostFilters(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, KindOfOperatorOfLogicalQueryNode kindOfBinaryOperator)
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

        private void FillExecutingCardForAndOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            FillExecutingCard(logger, processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (leftResultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, leftQueryExecutingCard, leftResultExplainNode);
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

            FillExecutingCard(logger, processedExpr.Right, rightQueryExecutingCard, dataSource, options);

            if (rightResultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, rightQueryExecutingCard, rightResultExplainNode);
            }

            if (!rightQueryExecutingCard.IsSuccess)
            {
                if(rightResultExplainNode != null)
                {
                    rightResultExplainNode.AdditionalInformation.Add("!rightQueryExecutingCard.IsSuccess");
                }

                return;
            }

            var leftQueryExecutingCardIsNegative = leftQueryExecutingCard.IsNegative;
            var rightQueryExecutingCardIsNegative = rightQueryExecutingCard.IsNegative;

            if(leftQueryExecutingCardIsNegative && rightQueryExecutingCardIsNegative)
            {
                MergeExecutingCardForOrOperatorLogicalQueryNode(logger, leftQueryExecutingCard, rightQueryExecutingCard, queryExecutingCard, currentExplainNode, dataSource, options);
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

                CopyPostFilters(logger, leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(logger, rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                CopyPostFilters(logger, leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(logger, rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

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

                CopyPostFilters(logger, leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(logger, rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

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

                CopyPostFilters(logger, leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(logger, rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;

                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var isFit = true;

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
                        var leftVarsDictContainsKey = leftVarsDict.ContainsKey(varName);
                        var rightVarsDictContainsKey = rightVarsDict.ContainsKey(varName);

                        if (leftVarsDictContainsKey && rightVarsDictContainsKey)
                        {
                            var leftVal = leftVarsDict[varName];
                            var rightVal = rightVarsDict[varName];

                            var resultOfComparison = EqualityCompare(logger, leftVal, rightVal, null, null, null, options, null, dataSource);

                            if (leftQueryExecutingCardIsNegative || rightQueryExecutingCardIsNegative)
                            {
                                resultOfComparison = !resultOfComparison;
                            }

                            if (resultOfComparison)
                            {
                                if(leftVal.Kind == KindOfLogicalQueryNode.Relation && rightVal.Kind != KindOfLogicalQueryNode.Relation)
                                {
                                    if(leftQueryExecutingCardIsNegative || rightQueryExecutingCardIsNegative)
                                    {
                                        throw new NotImplementedException();
                                    }
                                    else
                                    {
                                        varValuesList.Add((varName, rightVal));
                                    }
                                }
                                else
                                {
                                    if(leftQueryExecutingCardIsNegative || rightQueryExecutingCardIsNegative)
                                    {
                                        if(leftQueryExecutingCardIsNegative)
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
                                        varValuesList.Add((varName, leftVal));
                                    }
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

                    if (!isFit)
                    {
                        continue;
                    }

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

                    var varNamesListEnumerator = varNamesList.GetEnumerator();

                    if (varNamesListEnumerator.MoveNext())
                    {
                        var resultVarValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                        BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(logger, varNamesListEnumerator, varValuesDict, resultVarValuesList, resultsOfQueryToRelationList, options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (resultsOfQueryToRelationList.Any())
            {
                CopyPostFilters(logger, leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(logger, rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
            }
            else
            {
                queryExecutingCard.IsSuccess = false;
            }

        }

        private void FillExecutingCardForOrOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            FillExecutingCard(logger, processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (leftResultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, leftQueryExecutingCard, leftResultExplainNode);
            }

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            rightQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            rightQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;
            rightQueryExecutingCard.ParentExplainNode = rightResultExplainNode;

            FillExecutingCard(logger, processedExpr.Right, rightQueryExecutingCard, dataSource, options);

            if (rightResultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, rightQueryExecutingCard, rightResultExplainNode);
            }

            MergeExecutingCardForOrOperatorLogicalQueryNode(logger, leftQueryExecutingCard, rightQueryExecutingCard, queryExecutingCard, currentExplainNode, dataSource, options);
        }

        private void MergeExecutingCardForOrOperatorLogicalQueryNode(IMonitorLogger logger, QueryExecutingCardForIndexedPersistLogicalData leftQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData rightQueryExecutingCard, 
            QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, LogicalSearchExplainNode currentExplainNode, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            if (!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCard.IsSuccess && !rightQueryExecutingCard.IsSuccess");
                }

                return;
            }

            var leftQueryExecutingCardIsNegative = leftQueryExecutingCard.IsNegative;
            var rightQueryExecutingCardIsNegative = rightQueryExecutingCard.IsNegative;

            var setIsNegativeToResult = false;

            if(leftQueryExecutingCardIsNegative && leftQueryExecutingCardIsNegative)
            {
                leftQueryExecutingCardIsNegative = false;
                rightQueryExecutingCardIsNegative = false;
                setIsNegativeToResult = true;
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
                if (leftQueryExecutingCardIsNegative)
                {
                    leftQueryExecutingCardIsNegative = false;
                    leftQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, leftQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
                }

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

                if(setIsNegativeToResult)
                {
                    queryExecutingCard.IsNegative = true;
                }

                return;
            }

            if (!leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess)
            {
                if (rightQueryExecutingCardIsNegative)
                {
                    rightQueryExecutingCardIsNegative = false;
                    rightQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, rightQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
                }

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

                if (setIsNegativeToResult)
                {
                    queryExecutingCard.IsNegative = true;
                }

                return;
            }

            queryExecutingCard.IsSuccess = true;

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList.Concat(rightQueryExecutingCard.UsedKeysList));

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            if (leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (leftQueryExecutingCardIsNegative)
                {
                    leftQueryExecutingCardIsNegative = false;
                    leftQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, leftQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
                }

                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                if (setIsNegativeToResult)
                {
                    queryExecutingCard.IsNegative = true;
                }

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                if (rightQueryExecutingCardIsNegative)
                {
                    rightQueryExecutingCardIsNegative = false;
                    rightQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, rightQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
                }

                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add("!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && rightQueryExecutingCardResultsOfQueryToRelationList.Any()");
                }

                foreach (var resultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }

                if (setIsNegativeToResult)
                {
                    queryExecutingCard.IsNegative = true;
                }

                return;
            }

            if (leftQueryExecutingCardIsNegative)
            {
                leftQueryExecutingCardIsNegative = false;
                leftQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, leftQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
            }

            if (rightQueryExecutingCardIsNegative)
            {
                rightQueryExecutingCardIsNegative = false;
                rightQueryExecutingCardResultsOfQueryToRelationList = _logicalSearchVarResultsItemInvertor.Invert<ResultOfQueryToRelation>(logger, rightQueryExecutingCardResultsOfQueryToRelationList.Cast<IResultOfQueryToRelation>(), dataSource.StoragesList, options.ReplacingNotResultsStrategy).ToList();
            }

            foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
            {
                var leftVarsList = leftResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                var leftVarsDict = leftVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);
                var leftVarNamesList = leftVarsList.Select(p => p.NameOfVar);

                foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                {
                    var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                    var rightVarsDict = rightVarsList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                    var varNamesList = rightVarsList.Select(p => p.NameOfVar).Concat(leftVarNamesList).Distinct();

                    var varValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                    foreach (var varName in varNamesList)
                    {
                        var resultOfVarOfQueryToRelationItem = new ResultOfVarOfQueryToRelation();
                        resultOfVarOfQueryToRelationItem.NameOfVar = varName;

                        var leftVarsDictContainsKey = leftVarsDict.ContainsKey(varName);
                        var rightVarsDictContainsKey = rightVarsDict.ContainsKey(varName);

                        if (leftVarsDictContainsKey && rightVarsDictContainsKey)
                        {
                            var leftVal = leftVarsDict[varName];
                            var rightVal = rightVarsDict[varName];

                            var resultOfComparison = EqualityCompare(logger, leftVal, rightVal, null, null, null, options, null, dataSource);

                            if (resultOfComparison)
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

                    var varValuesDict = varValuesList.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.ToList());

                    var varNamesListEnumerator = varNamesList.GetEnumerator();

                    if (varNamesListEnumerator.MoveNext())
                    {
                        var resultVarValuesList = new List<(StrongIdentifierValue, LogicalQueryNode)>();

                        BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(logger, varNamesListEnumerator, varValuesDict, resultVarValuesList, resultsOfQueryToRelationList, options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (setIsNegativeToResult)
            {
                queryExecutingCard.IsNegative = true;
            }
        }

        private void FillExecutingCardForIsOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            if (leftExpr.IsNull & rightExpr.IsNull)
            {
                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.UsedKeysList.Add(leftExpr.Name);
                queryExecutingCard.UsedKeysList.Add(rightExpr.Name);
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

                return;
            }

            if ((leftExpr.IsNull & !rightExpr.IsNull) || (!leftExpr.IsNull & rightExpr.IsNull))
            {
                return;
            }

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(logger, leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(logger, rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(logger, leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);
                
                if (resultOfComparison == true)
                {
                    queryExecutingCard.IsSuccess = true;
                    queryExecutingCard.UsedKeysList.Add(leftExpr.Name);
                    queryExecutingCard.UsedKeysList.Add(rightExpr.Name);
                }

                return;
            }

            logger.Info("41D74D94-B8B4-42A8-BDB4-8CFB975017FE", $"leftExpr = {leftExpr?.ToHumanizedString()}");
            logger.Info("77163C03-4B19-4F43-9DA9-07700D17C98B", $"rightExpr = {rightExpr?.ToHumanizedString()}");
            logger.Info("05288B7D-2084-4AE2-9145-12A39B4AC8E3", $"leftExpr = {leftExpr}");
            logger.Info("06EF83BE-ABE5-490B-9BFC-5784E78EF4F3", $"rightExpr = {rightExpr}");

            throw new NotImplementedException();
        }

        private void FillExecutingCardForIsNotOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

            if(leftExpr.IsNull && rightExpr.IsNull)
            {
                return;
            }

            if((leftExpr.IsNull && !rightExpr.IsNull) || (!leftExpr.IsNull && rightExpr.IsNull))
            {
                queryExecutingCard.IsSuccess = true;
                queryExecutingCard.UsedKeysList.Add(leftExpr.Name);
                queryExecutingCard.UsedKeysList.Add(rightExpr.Name);
                return;
            }

            if ((leftExpr.Kind == KindOfLogicalQueryNode.Concept || leftExpr.Kind == KindOfLogicalQueryNode.Entity) && (rightExpr.Kind == KindOfLogicalQueryNode.Concept || rightExpr.Kind == KindOfLogicalQueryNode.Entity))
            {
                var additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(logger, leftExpr.Name, options.LocalCodeExecutionContext);
                var additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(logger, rightExpr.Name, options.LocalCodeExecutionContext);

                var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
                reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance;

                var resultOfComparison = EqualityCompare(logger, leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null, dataSource);

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

                return;
            }

#if DEBUG
            logger.Info("3179F4D5-CABA-4844-A563-0978A77C9AC3", $"leftExpr.Kind = {leftExpr.Kind}");
            logger.Info("65432D55-C080-4DAC-A78F-BE1FA4289D3F", $"rightExpr.Kind = {rightExpr.Kind}");
            logger.Info("B2036CA7-931B-4985-8CB8-06C90B645723", $"rightExpr = {rightExpr.ToHumanizedString()}");
#endif

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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

                return;
            }

            throw new NotImplementedException();
        }

        private void BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(IMonitorLogger logger, IEnumerator<StrongIdentifierValue> varNamesListEnumerator, Dictionary<StrongIdentifierValue, List<(StrongIdentifierValue, LogicalQueryNode)>> varValuesDict, List<(StrongIdentifierValue, LogicalQueryNode)> resultVarValuesList, IList<ResultOfQueryToRelation> resultsOfQueryToRelationList, OptionsOfFillExecutingCard options)
        {
            var varName = varNamesListEnumerator.Current;

            var targetVarsValuesList = varValuesDict[varName];

            foreach (var varValue in targetVarsValuesList)
            {
                var newResultVarValuesList = resultVarValuesList.ToList();

                newResultVarValuesList.Add(varValue);

                if(varNamesListEnumerator.MoveNext())
                {
                    BuildResultsOfQueryToRelationListForBinaryOperatorLogicalQueryNode(logger, varNamesListEnumerator, varValuesDict, newResultVarValuesList, resultsOfQueryToRelationList, options);
                }
                else
                {
                    var resultOfQueryToRelation = new ResultOfQueryToRelation();

                    foreach (var newResultVarValue in newResultVarValuesList)
                    {
                        var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                        resultOfVarOfQueryToRelation.NameOfVar = newResultVarValue.Item1;
                        resultOfVarOfQueryToRelation.FoundExpression = newResultVarValue.Item2;

                        resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);
                    }

                    resultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                }
            }
        }

        private void FillExecutingCardForNotOperatorLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            FillExecutingCard(logger, processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, leftQueryExecutingCard, resultExplainNode);
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
                AppendResults(logger, leftQueryExecutingCard, queryExecutingCard);

                queryExecutingCard.IsSuccess = true;
            }
            else
            {
                queryExecutingCard.IsSuccess = !leftQueryExecutingCard.IsSuccess;
            }

            queryExecutingCard.IsNegative = !leftQueryExecutingCard.IsNegative;
            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            if (parentExplainNode != null)
            {
                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }
        }

        private void FillExecutingCardForCallingFromRelationForFact(IMonitorLogger logger, BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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
                currentExplainNode.TargetRelation = queryExecutingCard.TargetRelationName;
            }

            var targetRelationName = queryExecutingCard.TargetRelationName;

#if DEBUG
            //Log($"targetRelationName = {targetRelationName.ToHumanizedString()}");
#endif

            var usedKeysList = queryExecutingCard.UsedKeysList;

            var useInheritance = options.UseInheritance;
            var inheritanceResolver = _inheritanceResolver;

            var targetRelationsList = processedExpr.RelationsDict[targetRelationName];

            var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
            reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Relation;
            reasonOfFuzzyLogicResolving.RelationName = targetRelationName;

            foreach (var targetRelation in targetRelationsList)
            {
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
                //Log($"targetRelation = {targetRelation.ToHumanizedString()}");
#endif

                usedKeysList.Add(targetRelation.Name);

                var isFitByKnownInfoResult = IsFitByKnownInfo(logger, targetRelation, queryExecutingCard, dataSource, options, rootParentExplainNode, reasonOfFuzzyLogicResolving);

                var isFit = isFitByKnownInfoResult.IsFit;
                var comparisonQueryExecutingCard = isFitByKnownInfoResult.ComparisonQueryExecutingCard;

#if DEBUG
                //Log($"isFit = {isFit}");
#endif

                if (relationWithDirectFactQueryProcessTargetRelationExplainNode != null)
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


                    }

                    if(queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam)
                    {
                        var resultOfQueryToRelation = new ResultOfQueryToRelation();

                        foreach (var paramOfTargetRelation in targetRelation.ParamsList)
                        {
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
                            var paramsListOfTargetRelation = targetRelation.ParamsList;

                            if (DetectExpressionInParamOfRelation(logger, queryExecutingCard.VarsInfoList, paramsListOfTargetRelation))
                            {
                                var resultCache = new List<List<IResultOfVarOfQueryToRelation>>();

                                foreach (var varItem in queryExecutingCard.VarsInfoList)
                                {
                                    var resultCacheItem = new List<IResultOfVarOfQueryToRelation>();
                                    resultCache.Add(resultCacheItem);

                                    var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

                                    if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                                    {
                                        continue;
                                    }

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

                                        queryExecutingCardForExprInParameter.IsFetchingAllValuesForResolvingExpressionParam = true;
                                        queryExecutingCardForExprInParameter.RootParentExplainNode = rootParentExplainNode;
                                        queryExecutingCardForExprInParameter.ParentExplainNode = fetchingAllValuesForResolvingExpressionParamResultExplainNode;

                                        FillExecutingCard(logger, paramOfTargetRelation, queryExecutingCardForExprInParameter, dataSource, options);

                                        if (fetchingAllValuesForResolvingExpressionParamResultExplainNode != null)
                                        {
                                            FillUpResultToExplainNode(logger, queryExecutingCardForExprInParameter, fetchingAllValuesForResolvingExpressionParamResultExplainNode);
                                        }

                                        if (queryExecutingCardForExprInParameter.IsNegative)
                                        {
                                            throw new NotImplementedException();
                                        }

                                        queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExprInParameter.UsedKeysList);

                                        if(queryExecutingCardForExprInParameter.IsSuccess && queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList.Any())
                                        {
                                            foreach(var resultItem in queryExecutingCardForExprInParameter.ResultsOfQueryToRelationList)
                                            {
                                                foreach(var resultVarItem in resultItem.ResultOfVarOfQueryToRelationList)
                                                {
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

                                        var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                                        resultOfVarOfQueryToRelation.NameOfVar = varItem.NameOfVar;
                                        resultOfVarOfQueryToRelation.FoundExpression = paramOfTargetRelation;
                                        resultCacheItem.Add(resultOfVarOfQueryToRelation);
                                    }
                                }

                                var linearizedItems = CollectionCombinationHelper.Combine(resultCache);

                                foreach(var linearizedItem in linearizedItems)
                                {
                                    var resultOfQueryToRelation = new ResultOfQueryToRelation();
                                    resultOfQueryToRelation.ResultOfVarOfQueryToRelationList = linearizedItem;

                                    queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                                }

                                queryExecutingCard.IsSuccess = true;
                            }
                            else
                            {
                                var resultOfQueryToRelation = new ResultOfQueryToRelation();

                                foreach (var varItem in queryExecutingCard.VarsInfoList)
                                {
                                    var paramOfTargetRelation = paramsListOfTargetRelation[varItem.Position];

                                    if (isEntityIdOnly && !paramOfTargetRelation.IsEntityRef)
                                    {
                                        continue;
                                    }

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

                }
            }
        }

        private (bool IsFit, QueryExecutingCardForIndexedPersistLogicalData ComparisonQueryExecutingCard) IsFitByKnownInfo(IMonitorLogger logger, LogicalQueryNode targetRelation, 
            QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options, 
            LogicalSearchExplainNode rootParentExplainNode, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving)
        {
            var paramsListOfTargetRelation = targetRelation.ParamsList;

            var comparisonQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            comparisonQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            comparisonQueryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam = queryExecutingCard.IsFetchingAllValuesForResolvingExpressionParam;
            comparisonQueryExecutingCard.RootParentExplainNode = rootParentExplainNode;

            foreach (var knownInfo in queryExecutingCard.KnownInfoList)
            {
                var position = knownInfo.Position;

                if (position.HasValue)
                {
                    var paramOfTargetRelation = paramsListOfTargetRelation[position.Value];

#if DEBUG
                    //Log($"paramOfTargetRelation = {paramOfTargetRelation.ToHumanizedString()}");
#endif

                    var resultOfComparison = CompareKnownInfoAndParamOfTargetRelation(logger, knownInfo, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard, dataSource);

#if DEBUG
                    //Log($"resultOfComparison = {resultOfComparison}");
#endif

                    if (!resultOfComparison)
                    {
                        return (false, comparisonQueryExecutingCard);
                    }
                }
                else
                {
                    return (false, comparisonQueryExecutingCard);
                }
            }

            return (true, comparisonQueryExecutingCard);
        }

        private bool DetectExpressionInParamOfRelation(IMonitorLogger logger, IList<QueryExecutingCardAboutVar> varsInfoList, IList<LogicalQueryNode> paramsListOfTargetRelation)
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

        private void FillExecutingCardForGroupLogicalQueryNode(IMonitorLogger logger, LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            FillExecutingCard(logger, processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, leftQueryExecutingCard, resultExplainNode);
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

            if (leftQueryExecutingCard.IsNegative)
            {
                throw new NotImplementedException();
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.RootParentExplainNode = rootParentExplainNode;
                queryExecutingCardForFillExecutingCardUsingPostFiltersList.ParentExplainNode = parentExplainNode;

                var postFilterNode = FillExecutingCardUsingPostFiltersList(logger, leftQueryExecutingCard, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options, dataSource);

                AppendResults(logger, queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);

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

                AppendResults(logger, leftQueryExecutingCard, queryExecutingCard);
            }
        }

        private bool CompareKnownInfoAndParamOfTargetRelation(IMonitorLogger logger, QueryExecutingCardAboutKnownInfo knownInfo, LogicalQueryNode paramOfTargetRelation, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            if(CompareKnownInfoAndExpressionNode(logger, knownInfo.Expression, paramOfTargetRelation, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource))
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
                        if (CompareKnownInfoAndExpressionNode(logger, knownInfoItem, paramOfTargetRelationItem, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CompareKnownInfoAndExpressionNode(IMonitorLogger logger, LogicalQueryNode knownInfo, LogicalQueryNode expressionNode, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
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
                        additionalKeys_1 = _inheritanceResolver.GetSuperClassesKeysList(logger, knownInfo.Name, options.LocalCodeExecutionContext);
                        break;

                    case KindOfLogicalQueryNode.Value:
                    case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    case KindOfLogicalQueryNode.Relation:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(knownInfoKind), knownInfoKind, null);
                }
            }

            List<StrongIdentifierValue> additionalKeys_2 = null;

            if (useInheritance && expressionNode.IsKeyRef)
            {
                additionalKeys_2 = _inheritanceResolver.GetSuperClassesKeysList(logger, expressionNode.Name, options.LocalCodeExecutionContext);
            }

            return EqualityCompare(logger, knownInfo, expressionNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard, dataSource);
        }

        private void FillExecutingCardForCallingFromRelationForProduction(IMonitorLogger logger, BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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
                currentExplainNode.TargetRelation = queryExecutingCard.TargetRelationName;
            }

            var targetRelationsList = processedExpr.RelationsDict[queryExecutingCard.TargetRelationName];

            if (targetRelationsList.Count != 1)
            {
                if(currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add($"{nameof(targetRelationsList)} should has 1 item instead of {targetRelationsList.Count}");
                }

                return;
            }

            var targetRelation = targetRelationsList.First();

            if (targetRelation.ParamsList.Count != queryExecutingCard.CountParams)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add($"targetRelation.ParamsList.Count != queryExecutingCard.CountParams: {targetRelation.ParamsList.Count} != {queryExecutingCard.CountParams}");
                }

                return;
            }

#if DEBUG
            //Log($"targetRelation = {targetRelation.ToHumanizedString()}");
#endif

#if DEBUG
            //Log($"queryExecutingCard.KnownInfoList = {queryExecutingCard.KnownInfoList.WriteListToString()}");
#endif

            var reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving();
            reasonOfFuzzyLogicResolving.Kind = KindOfReasonOfFuzzyLogicResolving.Relation;
            reasonOfFuzzyLogicResolving.RelationName = targetRelation.Name;

            var isFitByKnownInfoResult = IsFitByKnownInfo(logger, targetRelation, queryExecutingCard, dataSource, options, rootParentExplainNode, reasonOfFuzzyLogicResolving);

            var isFit = isFitByKnownInfoResult.IsFit;

#if DEBUG
            //Log($"isFit = {isFit}");
#endif

            if(!isFit)
            {
                if (currentExplainNode != null)
                {
                    currentExplainNode.AdditionalInformation.Add($"targetRelation does not fit by known info.");
                }

                return;
            }

            var targetRelationVarsInfoList = targetRelation.VarsInfoList;

            var targetRelationVarsInfoDictByPosition = targetRelationVarsInfoList.ToDictionary(p => p.Position, p => p.NameOfVar);

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(logger, targetRelation.KnownInfoList, targetRelationVarsInfoList, queryExecutingCard.KnownInfoList, true);
            if (!mergingResult.IsSuccess)
            {
                return;
            }

            var targetKnownInfoList = mergingResult.KnownInfoList;

#if DEBUG
            //Log($"targetKnownInfoList = {targetKnownInfoList.WriteListToString()}");
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

            var nextPartsList = processedExpr.GetNextPartsList(logger);

            foreach (var nextPart in nextPartsList)
            {
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
                queryExecutingCardForNextPart.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

                FillExecutingCardForCallingFromOtherPart(logger, nextPart, queryExecutingCardForNextPart, dataSource, options);

                if (nextPartLocalResultExplainNode != null)
                {
                    FillUpResultToExplainNode(logger, queryExecutingCardForNextPart, nextPartLocalResultExplainNode);
                }

                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForNextPart.UsedKeysList);

                if (queryExecutingCardForNextPart.IsNegative)
                {
                    throw new NotImplementedException();
                }

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

                    foreach (var varInfo in varsInfoList)
                    {
                        var targetInternalKeyOfVar = targetRelationVarsInfoDictByPosition[varInfo.Position];

                        backKeysDict[targetInternalKeyOfVar] = varInfo.NameOfVar;
                    }

                    foreach (var resultOfQueryToRelation in resultsOfQueryToRelationList)
                    {
                        var newResultOfQueryToRelation = new ResultOfQueryToRelation();
                        var newResultOfVarOfQueryToRelationList = new List<IResultOfVarOfQueryToRelation>();

                        foreach (var resultOfVarOfQueryToRelation in resultOfQueryToRelation.ResultOfVarOfQueryToRelationList)
                        {
                            var internalKeyOfVar = resultOfVarOfQueryToRelation.NameOfVar;

                            if (backKeysDict.ContainsKey(internalKeyOfVar))
                            {
                                var externalKeyOfVar = backKeysDict[internalKeyOfVar];

                                resultOfVarOfQueryToRelation.NameOfVar = externalKeyOfVar;

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

        }

        private void FillExecutingCardForCallingFromOtherPart(IMonitorLogger logger, BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
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
            queryExecutingCardForExpression.UsedRelations.AddRange(queryExecutingCard.UsedRelations);

            FillExecutingCard(logger, processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

            if (resultExplainNode != null)
            {
                FillUpResultToExplainNode(logger, queryExecutingCardForExpression, resultExplainNode);
            }

            queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

            if (queryExecutingCardForExpression.IsNegative)
            {
                throw new NotImplementedException();
            }

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

        }

        private bool EqualityCompare(IMonitorLogger logger, LogicalQueryNode expressionNode1, LogicalQueryNode expressionNode2, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reason, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource)
        {
            if(expressionNode1.IsNull && expressionNode2.IsNull)
            {
                return true;
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar && (expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Entity))
                || (expressionNode2.Kind == KindOfLogicalQueryNode.LogicalVar && (expressionNode1.Kind == KindOfLogicalQueryNode.Concept || expressionNode1.Kind == KindOfLogicalQueryNode.Entity)))
            {
                StrongIdentifierValue nameOfVar = null;
                LogicalQueryNode foundExpression = null;

                if (expressionNode1.Kind == KindOfLogicalQueryNode.LogicalVar)
                {
                    nameOfVar = expressionNode1.Name;
                    foundExpression = expressionNode2;
                }
                else
                {
                    nameOfVar = expressionNode2.Name;
                    foundExpression = expressionNode1;
                }

                var resultOfQueryToRelation = new ResultOfQueryToRelation();

                var resultOfVarOfQueryToRelation = new ResultOfVarOfQueryToRelation();
                resultOfVarOfQueryToRelation.NameOfVar = nameOfVar;
                resultOfVarOfQueryToRelation.FoundExpression = foundExpression;
                resultOfQueryToRelation.ResultOfVarOfQueryToRelationList.Add(resultOfVarOfQueryToRelation);

                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);

                queryExecutingCard.UsedKeysList.Add(foundExpression.Name);

                return true;
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.Concept || expressionNode1.Kind == KindOfLogicalQueryNode.Entity) && (expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Entity))
            {
                var key_1 = expressionNode1.Name;
                var key_2 = expressionNode2.Name;

                if (key_1 == key_2)
                {
                    if(queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }

                if (additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2))
                {
                    if (queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_1);
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }

                if (additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1))
                {
                    if (queryExecutingCard != null)
                    {
                        queryExecutingCard.UsedKeysList.Add(key_1);
                        queryExecutingCard.UsedKeysList.Add(key_2);
                    }

                    return true;
                }



                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                var synonymsList1 = _synonymsResolver.GetSynonyms(logger, key_1, localCodeExecutionContext);

                if(synonymsList1.Contains(key_2))
                {
                    return true;
                }

                var synonymsList2 = _synonymsResolver.GetSynonyms(logger, key_2, localCodeExecutionContext);

                if (synonymsList2.Contains(key_1))
                {
                    return true;
                }

                if (synonymsList1.Intersect(synonymsList2).Any())
                {
                    return true;
                }

                var fuzzyResult = _fuzzyLogicResolver.Equals(logger, key_1, key_2, localCodeExecutionContext);

                return fuzzyResult;
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Value && expressionNode2.Kind == KindOfLogicalQueryNode.Value)
            {
                var sysValue1 = expressionNode1.Value.GetSystemValue();
                var sysValue2 = expressionNode2.Value.GetSystemValue();

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            if ((expressionNode1.IsKeyRef && expressionNode2.Kind == KindOfLogicalQueryNode.Value && (expressionNode2.Value.IsLogicalValue || expressionNode2.Value.IsNumberValue)) || (expressionNode2.IsKeyRef && expressionNode1.Kind == KindOfLogicalQueryNode.Value && (expressionNode1.Value.IsNumberValue || expressionNode1.Value.IsLogicalValue)))
            {
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

                var value = valueNode.Value;
                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                if (value.GetSystemValue() == null || !_numberValueLinearResolver.CanBeResolved(logger, value))
                {
                    return false;
                }

                return _fuzzyLogicResolver.Equals(logger, conceptNode.Name, _numberValueLinearResolver.Resolve(logger, value, localCodeExecutionContext), reason, localCodeExecutionContext);
            }

            if ((expressionNode1.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence && expressionNode2.Kind == KindOfLogicalQueryNode.Value && (expressionNode2.Value.IsLogicalValue || expressionNode2.Value.IsNumberValue)) || (expressionNode2.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence && expressionNode1.Kind == KindOfLogicalQueryNode.Value && (expressionNode1.Value.IsNumberValue || expressionNode1.Value.IsLogicalValue)))
            {
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

                var value = valueNode.Value;
                var localCodeExecutionContext = options.LocalCodeExecutionContext;

                if (value.GetSystemValue() == null || !_numberValueLinearResolver.CanBeResolved(logger, value))
                {
                    return false;
                }

                return _fuzzyLogicResolver.Equals(logger, sequenceNode.FuzzyLogicNonNumericSequenceValue, _numberValueLinearResolver.Resolve(logger, value, localCodeExecutionContext), reason, localCodeExecutionContext);
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Relation && expressionNode2.Kind == KindOfLogicalQueryNode.Relation)
            {
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

                    if (!EqualityCompare(logger, param1, param2, null, null, reason, options, queryExecutingCard, dataSource))
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

            if((expressionNode1.Kind == KindOfLogicalQueryNode.Entity && expressionNode2.Kind == KindOfLogicalQueryNode.Value) ||
                (expressionNode2.Kind == KindOfLogicalQueryNode.Entity && expressionNode1.Kind == KindOfLogicalQueryNode.Value))
            {
                StrongIdentifierValue entityName = null;
                Value value = null;

                if (expressionNode1.Kind == KindOfLogicalQueryNode.Entity)
                {
                    entityName = expressionNode1.Name;
                    value = expressionNode2.Value;
                }
                else
                {
                    entityName = expressionNode2.Name;
                    value = expressionNode1.Value;
                }

                if(value.IsConditionalEntityValue)
                {
                    var conditionalEntityValue = value.AsConditionalEntityValue;

                    var conditionalEntityId = conditionalEntityValue.ResolveAndGetEntityId(logger);


                    return entityName == conditionalEntityId;
                }

                if(value.IsNullValue)
                {
                    return false;
                }

#if DEBUG
                logger.Info("04E7EB6B-B92A-4742-A5E3-228444466B15", $"expressionNode1 = {expressionNode1}");
                logger.Info("6D12CBA5-366C-469A-ACBC-1C7B74E0DA9B", $"expressionNode2 = {expressionNode2}");
#endif

                throw new NotImplementedException();
            }

#if DEBUG
            logger.Info("5B3B3D0F-E2BE-44A7-982F-CCBF45F27752", $"expressionNode1 = {expressionNode1}");
            logger.Info("AA528513-841D-40DC-933C-157E99771721", $"expressionNode2 = {expressionNode2}");
#endif

            throw new NotImplementedException();
        }
    }
}
