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

        public IList<T> Invert<T>(IEnumerable<IResultOfQueryToRelation> source, List<StorageUsingOptions> storagesList) where T : IResultOfQueryToRelation
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

            var replacingNotResultsStrategy = ReplacingNotResultsStrategy.AllKindOfItems;//ReplacingNotResultsStrategy.PresentKindOfItems;//tmp

            foreach (var initialResultVarsKvpItem in initialResultVarsDict)
            {
                var varName = initialResultVarsKvpItem.Key;
                var exceptList = initialResultVarsKvpItem.Value;

#if DEBUG
                Log($"varName = {varName}");
                Log($"exceptList = {exceptList.WriteListToString()}");
#endif

                var calculateTargetKindsOfItemsResult = CalculateTargetKindsOfItems(exceptList, replacingNotResultsStrategy);

                var newLogicalQueryNodes = GetLogicalQueryNodes(exceptList, calculateTargetKindsOfItemsResult.ReplacingNotResultsStrategy, calculateTargetKindsOfItemsResult.TargetKindsOfItems, storagesList);

#if DEBUG
                Log($"newLogicalQueryNodes = {newLogicalQueryNodes.WriteListToString()}");
#endif

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private static readonly List<KindOfLogicalQueryNode> EmptyTargetKindsOfItems = new List<KindOfLogicalQueryNode>();
        private static LogicalQueryNodeEqualityComparer _logicalQueryNodeEqualityComparer = new LogicalQueryNodeEqualityComparer();

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

                        return (exceptList.Select(p => p.Kind).GroupBy(p => p).Select(p => new
                        {
                            Value = p.Key,
                            Count = p.Count()
                        }).OrderByDescending(p => p.Count).Select(p => p.Value).ToList(), replacingNotResultsStrategy);
                    }

                case ReplacingNotResultsStrategy.FirstPresentNextOtherKindOfItems:
                    {
                        if(exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                        }

                        return (exceptList.Select(p => p.Kind).GroupBy(p => p).Select(p => new
                        {
                            Value = p.Key,
                            Count = p.Count()
                        }).OrderByDescending(p => p.Count).Select(p => p.Value).ToList(), ReplacingNotResultsStrategy.AllKindOfItems);
                    }

                case ReplacingNotResultsStrategy.DominantKindOfItems:
                    {
                        if (exceptList.Count == 1)
                        {
                            return (exceptList.Select(p => p.Kind).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                        }

                        return (exceptList.Select(p => p.Kind).GroupBy(p => p).Select(p => new
                        {
                            Value = p.Key,
                            Count = p.Count()
                        }).OrderByDescending(p => p.Count).Select(p => p.Value).Take(1).ToList(), ReplacingNotResultsStrategy.PresentKindOfItems);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }
        }

        private List<LogicalQueryNode> GetLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            Log($"exceptList = {exceptList.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
            Log($"targetKindsOfItems = {targetKindsOfItems.WritePODListToString()}");
#endif

            return SortLogicalQueryNodes(GetRawLogicalQueryNodes(exceptList, replacingNotResultsStrategy, targetKindsOfItems, storagesList), replacingNotResultsStrategy, targetKindsOfItems);
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }
        }
    }
}
