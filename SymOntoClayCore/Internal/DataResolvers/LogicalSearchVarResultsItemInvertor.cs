/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.EqualityComparers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchVarResultsItemInvertor : BaseResolver
    {
        public LogicalSearchVarResultsItemInvertor(IMainStorageContext context)
            : base(context)
        {
        }

        public IList<T> Invert<T>(IMonitorLogger logger, IEnumerable<IResultOfQueryToRelation> source, List<StorageUsingOptions> storagesList, ReplacingNotResultsStrategy replacingNotResultsStrategy) 
            where T : IResultOfQueryToRelation, new()
        {
#if DEBUG
            //foreach (var tmpItem in source)
            //{
            //    Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

            //    foreach (var varInfo in tmpItem.ResultOfVarOfQueryToRelationList)
            //    {
            //        Log($"varInfo = {varInfo.ToHumanizedString()}");
            //        //Log($"varInfo = {varInfo}");
            //    }
            //}
#endif

            var initialResultVarsDict = source.SelectMany(p => p.ResultOfVarOfQueryToRelationList).GroupBy(p => p.NameOfVar).ToDictionary(p => p.Key, p => p.Select(x => x.FoundExpression).ToList());

#if DEBUG
            //var tmpVar = initialResultVarsDict.ToDictionary(p => p.Key.ToHumanizedString(), p => p.Value.Select(x => x.ToHumanizedString()));

            //Log($"tmpVar = {JsonConvert.SerializeObject(tmpVar, Formatting.Indented)}");
#endif

            //var replacingNotResultsStrategy = ReplacingNotResultsStrategy.DominantKindOfItems;//tmp

#if DEBUG
            //Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
#endif


            var newResultVarsDict = new Dictionary<StrongIdentifierValue, List<LogicalQueryNode>>();

            foreach (var initialResultVarsKvpItem in initialResultVarsDict)
            {
                var varName = initialResultVarsKvpItem.Key;
                var exceptList = initialResultVarsKvpItem.Value;

#if DEBUG
                //Log($"varName = {varName}");
                //Log($"exceptList = {exceptList.WriteListToString()}");
#endif

                var newLogicalQueryNodes = GetLogicalQueryNodes(logger, exceptList, replacingNotResultsStrategy, storagesList);

#if DEBUG
                //Log($"newLogicalQueryNodes = {newLogicalQueryNodes.WriteListToString()}");
#endif

                newResultVarsDict[varName] = newLogicalQueryNodes;
            }

            return ConvertToResult<T>(logger, newResultVarsDict);
        }

        private static readonly List<KindOfLogicalQueryNode> EmptyTargetKindsOfItems = new List<KindOfLogicalQueryNode>();
        private static LogicalQueryNodeEqualityComparer _logicalQueryNodeEqualityComparer = new LogicalQueryNodeEqualityComparer();

        private IList<T> ConvertToResult<T>(IMonitorLogger logger, Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source)
            where T : IResultOfQueryToRelation, new()
        {
            var keysList = source.Keys.ToList();

            var result = new List<T>();

            ProcessConvertToResult(logger, source, 0, keysList, new List<(StrongIdentifierValue, LogicalQueryNode)>(), ref result);

            return result;
        }

        private void ProcessConvertToResult<T>(IMonitorLogger logger, Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList, 
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation, new()
        {
            if (n == keysList.Count - 1)
            {
                ProcessConvertToResultFinalNode(logger, source, n, keysList, currentValues, ref result);
            }
            else
            {
                ProcessConvertToResultIntermediateNode(logger, source, n, keysList, currentValues, ref result);
            }
        }

        private void ProcessConvertToResultIntermediateNode<T>(IMonitorLogger logger, Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList,
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation, new()
        {
#if DEBUG
            //Log($"n = {n}");
            //Log($"keysList = {keysList.WriteListToString()}");
#endif

            if(!keysList.Any())
            {
                return;
            }

            var key = keysList[n];
            var list = source[key];
            var nextN = n + 1;

            foreach (var item in list)
            {
                var newCurrentValues = currentValues.ToList();
                newCurrentValues.Add((key, item));

                ProcessConvertToResult(logger, source, nextN, keysList, newCurrentValues, ref result);
            }
        }

        private void ProcessConvertToResultFinalNode<T>(IMonitorLogger logger, Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList,
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation, new()
        {
            var key = keysList[n];
            var list = source[key];

            foreach (var item in list)
            {
                var resultItem = new T();
                result.Add(resultItem);

                var resultOfVarOfQueryToRelationList = new List<IResultOfVarOfQueryToRelation>();
                resultItem.ResultOfVarOfQueryToRelationList = resultOfVarOfQueryToRelationList;

                foreach (var currentValue in currentValues)
                {
                    resultOfVarOfQueryToRelationList.Add(new ResultOfVarOfQueryToRelation() 
                    {
                        NameOfVar = currentValue.Item1,
                        FoundExpression = currentValue.Item2
                    });
                }

                resultOfVarOfQueryToRelationList.Add(new ResultOfVarOfQueryToRelation()
                {
                    NameOfVar = key,
                    FoundExpression = item
                });
            }
        }

        private (List<KindOfLogicalQueryNode> TargetKindsOfItems, ReplacingNotResultsStrategy ReplacingNotResultsStrategy) CalculateTargetKindsOfItems(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy)
        {
            switch(replacingNotResultsStrategy)
            {
                case ReplacingNotResultsStrategy.AllKindOfItems:
                    return (EmptyTargetKindsOfItems, replacingNotResultsStrategy);

                case ReplacingNotResultsStrategy.PresentKindOfItems:
                    {
                        if (exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), replacingNotResultsStrategy);
                        }

                        return (GetOrderedKindOfLogicalQueryNodes(logger, exceptList).ToList(), replacingNotResultsStrategy);
                    }

                case ReplacingNotResultsStrategy.FirstPresentNextOtherKindOfItems:
                    {
                        if(exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                        }

                        return (GetOrderedKindOfLogicalQueryNodes(logger, exceptList).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                    }

                case ReplacingNotResultsStrategy.DominantKindOfItems:
                    {
                        if (exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                        }

                        return (GetOrderedKindOfLogicalQueryNodes(logger, exceptList).Take(1).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }
        }

        private IEnumerable<KindOfLogicalQueryNode> GetOrderedKindOfLogicalQueryNodes(IMonitorLogger logger, IEnumerable<LogicalQueryNode> exceptList)
        {
            return exceptList.Select(p => p.Kind).GroupBy(p => p).Select(p => new
            {
                Value = p.Key,
                Count = p.Count()
            }).OrderByDescending(p => p.Count).Select(p => p.Value);
        }

        private List<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, List<StorageUsingOptions> storagesList)
        {
            var calculateTargetKindsOfItemsResult = CalculateTargetKindsOfItems(logger, exceptList, replacingNotResultsStrategy);

            var targetKindsOfItems = calculateTargetKindsOfItemsResult.TargetKindsOfItems;

            return SortLogicalQueryNodes(logger, GetRawLogicalQueryNodes(logger, exceptList, calculateTargetKindsOfItemsResult.ReplacingNotResultsStrategy, targetKindsOfItems, storagesList), replacingNotResultsStrategy, targetKindsOfItems);
        }

        private List<LogicalQueryNode> GetRawLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems, List<StorageUsingOptions> storagesList)
        {
            var result = new List<LogicalQueryNode>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.LogicalStorage.GetLogicalQueryNodes(logger, exceptList, replacingNotResultsStrategy, targetKindsOfItems);

                if (itemsList.IsNullOrEmpty())
                {
                    continue;
                }

                result.AddRange(itemsList);
            }

            result = result.Distinct(_logicalQueryNodeEqualityComparer).ToList();

            return result;
        }

        private List<LogicalQueryNode> SortLogicalQueryNodes(IMonitorLogger logger, List<LogicalQueryNode> source, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            if(source.IsNullOrEmpty())
            {
                return new List<LogicalQueryNode>();
            }

            switch(replacingNotResultsStrategy)
            {
                case ReplacingNotResultsStrategy.AllKindOfItems:
                case ReplacingNotResultsStrategy.DominantKindOfItems:
                    return source;

                case ReplacingNotResultsStrategy.PresentKindOfItems:
                    {
                        if(targetKindsOfItems.Count == 1)
                        {
                            return source;
                        }

                        if(source.Count == 1)
                        {
                            return source;
                        }

                        var result = new List<LogicalQueryNode>();

                        var sourceDict = source.GroupBy(p => p.Kind).ToDictionary(p => p.Key, p => p.ToList());

                        List<LogicalQueryNode> nodesList = null;

                        foreach (var targetKind in targetKindsOfItems)
                        {
                            if (sourceDict.TryGetValue(targetKind, out nodesList))
                            {
                                result.AddRange(nodesList);
                            }
                        }

                        return result;
                    }

                case ReplacingNotResultsStrategy.FirstPresentNextOtherKindOfItems:
                    {
                        if (source.Count == 1)
                        {
                            return source;
                        }

                        var result = new List<LogicalQueryNode>();

                        var sourceDict = source.GroupBy(p => p.Kind).ToDictionary(p => p.Key, p => p.ToList());

                        List<LogicalQueryNode> nodesList = null;

                        foreach (var targetKind in targetKindsOfItems)
                        {
                            if(sourceDict.TryGetValue(targetKind, out nodesList))
                            {
                                result.AddRange(nodesList);
                            }
                        }

                        var otherKindOfItems = Enum.GetValues(typeof(KindOfLogicalQueryNode)).Cast<KindOfLogicalQueryNode>().Where(p => !targetKindsOfItems.Contains(p));

                        foreach(var otherKind in otherKindOfItems)
                        {
                            if (sourceDict.TryGetValue(otherKind, out nodesList))
                            {
                                result.AddRange(nodesList);
                            }
                        }

                        return result;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }
        }
    }
}
