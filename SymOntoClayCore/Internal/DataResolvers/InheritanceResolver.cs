/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class InheritanceResolver : BaseResolver
    {
        public ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        public InheritanceResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly SynonymsResolver _synonymsResolver;

        public const uint SelfDistance = 0u;
        public const uint TopTypeDistance = uint.MaxValue;

        public Value GetInheritanceRank(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetInheritanceRank(logger, subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public Value GetInheritanceRank(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new LogicalValue(GetRawInheritanceRank(logger, subName, superName, localCodeExecutionContext, options));
            return result;
        }

        public float GetRawInheritanceRank(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRawInheritanceRank(logger, subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public float GetRawInheritanceRank(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, subName, localCodeExecutionContext, options);

            if (!weightedInheritanceItemsList.Any())
            {
                return 0;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

            if (!targetWeightedInheritanceItemsList.Any())
            {
                return 0;
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                return targetWeightedInheritanceItemsList.First().Rank;
            }

            throw new NotImplementedException();
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetSuperClassesKeysList(logger, subName, localCodeExecutionContext, DefaultOptions);
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(logger, subName, localCodeExecutionContext, options).Select(p => p.SuperName).Where(p => !p.IsEmpty).Distinct().ToList();
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(logger, localCodeExecutionContext.Holder, localCodeExecutionContext, options);
        }
        
        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(logger, subName, localCodeExecutionContext, DefaultOptions);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var rawResult = new Dictionary<StrongIdentifierValue, WeightedInheritanceItem>();

            if(options == null || options.SkipRealSearching == false)
            {
                GetWeightedInheritanceItemsBySubName(logger, subName, localCodeExecutionContext, rawResult, 1, 0, storagesList, !(options?.OnlyDirectInheritance ?? false));
            }

            foreach (var resultItem in rawResult.ToList())
            {
                var synonymsList = _synonymsResolver.GetSynonyms(logger, resultItem.Key, storagesList);

                if(!synonymsList.IsNullOrEmpty())
                {
                    var rank = resultItem.Value.Rank;
                    var distance = resultItem.Value.Distance;

                    foreach (var synonym in synonymsList)
                    {
                        if (rawResult.ContainsKey(synonym))
                        {
                            var existingResultItem = rawResult[synonym];

                            if(existingResultItem.Rank < rank)
                            {
                                existingResultItem.Rank = rank;
                            }

                            if(existingResultItem.Distance < distance)
                            {
                                existingResultItem.Distance = distance;
                            }
                        }
                        else
                        {
                            var newResultItem = new WeightedInheritanceItem();
                            newResultItem.Rank = rank;
                            newResultItem.Distance = distance;
                            newResultItem.SuperName = synonym;

                            rawResult[synonym] = newResultItem;
                        }
                    }
                }
            }

            var result = rawResult.Select(p => p.Value).ToList();

            if(options.AddTopType)
            {
                result.Add(GetTopTypeWeightedInheritanceItem(logger, subName));
            }            

            if(options.AddSelf)
            {
                result.Add(GetSelfWeightedInheritanceItem(logger, subName));
            }

            return OrderAndDistinct(logger, result, localCodeExecutionContext, options);
        }

        private WeightedInheritanceItem GetTopTypeWeightedInheritanceItem(IMonitorLogger logger, StrongIdentifierValue subName)
        {
            var topTypeName = _context.CommonNamesStorage.DefaultHolder;

            var item = new WeightedInheritanceItem();
            item.SuperName = topTypeName;
            item.Rank = 0.1F;
            item.Distance = TopTypeDistance;

            return item;
        }

        public static WeightedInheritanceItem GetSelfWeightedInheritanceItem(IMonitorLogger logger, StrongIdentifierValue subName)
        {
            var item = new WeightedInheritanceItem();
            item.SuperName = subName;
            item.Rank = 1F;
            item.Distance = SelfDistance;
            item.IsSelf = true;

            return item;
        }

        private List<WeightedInheritanceItem> OrderAndDistinct(IMonitorLogger logger, List<WeightedInheritanceItem> source, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return source.OrderByDescending(p => p.IsSelf).ToList();
        }

        private void GetWeightedInheritanceItemsBySubName(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext, Dictionary<StrongIdentifierValue, WeightedInheritanceItem> result, float currentRank, uint currentDistance, List<StorageUsingOptions> storagesList, bool allInheritance)
        {
            currentDistance++;
            var rawList = GetRawList(logger, subName, storagesList);

            if(!rawList.Any())
            {
                return;
            }

            var filteredList = Filter(logger, rawList);

            if(!filteredList.Any())
            {
                return;
            }

            foreach (var filteredItem in filteredList)
            {
                var targetItem = filteredItem.ResultItem;

                var resolvedRankValue = _logicalValueLinearResolver.Resolve(logger, targetItem.Rank, localCodeExecutionContext, ResolverOptions.GetDefaultOptions(), true);

                var systemValue = resolvedRankValue.SystemValue;

                if(!systemValue.HasValue)
                {
                    systemValue = 0.5f;
                }

                var calculatedRank = currentRank * systemValue.Value;

                if(calculatedRank == 0)
                {
                    continue;
                }

                var superName = targetItem.SuperName;

                if (result.ContainsKey(superName))
                {
                    var item = result[superName];

                    if(item.Rank < calculatedRank)
                    {
                        item.Distance = currentDistance;
                        item.Rank = calculatedRank;
                        item.OriginalItem = targetItem;
                    }

                }
                else
                {
                    var item = new WeightedInheritanceItem();
                    item.SuperName = superName;
                    item.Distance = currentDistance;
                    item.Rank = calculatedRank;
                    item.OriginalItem = targetItem;

                    result[superName] = item;
                }

                if(allInheritance)
                {
                    GetWeightedInheritanceItemsBySubName(logger, targetItem.SuperName, localCodeExecutionContext, result, calculatedRank, currentDistance, storagesList, true);
                }                
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>> GetRawList(IMonitorLogger logger, StrongIdentifierValue subName, List<StorageUsingOptions> storagesList)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.InheritanceStorage.GetItemsDirectly(logger, subName);

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

        public bool IsFit(IMonitorLogger logger, IList<StrongIdentifierValue> typeNamesList, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(GetDistance(logger, typeNamesList, value, localCodeExecutionContext, options).HasValue)
            {
                return true;
            }

            return false;
        }

        public uint? GetDistance(IMonitorLogger logger, IList<StrongIdentifierValue> typeNamesList, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(typeNamesList.IsNullOrEmpty())
            {
                return TopTypeDistance;
            }

            value.CheckDirty();

            var distancesList = new List<uint>();

            foreach(var typeName in typeNamesList)
            {
                var typeDistance = GetDistance(logger, typeName, value, localCodeExecutionContext, options);

                if(typeDistance.HasValue)
                {
                    distancesList.Add(typeDistance.Value);
                }
            }

            if(!distancesList.Any())
            {
                return null;
            }

            return distancesList.Min();
        }

        public uint? GetDistance(IMonitorLogger logger, StrongIdentifierValue typeName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var biltInSuperTypesList = value.BuiltInSuperTypes;

            if(biltInSuperTypesList.Contains(typeName))
            {
                return 0u;
            }

            var distancesList = new List<uint>();

            var newOptions = options.Clone();
            newOptions.AddTopType = false;
            newOptions.AddSelf = false;

            foreach (var buildInSuperType in biltInSuperTypesList)
            {
                var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, buildInSuperType, localCodeExecutionContext, newOptions);

                if(!weightedInheritanceItemsList.Any())
                {
                    return null;
                }

                var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(typeName)).ToList();

                if (!targetWeightedInheritanceItemsList.Any())
                {
                    return null;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public uint? GetDistance(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetDistance(logger, subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public uint? GetDistance(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, subName, localCodeExecutionContext, options);

            if (!weightedInheritanceItemsList.Any())
            {
                return null;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

            if (!targetWeightedInheritanceItemsList.Any())
            {
                return null;
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                return targetWeightedInheritanceItemsList.First().Distance;
            }

            throw new NotImplementedException();
        }
    }
}
