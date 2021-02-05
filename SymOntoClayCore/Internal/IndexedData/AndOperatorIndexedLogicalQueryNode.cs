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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class AndOperatorIndexedLogicalQueryNode: BinaryOperatorIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.BinaryOperator;

        /// <inheritdoc/>
        public override KindOfOperatorOfLogicalQueryNode KindOfOperator => KindOfOperatorOfLogicalQueryNode.And;

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;
#endif
            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
            leftQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            leftQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
            leftQueryExecutingCard.SenderExpressionNode = this;
#endif
            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            Left.FillExecutingCard(leftQueryExecutingCard, dataSource, options);

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

            if(leftQueryExecutingCardResultsOfQueryToRelationList.Any())
            {
                var resultsOfQueryToRelationList = queryExecutingCard.ResultsOfQueryToRelationList;

                foreach (var leftResultOfQueryToRelation in leftQueryExecutingCardResultsOfQueryToRelationList)
                {
                    var rightQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
#if DEBUG
                    rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
                    rightQueryExecutingCard.SenderExpressionNode = this;
#endif
                    rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                    Right.FillExecutingCard(rightQueryExecutingCard, dataSource, options);

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
                    var leftVarsKeysList = leftVarsList.Select(p => p.KeyOfVar).Distinct().ToList();

                    foreach (var rightResultOfQueryToRelation in rightQueryExecutingCardResultsOfQueryToRelationList)
                    {
                        var rightVarsList = rightResultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                        var rightVarsKeysList = rightVarsList.Select(p => p.KeyOfVar).Distinct().ToList();
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
                            var leftVarsDict = new Dictionary<ulong, ResultOfVarOfQueryToRelation>();
                            var resultItem = new ResultOfQueryToRelation();
                            foreach (var varItem in leftVarsList)
                            {
                                var keyOfVars = varItem.KeyOfVar;
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
                                var keyOfVars = varItem.KeyOfVar;
                                if (intersectOfVarsKeysList.Contains(keyOfVars))
                                {
                                    var leftVarItem = leftVarsDict[keyOfVars];
                                    var resultOfComparison = ExpressionNodeHelper.Compare(varItem.FoundExpression, leftVarItem.FoundExpression, null
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
                rightQueryExecutingCard.SenderExpressionNode = this;
#endif
                rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                Right.FillExecutingCard(rightQueryExecutingCard, dataSource, options);

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
    }
}
