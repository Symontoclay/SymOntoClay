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

        public Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetInheritanceRank(subName, superName, localCodeExecutionContext, _defaultOptions);
        }

        public Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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
                return result;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

#if DEBUG
            //Log($"targetWeightedInheritanceItemsList = {targetWeightedInheritanceItemsList.WriteListToString()}");
#endif

            if (!targetWeightedInheritanceItemsList.Any())
            {
                var result = new LogicalValue(0);
                return result;
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                var result = new LogicalValue(targetWeightedInheritanceItemsList.First().Rank);
                
                return result;
            }

            throw new NotImplementedException();
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(subName, localCodeExecutionContext).Select(p => p.SuperName).Where(p => !p.IsEmpty).Distinct().ToList();
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(localCodeExecutionContext.Holder, localCodeExecutionContext, options);
        }

        //public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        //{
        //    return GetWeightedInheritanceItems(subName.NormalizedNameValue, localCodeExecutionContext, options);
        //}

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(subName, localCodeExecutionContext, _defaultOptions);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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
            //    Log($"tmpStorage.Storage = {tmpStorage.Storage}");
            //}
#endif

            var rawResult = new Dictionary<StrongIdentifierValue, WeightedInheritanceItem>();

            GetWeightedInheritanceItemsBySubName(subName, localCodeExecutionContext, rawResult, 1, 0, storagesList);

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
                result.Add(GetTopTypeWeightedInheritanceItem(subName));
            }            

            if(options.AddSelf)
            {
                result.Add(GetSelfWeightedInheritanceItem(subName));
            }

            return OrderAndDistinct(result, localCodeExecutionContext, options);
        }

        private WeightedInheritanceItem GetTopTypeWeightedInheritanceItem(StrongIdentifierValue subName)
        {
            var topTypeName = _context.CommonNamesStorage.DefaultHolder;

            var item = new WeightedInheritanceItem();
            item.SuperName = topTypeName;
            item.Rank = 0.1F;
            item.Distance = uint.MaxValue;

            return item;
        }

        private WeightedInheritanceItem GetSelfWeightedInheritanceItem(StrongIdentifierValue subName)
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

        private void GetWeightedInheritanceItemsBySubName(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, Dictionary<StrongIdentifierValue, WeightedInheritanceItem> result, float currentRank, uint currentDistance, List<StorageUsingOptions> storagesList)
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
                        item.OriginalItem = targetItem;
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
                    item.OriginalItem = targetItem;

#if DEBUG
                    //Log($"item (w 2) = {item}");
#endif

                    result[superName] = item;
                }

                GetWeightedInheritanceItemsBySubName(targetItem.SuperName, localCodeExecutionContext, result, calculatedRank, currentDistance, storagesList);
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>> GetRawList(StrongIdentifierValue subName, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"subNameKey = {subNameKey}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.InheritanceStorage.GetItemsDirectly(subName);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
