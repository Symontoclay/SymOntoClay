using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.EqualityComparers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchVarResultsItemInvertor : BaseResolver
    {
        public LogicalSearchVarResultsItemInvertor(IMainStorageContext context)
            : base(context)
        {
        }

        public IList<T> Invert<T>(IEnumerable<IResultOfQueryToRelation> source, List<StorageUsingOptions> storagesList) 
            where T : IResultOfQueryToRelation
        {
#if DEBUG
            foreach (var tmpItem in source)
            {
                Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

                foreach (var varInfo in tmpItem.ResultOfVarOfQueryToRelationList)
                {
                    Log($"varInfo = {varInfo}");
                }
            }
#endif

            var initialResultVarsDict = source.SelectMany(p => p.ResultOfVarOfQueryToRelationList).GroupBy(p => p.NameOfVar).ToDictionary(p => p.Key, p => p.Select(x => x.FoundExpression).ToList());

#if DEBUG
            var tmpVar = initialResultVarsDict.ToDictionary(p => p.Key.ToHumanizedString(), p => p.Value.Select(x => x.ToHumanizedString()));

            Log($"tmpVar = {JsonConvert.SerializeObject(tmpVar, Formatting.Indented)}");
#endif

            var replacingNotResultsStrategy = ReplacingNotResultsStrategy.DominantKindOfItems;//tmp

            var newResultVarsDict = new Dictionary<StrongIdentifierValue, List<LogicalQueryNode>>();

            foreach (var initialResultVarsKvpItem in initialResultVarsDict)
            {
                var varName = initialResultVarsKvpItem.Key;
                var exceptList = initialResultVarsKvpItem.Value;

#if DEBUG
                Log($"varName = {varName}");
                Log($"exceptList = {exceptList.WriteListToString()}");
#endif

                var newLogicalQueryNodes = GetLogicalQueryNodes(exceptList, replacingNotResultsStrategy, storagesList);

#if DEBUG
                Log($"newLogicalQueryNodes = {newLogicalQueryNodes.WriteListToString()}");
#endif

                newResultVarsDict[varName] = newLogicalQueryNodes;
            }

            return ConvertToResult<T>(newResultVarsDict);
        }

        private static readonly List<KindOfLogicalQueryNode> EmptyTargetKindsOfItems = new List<KindOfLogicalQueryNode>();
        private static LogicalQueryNodeEqualityComparer _logicalQueryNodeEqualityComparer = new LogicalQueryNodeEqualityComparer();

        private IList<T> ConvertToResult<T>(Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source)
            where T : IResultOfQueryToRelation
        {
            var keysList = source.Keys.ToList();

            var result = new List<T>();

            ProcessConvertToResult(source, 0, keysList, new List<(StrongIdentifierValue, LogicalQueryNode)>(), ref result);

            return result;
        }

        private static void ProcessConvertToResult<T>(Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList, 
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation
        {
            if (n == keysList.Count - 1)
            {
                ProcessConvertToResultFinalNode(source, n, keysList, currentValues, ref result);
            }
            else
            {
                ProcessConvertToResultIntermediateNode(source, n, keysList, currentValues, ref result);
            }
        }

        private static void ProcessConvertToResultIntermediateNode<T>(Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList,
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation
        {
            var key = keysList[n];

            var list = source[key];

            var nextN = n + 1;

            foreach (var item in list)
            {
                var newCurrentValues = currentValues.ToList();
                newCurrentValues.Add((key, item));

                ProcessConvertToResult(source, nextN, keysList, newCurrentValues, ref result);
            }
        }

        private static void ProcessConvertToResultFinalNode<T>(Dictionary<StrongIdentifierValue, List<LogicalQueryNode>> source, int n, List<StrongIdentifierValue> keysList,
            List<(StrongIdentifierValue, LogicalQueryNode)> currentValues, ref List<T> result)
            where T : IResultOfQueryToRelation
        {
            throw new NotImplementedException();
        }

        private (List<KindOfLogicalQueryNode> TargetKindsOfItems, ReplacingNotResultsStrategy ReplacingNotResultsStrategy) CalculateTargetKindsOfItems(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy)
        {
#if DEBUG
            Log($"exceptList = {exceptList.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
#endif

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

                        return (GetOrderedKindOfLogicalQueryNodes(exceptList).ToList(), replacingNotResultsStrategy);
                    }

                case ReplacingNotResultsStrategy.FirstPresentNextOtherKindOfItems:
                    {
                        if(exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                        }

                        return (GetOrderedKindOfLogicalQueryNodes(exceptList).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                    }

                case ReplacingNotResultsStrategy.DominantKindOfItems:
                    {
                        if (exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                        }

                        return (GetOrderedKindOfLogicalQueryNodes(exceptList).Take(1).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }
        }

        private IEnumerable<KindOfLogicalQueryNode> GetOrderedKindOfLogicalQueryNodes(IEnumerable<LogicalQueryNode> exceptList)
        {
            return exceptList.Select(p => p.Kind).GroupBy(p => p).Select(p => new
            {
                Value = p.Key,
                Count = p.Count()
            }).OrderByDescending(p => p.Count).Select(p => p.Value);
        }

        private List<LogicalQueryNode> GetLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            Log($"exceptList = {exceptList.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
#endif

            var calculateTargetKindsOfItemsResult = CalculateTargetKindsOfItems(exceptList, replacingNotResultsStrategy);

            var targetKindsOfItems = calculateTargetKindsOfItemsResult.TargetKindsOfItems;

#if DEBUG
            Log($"targetKindsOfItems = {targetKindsOfItems.WritePODListToString()}");
#endif

            return SortLogicalQueryNodes(GetRawLogicalQueryNodes(exceptList, calculateTargetKindsOfItemsResult.ReplacingNotResultsStrategy, targetKindsOfItems, storagesList), replacingNotResultsStrategy, targetKindsOfItems);
        }

        private List<LogicalQueryNode> GetRawLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems, List<StorageUsingOptions> storagesList)
        {
            var result = new List<LogicalQueryNode>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.LogicalStorage.GetLogicalQueryNodes(exceptList, replacingNotResultsStrategy, targetKindsOfItems);

#if DEBUG
                Log($"itemsList = {itemsList.WriteListToString()}");
#endif

                if (itemsList.IsNullOrEmpty())
                {
                    continue;
                }

                result.AddRange(itemsList);
            }

#if DEBUG
            Log($"result = {result.WriteListToString()}");
#endif

            result = result.Distinct(_logicalQueryNodeEqualityComparer).ToList();

#if DEBUG
            Log($"result (after) = {result.WriteListToString()}");
#endif

            return result;
        }

        private List<LogicalQueryNode> SortLogicalQueryNodes(List<LogicalQueryNode> source, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
#if DEBUG
            Log($"source = {source.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
            Log($"targetKindsOfItems = {targetKindsOfItems.WritePODListToString()}");
#endif

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
#if DEBUG
                            //Log($"targetKind = {targetKind}");
#endif

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
#if DEBUG
                            Log($"targetKind = {targetKind}");
#endif

                            if(sourceDict.TryGetValue(targetKind, out nodesList))
                            {
                                result.AddRange(nodesList);
                            }
                        }

                        var otherKindOfItems = Enum.GetValues(typeof(KindOfLogicalQueryNode)).Cast<KindOfLogicalQueryNode>().Where(p => !targetKindsOfItems.Contains(p));

                        foreach(var otherKind in otherKindOfItems)
                        {
#if DEBUG
                            Log($"otherKind = {otherKind}");
#endif

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
