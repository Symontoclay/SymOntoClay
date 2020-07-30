using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class InheritanceResolver : BaseResolver
    {
        public InheritanceResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedValue GetInheritanceRank(IndexedStrongIdentifierValue subName, IndexedStrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
#endif

            var weightedInheritanceItemsList = GetWeightedInheritanceItems(subName, localCodeExecutionContext, options);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

            if(!weightedInheritanceItemsList.Any())
            {
                var result = new LogicalValue(0);
                return result.GetIndexedValue(_context);
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

#if DEBUG
            //Log($"targetWeightedInheritanceItemsList = {targetWeightedInheritanceItemsList.WriteListToString()}");
#endif

            if (!targetWeightedInheritanceItemsList.Any())
            {
                var result = new LogicalValue(0);
                return result.GetIndexedValue(_context);
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                var result = new LogicalValue(targetWeightedInheritanceItemsList.First().Rank);
                
                return result.GetIndexedValue(_context);
            }

            throw new NotImplementedException();
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(localCodeExecutionContext.Holder, localCodeExecutionContext, options);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IndexedStrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var rawResult = new Dictionary<IndexedStrongIdentifierValue, WeightedInheritanceItem>();

            GetWeightedInheritanceItemsBySuperName(subName, localCodeExecutionContext, rawResult, 1, 0, storagesList);

#if DEBUG
            //Log($"rawResult.Count = {rawResult.Count}");
            //foreach (var resultItem in rawResult)
            //{
            //    Log($"resultItem.Key = {resultItem.Key}");
            //    Log($"resultItem.Value.Rank = {resultItem.Value.Rank}");
            //    Log($"resultItem.Value.Distance = {resultItem.Value.Distance}");
            //}
#endif

            var result = rawResult.Select(p => p.Value).ToList();

            result.Add(GetTopTypeWeightedInheritanceItem(subName));

            if(options.AddSelf)
            {
                result.Add(GetSelfWeightedInheritanceItem(subName));
            }

            return OrderAndDistinct(result, localCodeExecutionContext, options);
        }

        private WeightedInheritanceItem GetTopTypeWeightedInheritanceItem(IndexedStrongIdentifierValue subName)
        {
            var topTypeName = _context.CommonNamesStorage.IndexedDefaultHolder;

            var item = new WeightedInheritanceItem();
            item.SuperName = topTypeName;
            item.Rank = 0.1F;
            item.Distance = uint.MaxValue;

            return item;
        }

        private WeightedInheritanceItem GetSelfWeightedInheritanceItem(IndexedStrongIdentifierValue subName)
        {
            var item = new WeightedInheritanceItem();
            item.SuperName = subName;
            item.Rank = 1F;
            item.Distance = 0u;
            item.IsSelf = true;

            return item;
        }

        private List<WeightedInheritanceItem> OrderAndDistinct(List<WeightedInheritanceItem> source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return source.OrderByDescending(p => p.IsSelf).ToList();
        }

        private void GetWeightedInheritanceItemsBySuperName(IndexedStrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, Dictionary<IndexedStrongIdentifierValue, WeightedInheritanceItem> result, float currentRank, uint currentDistance, List<KeyValuePair<uint, IStorage>> storagesList)
        {
#if DEBUG
            //Log($"subName = {subName}");
            //Log($"currentRank = {currentRank}");
            //Log($"currentDistance = {currentDistance}");
            //Log($"result.Count = {result.Count}");
            //foreach(var resultItem in result)
            //{
            //    Log($"resultItem.Key = {resultItem.Key}");
            //    Log($"resultItem.Value.Rank = {resultItem.Value.Rank}");
            //    Log($"resultItem.Value.Distance = {resultItem.Value.Distance}");
            //}
#endif

            currentDistance++;
#if DEBUG
            //Log($"currentDistance (after) = {currentDistance}");
#endif

            var rawList = GetRawList(subName, storagesList);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            var logicalValueLinearResolver = _context.DataResolversFactory.GetLogicalValueLinearResolver();

            foreach (var filteredItem in filteredList)
            {
                var targetItem = filteredItem.ResultItem;

#if DEBUG
                //Log($"targetItem = {targetItem}");
#endif

                var resolvedRankValue = logicalValueLinearResolver.Resolve(targetItem.Rank, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

                var systemValue = resolvedRankValue.SystemValue;

                if(!systemValue.HasValue)
                {
                    systemValue = 0.5f;
                }

                var calculatedRank = currentRank * systemValue.Value;

#if DEBUG
                //Log($"calculatedRank = {calculatedRank}");
#endif

                if(calculatedRank == 0)
                {
                    continue;
                }

                var superName = targetItem.SuperName;

#if DEBUG
                //Log($"superName = {superName}");
#endif

                if (result.ContainsKey(superName))
                {
                    var item = result[superName];

                    if(item.Rank < calculatedRank)
                    {
                        item.Distance = currentDistance;
                        item.Rank = calculatedRank;
                        item.OriginalIndexedItem = targetItem;
                    }

#if DEBUG
                    //Log($"item (w 1)= {item}");
#endif
                }
                else
                {
                    var item = new WeightedInheritanceItem();
                    item.SuperName = superName;
                    item.Distance = currentDistance;
                    item.Rank = calculatedRank;
                    item.OriginalIndexedItem = targetItem;

#if DEBUG
                    //Log($"item (w 2) = {item}");
#endif

                    result[superName] = item;
                }

                GetWeightedInheritanceItemsBySuperName(targetItem.SuperName, localCodeExecutionContext, result, calculatedRank, currentDistance, storagesList);
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>> GetRawList(IndexedStrongIdentifierValue subName, List<KeyValuePair<uint, IStorage>> storagesList)
        {
#if DEBUG
            //Log($"subName = {subName}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Value.InheritanceStorage.GetItemsDirectly(subName);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Key;
                var storage = storageItem.Value;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
