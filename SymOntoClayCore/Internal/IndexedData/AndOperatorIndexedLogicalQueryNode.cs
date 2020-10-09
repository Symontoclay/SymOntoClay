/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
            var senderIndexedRulePart = queryExecutingCard.SenderIndexedRulePart;

            var leftQueryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();
            leftQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            leftQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
            leftQueryExecutingCard.SenderExpressionNode = this;
            leftQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
            Left.FillExecutingCard(leftQueryExecutingCard, dataSource, options);

#if DEBUG
            //options.Logger.Log($"leftQueryExecutingCard = {leftQueryExecutingCard}");
#endif

            if(!leftQueryExecutingCard.IsSuccess)
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
                    rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                    rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
                    rightQueryExecutingCard.SenderExpressionNode = this;
                    rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                    Right.FillExecutingCard(rightQueryExecutingCard, dataSource, options);

                    queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess;

#if DEBUG
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
                                    var resultOfComparison = ExpressionNodeHelper.Compare(varItem.FoundExpression, leftVarItem.FoundExpression, null, options.Logger);

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
                rightQueryExecutingCard.SenderIndexedRuleInstance = senderIndexedRuleInstance;
                rightQueryExecutingCard.SenderIndexedRulePart = senderIndexedRulePart;
                rightQueryExecutingCard.SenderExpressionNode = this;
                rightQueryExecutingCard.KnownInfoList = queryExecutingCard.KnownInfoList;
                Right.FillExecutingCard(rightQueryExecutingCard, dataSource, options);

                queryExecutingCard.IsSuccess = leftQueryExecutingCard.IsSuccess && rightQueryExecutingCard.IsSuccess;

#if DEBUG
                //options.Logger.Log($"rightQueryExecutingCard = {rightQueryExecutingCard}");
#endif
            }
        }
    }
}
