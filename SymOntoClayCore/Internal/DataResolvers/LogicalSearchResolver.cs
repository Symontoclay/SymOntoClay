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
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        private InheritanceResolver _inheritanceResolver;
        private FuzzyLogicResolver _fuzzyLogicResolver;
        private NumberValueLinearResolver _numberValueLinearResolver;

        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
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

            if(_inheritanceResolver == null)
            {
                var dataResolversFactory = _context.DataResolversFactory;

                _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
                _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
                _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            }

            var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
            optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
            optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;
            optionsOfFillExecutingCard.UseInheritance = options.UseInheritance; 
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

#if DEBUG
            //Log($"queryExpression = {queryExpression}");
            //Log($"DebugHelperForRuleInstance.ToString(queryExpression) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

            queryExpression.CheckDirty();

            queryExpression = queryExpression.Normalized;

#if DEBUG
            //Log($"queryExpression (after) = {queryExpression}");
            //Log($"DebugHelperForRuleInstance.ToString(queryExpression) (after) = {DebugHelperForRuleInstance.ToString(queryExpression)}");
#endif

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

#if DEBUG
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

        private void FillExecutingCard(RuleInstance processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin");
#endif

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();

            FillExecutingCard(processedExpr.PrimaryPart, queryExecutingCardForPart_1, dataSource, options);

            if(queryExecutingCardForPart_1.IsPostFiltersListOnly)
            {
                throw new NotImplementedException();
            }

            if (queryExecutingCardForPart_1.PostFiltersList.Any())
            {
                throw new NotImplementedException();
            }

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
            //options.Logger.Log($"Begin ~~~~~~ processedExpr = {processedExpr}");
            //options.Logger.Log($"Begin ~~~~~~ processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

            FillExecutingCard(processedExpr.Expression, queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            //options.Logger.Log($"~~~~~~ (after) processedExpr = {processedExpr}");
            //options.Logger.Log($"~~~~~~ (after) processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
            //options.Logger.Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            if(!queryExecutingCardForExpression.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForExpression.UsedKeysList);

                return;
            }

            if(queryExecutingCardForExpression.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();

                FillExecutingCardUsingPostFiltersList(queryExecutingCardForExpression, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options);

                AppendResults(queryExecutingCardForFillExecutingCardUsingPostFiltersList, queryExecutingCard);
            }
            else
            {
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

        private void FillExecutingCardUsingPostFiltersList(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, OptionsOfFillExecutingCard options)
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

            foreach (var postFilter in postFiltersList)
            {
                var kindOfBinaryOperator = postFilter.KindOfBinaryOperator;

                var oldTargetSourceQueryExecutingCard = targetSourceQueryExecutingCard;

                targetSourceQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

                switch (kindOfBinaryOperator)
                {
                    case KindOfOperatorOfLogicalQueryNode.And:
                        FillExecutingCardUsingPostFilterListWithAndStrategy(oldTargetSourceQueryExecutingCard, targetSourceQueryExecutingCard, postFilter, options);
#if DEBUG
                        //options.Logger.Log($"targetSourceQueryExecutingCard = {targetSourceQueryExecutingCard}");
#endif
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfBinaryOperator), kindOfBinaryOperator, null);
                }
            }

#if DEBUG
            //options.Logger.Log($"targetSourceQueryExecutingCard (after) = {targetSourceQueryExecutingCard}");
#endif

            if(targetSourceQueryExecutingCard.IsSuccess)
            {
                destQueryExecutingCard.IsSuccess = true;

                CopyResultsOfQueryToRelationList(targetSourceQueryExecutingCard, destQueryExecutingCard);
                destQueryExecutingCard.UsedKeysList.AddRange(targetSourceQueryExecutingCard.UsedKeysList);
            }

#if DEBUG
            //options.Logger.Log($"///////////////// destQueryExecutingCard = {destQueryExecutingCard}");
#endif
        }

        private void FillExecutingCardUsingPostFilterListWithAndStrategy(QueryExecutingCardForIndexedPersistLogicalData sourceQueryExecutingCard, QueryExecutingCardForIndexedPersistLogicalData destQueryExecutingCard, PostFilterOfQueryExecutingCardForPersistLogicalData postFilter, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"sourceQueryExecutingCard = {sourceQueryExecutingCard}");
            //options.Logger.Log($"destQueryExecutingCard = {destQueryExecutingCard}");
            //options.Logger.Log($"postFilter = {postFilter}");
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
            //options.Logger.Log($"kindOfOperator = {kindOfOperator}");
#endif

            var resultsOfQueryToRelationList = new List<ResultOfQueryToRelation>();
            var usedKeysList = new List<StrongIdentifierValue>();

            if(leftExpr.Kind == KindOfLogicalQueryNode.LogicalVar && rightExpr.Kind == KindOfLogicalQueryNode.LogicalVar)
            {
                var leftVariableName = leftExpr.Name;
                var rightVariableName = rightExpr.Name;

#if DEBUG
                //options.Logger.Log($"leftVariableName = {leftVariableName}");
                //options.Logger.Log($"rightVariableName = {rightVariableName}");
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

                        var resultOfComparison = CompareForPostFilter(kindOfOperator, sourceLeftNode, sourceRightNode, options, comparisonQueryExecutingCard);

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
                //options.Logger.Log($"variableOfFilter = {variableOfFilter}");
                //options.Logger.Log($"valueOfFilter = {nodeOfFilter}");
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
                            resultOfComparison = CompareForPostFilter(kindOfOperator, sourceNode, nodeOfFilter, options, comparisonQueryExecutingCard);
                        }
                        else
                        {
                            resultOfComparison = CompareForPostFilter(kindOfOperator, nodeOfFilter, sourceNode, options, comparisonQueryExecutingCard);
                        }

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
        }

        private bool CompareForPostFilter(KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
            switch(kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Is:
                    return CompareForPostFilterByOperatorIs(leftNode, rightNode, options, queryExecutingCard);

                case KindOfOperatorOfLogicalQueryNode.IsNot:
                    return !CompareForPostFilterByOperatorIs(leftNode, rightNode, options, queryExecutingCard);

                case KindOfOperatorOfLogicalQueryNode.More:
                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                case KindOfOperatorOfLogicalQueryNode.Less:
                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    return CompareForPostFilterByOperatorsMoreOrLess(kindOfOperator, leftNode, rightNode, options, queryExecutingCard);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }
        }

        private bool CompareForPostFilterByOperatorIs(LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
#if DEBUG
            //options.Logger.Log($"leftNode = {leftNode}");
            //options.Logger.Log($"rightNode = {rightNode}");
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

            return EqualityCompare(leftNode, rightNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard);
        }

        private bool CompareForPostFilterByOperatorsMoreOrLess(KindOfOperatorOfLogicalQueryNode kindOfOperator, LogicalQueryNode leftNode, LogicalQueryNode rightNode, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
#if DEBUG
            //options.Logger.Log($"kindOfOperator = {kindOfOperator}");
            //options.Logger.Log($"leftNode = {leftNode}");
            //options.Logger.Log($"rightNode = {rightNode}");
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

            NFillExecutingCard(processedExpr, queryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"^^^^^^queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            //options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() (after) = {processedExpr.GetHumanizeDbgString()}");

            //options.Logger.Log("End");
#endif
        }

        private void NFillExecutingCard(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
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

            var indexedRulePartsOfFactsList = dataSource.GetIndexedRulePartOfFactsByKeyOfRelation(processedExpr.Name);

#if DEBUG
            //options.Logger.Log($"indexedRulePartsOfFactsList?.Count = {indexedRulePartsOfFactsList?.Count}");
#endif

            var mergingResult = QueryExecutingCardAboutKnownInfoHelper.Merge(processedExpr.KnownInfoList, processedExpr.VarsInfoList, queryExecutingCard.KnownInfoList, false);

#if DEBUG
            //options.Logger.Log($"mergingResult.IsSuccess = {mergingResult.IsSuccess}");
#endif

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

                    FillExecutingCardForCallingFromRelationForFact(indexedRulePartsOfFacts, queryExecutingCardForTargetFact, dataSource, options);

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

                        queryExecutingCard.IsSuccess = true;

                        foreach (var resultOfQueryToRelation in queryExecutingCardForTargetFact.ResultsOfQueryToRelationList)
                        {
                            queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                        }

                        queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForTargetFact.UsedKeysList);
                    }
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
                    //options.Logger.Log($"processedExpr = {processedExpr}");
                    //options.Logger.Log($"indexedRulePartsOfRule = {indexedRulePartsOfRule}");
                    //options.Logger.Log($"indexedRulePartsOfRule = {DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePartsOfRule)}");
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

                        queryExecutingCard.IsSuccess = true;

                        queryExecutingCard.UsedKeysList.AddRange(queryExecutingCardForTargetRule.UsedKeysList);

                        foreach (var resultOfQueryToRelation in queryExecutingCardForTargetRule.ResultsOfQueryToRelationList)
                        {
                            queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
                        }
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
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

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

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Right, rightQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log("||||||||||||||||||||||||||||||||||||||||||||||||||||~~~~~~~~~~~~");
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
            //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif

            if (!rightQueryExecutingCard.IsSuccess)
            {
                return;
            }

            queryExecutingCard.IsSuccess = true;

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList.Concat(rightQueryExecutingCard.UsedKeysList));

            queryExecutingCard.UsedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

            var leftQueryExecutingCardResultsOfQueryToRelationList = leftQueryExecutingCard.ResultsOfQueryToRelationList;
            var rightQueryExecutingCardResultsOfQueryToRelationList = rightQueryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCard.IsPostFiltersListOnly && rightQueryExecutingCard.IsPostFiltersListOnly)
            {
                queryExecutingCard.IsPostFiltersListOnly = true;

                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            if (!leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                CopyPostFilters(leftQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);
                CopyPostFilters(rightQueryExecutingCard, queryExecutingCard, KindOfOperatorOfLogicalQueryNode.And);

                return;
            }

            var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

            if (leftQueryExecutingCardResultsOfQueryToRelationList.Any() && !rightQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
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

                            var resultOfComparison = EqualityCompare(leftVal, rightVal, null, null, null, options, null);

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
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
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

                            var resultOfComparison = EqualityCompare(leftVal, rightVal, null, null, null, options, null);

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

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null);

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

                var resultOfComparison = EqualityCompare(leftExpr, rightExpr, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, null);

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
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

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

        private void FillExecutingCardForMoreOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

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

        private void FillExecutingCardForLessOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

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

        private void FillExecutingCardForLessOrEqualOperatorLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {processedExpr}");
            //options.Logger.Log($"processedExpr.GetHumanizeDbgString() = {processedExpr.GetHumanizeDbgString()}");
#endif

            var leftExpr = processedExpr.Left;
            var rightExpr = processedExpr.Right;

#if DEBUG
            //options.Logger.Log($"leftExpr = {leftExpr}");
            //options.Logger.Log($"rightExpr = {rightExpr}");
#endif

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
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
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
            //foreach(var item in queryExecutingCard.KnownInfoList)
            //{
            //    options.Logger.Log($"item = {item}");
            //}
#endif

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
                //options.Logger.Log($"targetRelation = {targetRelation.GetHumanizeDbgString()}");
                //options.Logger.Log($"targetRelation.CountParams = {targetRelation.CountParams}");
                //options.Logger.Log($"queryExecutingCard.CountParams = {queryExecutingCard.CountParams}");
#endif
                if (targetRelation.CountParams != queryExecutingCard.CountParams)
                {
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

                        var resultOfComparison = CompareKnownInfoAndExpressionNode(knownInfo, paramOfTargetRelation, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, comparisonQueryExecutingCard);

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

                    queryExecutingCard.UsedKeysList.AddRange(comparisonQueryExecutingCard.UsedKeysList);
                    queryExecutingCard.ResultsOfQueryToRelationList.AddRange(comparisonQueryExecutingCard.ResultsOfQueryToRelationList);

#if DEBUG
                    //options.Logger.Log($"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& queryExecutingCard = {queryExecutingCard}");
#endif
                }
            }
        }

        private void FillExecutingCardForGroupLogicalQueryNode(LogicalQueryNode processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            FillExecutingCard(processedExpr.Left, leftQueryExecutingCard, dataSource, options);

            if (!leftQueryExecutingCard.IsSuccess)
            {
                queryExecutingCard.UsedKeysList.AddRange(leftQueryExecutingCard.UsedKeysList);

                return;
            }

            if (leftQueryExecutingCard.PostFiltersList.Any())
            {
                var queryExecutingCardForFillExecutingCardUsingPostFiltersList = new QueryExecutingCardForIndexedPersistLogicalData();

                FillExecutingCardUsingPostFiltersList(leftQueryExecutingCard, queryExecutingCardForFillExecutingCardUsingPostFiltersList, options);

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

        private bool CompareKnownInfoAndExpressionNode(QueryExecutingCardAboutKnownInfo knownInfo, LogicalQueryNode expressionNode, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
            var knownInfoExpression = knownInfo.Expression;

            return EqualityCompare(knownInfoExpression, expressionNode, additionalKeys_1, additionalKeys_2, reasonOfFuzzyLogicResolving, options, queryExecutingCard);
        }

        private void FillExecutingCardForCallingFromRelationForProduction(BaseRulePart processedExpr, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            //options.Logger.Log($"processedExpr = {DebugHelperForRuleInstance.BaseRulePartToString(processedExpr)}");
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
            //if (targetKnownInfoList.Any())
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var nextPartsList = processedExpr.GetNextPartsList();

#if DEBUG
            //options.Logger.Log($"nextPartsList.Count = {nextPartsList.Count}");
#endif

            foreach (var nextPart in nextPartsList)
            {
#if DEBUG
                //options.Logger.Log($"nextPart = {DebugHelperForRuleInstance.BaseRulePartToString(nextPart)}");
#endif

                var queryExecutingCardForNextPart = new QueryExecutingCardForIndexedPersistLogicalData();
                queryExecutingCardForNextPart.VarsInfoList = targetRelation.VarsInfoList;
                queryExecutingCardForNextPart.KnownInfoList = targetKnownInfoList;

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
            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

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

        private bool EqualityCompare(LogicalQueryNode expressionNode1, LogicalQueryNode expressionNode2, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2, ReasonOfFuzzyLogicResolving reason, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
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
                if((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                {
                    //options.Logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
                    //options.Logger.Log($"expressionNode1 = {expressionNode1}");
                    //options.Logger.Log($"expressionNode2 = {expressionNode2}");
                    //options.Logger.Log($"key_1 = {key_1}");
                    //options.Logger.Log($"key_2 = {key_2}");
                    //options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
                    //options.Logger.Log($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
                    //options.Logger.Log($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
                }
#endif

                if (key_1 == key_2)
                {
#if DEBUG
                    if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    {
                        //options.Logger.Log($"key_1 == key_2 = {key_1 == key_2}");
                    }                    
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
                    if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    {
                        //options.Logger.Log("additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2)");
                    }                    
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
                    if ((key_1.NameValue == "#cleaned metal barrel" && key_2.NameValue == "mynpc") || (key_1.NameValue == "mynpc" && key_2.NameValue == "#cleaned metal barrel"))
                    {
                        //options.Logger.Log("additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1)");
                    }                    
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

                if (sysValue1.Equals(sysValue2))
                {
                    return true;
                }

                if(ObjectHelper.IsNumber(sysValue1) && ObjectHelper.IsNumber(sysValue2))
                {
                    if (Convert.ToDouble(sysValue1) == Convert.ToDouble(sysValue2))
                    {
                        return true;
                    }
                }

                return false;
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

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Relation && expressionNode2.Kind == KindOfLogicalQueryNode.Relation)
            {
#if DEBUG
                //options.Logger.Log("Try to compare relations!");
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
                    //options.Logger.Log($"param1 = {param1}");
                    //options.Logger.Log($"param2 = {param2}");
                    //options.Logger.Log($"EqualityCompare(param1, param2, null, null, reason, options) = {EqualityCompare(param1, param2, null, null, reason, options, queryExecutingCard)}");
                    //options.Logger.Log($"?????????????????>>>>>>>>>>queryExecutingCard (after) = {queryExecutingCard}");
#endif

                    if (!EqualityCompare(param1, param2, null, null, reason, options, queryExecutingCard))
                    {
                        return false;
                    }
                }

                return true;
            }

            if(((expressionNode1.Kind == KindOfLogicalQueryNode.Entity || expressionNode1.Kind == KindOfLogicalQueryNode.Concept || expressionNode1.Kind == KindOfLogicalQueryNode.Value || expressionNode1.Kind == KindOfLogicalQueryNode.EntityCondition || expressionNode1.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence) && expressionNode2.Kind == KindOfLogicalQueryNode.Relation && expressionNode2.VarsInfoList.IsNullOrEmpty()) 
                || ((expressionNode2.Kind == KindOfLogicalQueryNode.Entity || expressionNode2.Kind == KindOfLogicalQueryNode.Concept || expressionNode2.Kind == KindOfLogicalQueryNode.Value || expressionNode2.Kind == KindOfLogicalQueryNode.EntityCondition || expressionNode2.Kind == KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence) && expressionNode1.Kind == KindOfLogicalQueryNode.Relation && expressionNode1.VarsInfoList.IsNullOrEmpty()))
            {
#if DEBUG
                //options.Logger.Log("Try to compare relation and entity, concept or value!");
#endif

                LogicalQueryNode relationNode = null;
                LogicalQueryNode valueNode = null;
                List<StrongIdentifierValue> valueAdditionalKeys = null;

                if (expressionNode1.Kind == KindOfLogicalQueryNode.Relation)
                {
                    relationNode = expressionNode1;
                    valueNode = expressionNode2;
                    valueAdditionalKeys = additionalKeys_2;
                }
                else
                {
                    relationNode = expressionNode2;
                    valueNode = expressionNode1;
                    valueAdditionalKeys = additionalKeys_1;
                }

#if DEBUG
                //options.Logger.Log($"relationNode = {relationNode}");
                //options.Logger.Log($"valueNode = {valueNode}");
                //options.Logger.Log($"valueAdditionalKeys = {JsonConvert.SerializeObject(valueAdditionalKeys?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

                return RecursiveComparisonRelationWithNonRelation(relationNode, valueNode, valueAdditionalKeys, reason, options, queryExecutingCard);
            }

            throw new NotImplementedException();
        }

        private bool RecursiveComparisonRelationWithNonRelation(LogicalQueryNode relationNode, LogicalQueryNode valueNode, List<StrongIdentifierValue> valueAdditionalKeys, ReasonOfFuzzyLogicResolving reason, OptionsOfFillExecutingCard options, QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard)
        {
#if DEBUG
            //options.Logger.Log($"relationNode = {relationNode}");
            //options.Logger.Log($"valueNode = {valueNode}");
            //options.Logger.Log($"valueAdditionalKeys = {JsonConvert.SerializeObject(valueAdditionalKeys?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            foreach (var param in relationNode.ParamsList)
            {
#if DEBUG
                //options.Logger.Log($"param = {param}");
#endif

                var kindOfParam = param.Kind;

                if (kindOfParam == KindOfLogicalQueryNode.Relation)
                {
                    if (RecursiveComparisonRelationWithNonRelation(param, valueNode, valueAdditionalKeys, reason, options, queryExecutingCard))
                    {
                        return true;
                    }
                }

                List<StrongIdentifierValue> additionalKeysOfParam = null;

                switch (kindOfParam)
                {
                    case KindOfLogicalQueryNode.Value:
                        break;

                    case KindOfLogicalQueryNode.Entity:
                    case KindOfLogicalQueryNode.Concept:
                        additionalKeysOfParam = _inheritanceResolver.GetSuperClassesKeysList(param.Name, options.LocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                }

#if DEBUG
                //options.Logger.Log($"additionalKeysOfParam = {JsonConvert.SerializeObject(additionalKeysOfParam?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

                if(EqualityCompare(valueNode, param, valueAdditionalKeys, additionalKeysOfParam, reason, options, queryExecutingCard))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
