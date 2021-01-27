/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        private static ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public InheritanceResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedValue GetInheritanceRank(IndexedStrongIdentifierValue subName, IndexedStrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetInheritanceRank(subName.NameKey, superName.NameKey, localCodeExecutionContext, options);
        }

        public IndexedValue GetInheritanceRank(ulong subNameKey, ulong superNameKey, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetInheritanceRank(subNameKey, superNameKey, localCodeExecutionContext, _defaultOptions);
        }

        public IndexedValue GetInheritanceRank(ulong subNameKey, ulong superNameKey, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
#endif

            var weightedInheritanceItemsList = GetWeightedInheritanceItems(subNameKey, localCodeExecutionContext, options);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

            if(!weightedInheritanceItemsList.Any())
            {
                var result = new LogicalValue(0);
                return result.GetIndexedValue(_context);
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperNameKey.Equals(superNameKey)).ToList();

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

        public List<ulong> GetSuperClassesKeysList(ulong subNameKey, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(subNameKey, localCodeExecutionContext).Select(p => p.SuperNameKey).Where(p => p != 0).Distinct().ToList();
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(localCodeExecutionContext.Holder.NameKey, localCodeExecutionContext, options);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IndexedStrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(subName.NameKey, localCodeExecutionContext, options);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(ulong subNameKey, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(subNameKey, localCodeExecutionContext, _defaultOptions);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(ulong subNameKey, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

            var rawResult = new Dictionary<ulong, WeightedInheritanceItem>();

            GetWeightedInheritanceItemsBySubName(subNameKey, localCodeExecutionContext, rawResult, 1, 0, storagesList);

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

            if(options.AddTopType)
            {
                result.Add(GetTopTypeWeightedInheritanceItem(subNameKey));
            }            

            if(options.AddSelf)
            {
                result.Add(GetSelfWeightedInheritanceItem(subNameKey));
            }

            return OrderAndDistinct(result, localCodeExecutionContext, options);
        }

        private WeightedInheritanceItem GetTopTypeWeightedInheritanceItem(ulong subNameKey)
        {
            var topTypeName = _context.CommonNamesStorage.IndexedDefaultHolder;

            var item = new WeightedInheritanceItem();
            item.SuperNameKey = topTypeName.NameKey;
            item.Rank = 0.1F;
            item.Distance = uint.MaxValue;

            return item;
        }

        private WeightedInheritanceItem GetSelfWeightedInheritanceItem(ulong subNameKey)
        {
            var item = new WeightedInheritanceItem();
            item.SuperNameKey = subNameKey;
            item.Rank = 1F;
            item.Distance = 0u;
            item.IsSelf = true;

            return item;
        }

        private List<WeightedInheritanceItem> OrderAndDistinct(List<WeightedInheritanceItem> source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return source.OrderByDescending(p => p.IsSelf).ToList();
        }

        private void GetWeightedInheritanceItemsBySubName(ulong subNameKey, LocalCodeExecutionContext localCodeExecutionContext, Dictionary<ulong, WeightedInheritanceItem> result, float currentRank, uint currentDistance, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"subName = {subNameKey}");
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

            var rawList = GetRawList(subNameKey, storagesList);

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

                var superNameKey = targetItem.SuperName.NameKey;

#if DEBUG
                //Log($"superName = {superName}");
#endif

                if (result.ContainsKey(superNameKey))
                {
                    var item = result[superNameKey];

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
                    item.SuperNameKey = superNameKey;
                    item.Distance = currentDistance;
                    item.Rank = calculatedRank;
                    item.OriginalIndexedItem = targetItem;

#if DEBUG
                    //Log($"item (w 2) = {item}");
#endif

                    result[superNameKey] = item;
                }

                GetWeightedInheritanceItemsBySubName(targetItem.SuperName.NameKey, localCodeExecutionContext, result, calculatedRank, currentDistance, storagesList);
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>> GetRawList(ulong subNameKey, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"subNameKey = {subNameKey}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.InheritanceStorage.GetItemsDirectly(subNameKey);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IndexedInheritanceItem>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
