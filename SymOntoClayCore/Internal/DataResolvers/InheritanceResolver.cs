/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class InheritanceResolver : BaseResolver
    {
        public ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        public InheritanceResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private LogicalValueLinearResolver _logicalValueLinearResolver;
        private SynonymsResolver _synonymsResolver;

        public const uint SelfDistance = 0u;
        public const uint TopTypeDistance = uint.MaxValue;

        public Value GetInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetInheritanceRank(logger, subType, superType, localCodeExecutionContext, DefaultOptions);
        }

        public Value GetInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new LogicalValue(GetRawInheritanceRank(logger, subType, superType, localCodeExecutionContext, options));
            return result;
        }

        public float GetRawInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRawInheritanceRank(logger, subType, superType, localCodeExecutionContext, DefaultOptions);
        }

        public float GetRawInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, subType, localCodeExecutionContext, options);

            if (!weightedInheritanceItemsList.Any())
            {
                return 0;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperType.Equals(superType)).ToList();

            if (!targetWeightedInheritanceItemsList.Any())
            {
                return 0;
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                return targetWeightedInheritanceItemsList.First().Rank;
            }

            throw new NotImplementedException("22CC8B8D-9833-421D-A9EA-62F79D0EBD90");
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetSuperClassesKeysList(logger, subName, localCodeExecutionContext, DefaultOptions);
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(IMonitorLogger logger, StrongIdentifierValue subName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(logger, subName.ToTypeInfo(), localCodeExecutionContext, options).Select(p => p.SuperType.Name).Where(p => !p.IsEmpty).Distinct().ToList();
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(logger, localCodeExecutionContext.Holder.ToTypeInfo(), localCodeExecutionContext, options);
        }
        
        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, TypeInfo subType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetWeightedInheritanceItems(logger, subType, localCodeExecutionContext, DefaultOptions);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, TypeInfo subType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var rawResult = new Dictionary<TypeInfo, WeightedInheritanceItem>();

            if(options == null || options.SkipRealSearching == false)
            {
                FillUpWeightedInheritanceItemsBySubName(logger, subType, localCodeExecutionContext, rawResult, 1, 0, storagesList, !(options?.OnlyDirectInheritance ?? false));
            }

            foreach (var resultItem in rawResult.ToList())
            {
                var synonymsList = _synonymsResolver.GetSynonyms(logger, resultItem.Key.Name, storagesList);

                if(!synonymsList.IsNullOrEmpty())
                {
                    var rank = resultItem.Value.Rank;
                    var distance = resultItem.Value.Distance;

                    foreach (var synonym in synonymsList)
                    {
                        var synonymTypeInfo = synonym.ToTypeInfo();

                        if (rawResult.ContainsKey(synonymTypeInfo))
                        {
                            var existingResultItem = rawResult[synonymTypeInfo];

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
                            newResultItem.SuperType = synonymTypeInfo;

                            rawResult[synonymTypeInfo] = newResultItem;
                        }
                    }
                }
            }

            var result = rawResult.Select(p => p.Value).ToList();

            if(options.AddTopType)
            {
                result.Add(GetTopTypeWeightedInheritanceItem(logger, subType));
            }            

            if(options.AddSelf)
            {
                result.Add(GetSelfWeightedInheritanceItem(logger, subType));
            }

            return OrderAndDistinct(logger, result, localCodeExecutionContext, options);
        }

        private WeightedInheritanceItem GetTopTypeWeightedInheritanceItem(IMonitorLogger logger, TypeInfo subType)
        {
            var topTypeName = _context.CommonNamesStorage.DefaultHolder.ToTypeInfo();

            var item = new WeightedInheritanceItem();
            item.SuperType = topTypeName;
            item.Rank = 0.1F;
            item.Distance = TopTypeDistance;

            return item;
        }

        public static WeightedInheritanceItem GetSelfWeightedInheritanceItem(IMonitorLogger logger, TypeInfo subType)
        {
            var item = new WeightedInheritanceItem();
            item.SuperType = subType;
            item.Rank = 1F;
            item.Distance = SelfDistance;
            item.IsSelf = true;

            return item;
        }

        private List<WeightedInheritanceItem> OrderAndDistinct(IMonitorLogger logger, List<WeightedInheritanceItem> source, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return source.OrderByDescending(p => p.IsSelf).ToList();
        }

        private void FillUpWeightedInheritanceItemsBySubName(IMonitorLogger logger, TypeInfo subType, ILocalCodeExecutionContext localCodeExecutionContext, Dictionary<TypeInfo, WeightedInheritanceItem> result, float currentRank, uint currentDistance, List<StorageUsingOptions> storagesList, bool allInheritance)
        {
            currentDistance++;
            var rawList = GetRawList(logger, subType, storagesList);

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

                var resolvedRankValue = _logicalValueLinearResolver.Resolve(logger, targetItem.Rank, localCodeExecutionContext, true);

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

                var superType = targetItem.SuperType;

                if (result.ContainsKey(superType))
                {
                    var item = result[superType];

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
                    item.SuperType = superType;
                    item.Distance = currentDistance;
                    item.Rank = calculatedRank;
                    item.OriginalItem = targetItem;

                    result[superType] = item;
                }

                if(allInheritance)
                {
                    FillUpWeightedInheritanceItemsBySubName(logger, targetItem.SuperType, localCodeExecutionContext, result, calculatedRank, currentDistance, storagesList, true);
                }
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>> GetRawList(IMonitorLogger logger, TypeInfo subType, List<StorageUsingOptions> storagesList)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InheritanceItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.InheritanceStorage.GetItemsDirectly(logger, subType);

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

        public bool IsFit(IMonitorLogger logger, IList<TypeInfo> typeInfoList, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return IsFit(logger, typeInfoList, value, localCodeExecutionContext, DefaultOptions);
        }

        public bool IsFit(IMonitorLogger logger, IList<TypeInfo> typeInfoList, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(GetDistance(logger, typeInfoList, value, localCodeExecutionContext, options).HasValue)
            {
                return true;
            }

            return false;
        }

        public uint? GetDistance(IMonitorLogger logger, IList<TypeInfo> typeInfoList, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(typeInfoList.IsNullOrEmpty())
            {
                return TopTypeDistance;
            }

            value.CheckDirty();

            var distancesList = new List<uint>();

            foreach(var typeName in typeInfoList)
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

        public uint? GetDistance(IMonitorLogger logger, TypeInfo typeInfo, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var builtInSuperTypesList = value.BuiltInSuperTypes;

            if(builtInSuperTypesList.Contains(typeInfo))
            {
                return 0u;
            }

            var distancesList = new List<uint>();

            var newOptions = options.Clone();
            newOptions.AddTopType = false;
            newOptions.AddSelf = false;

            foreach (var builtInSuperType in builtInSuperTypesList)
            {
                var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, builtInSuperType, localCodeExecutionContext, newOptions);

                if(!weightedInheritanceItemsList.Any())
                {
                    return null;
                }

                var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperType.Equals(typeInfo)).ToList();

                if (!targetWeightedInheritanceItemsList.Any())
                {
                    return null;
                }

                throw new NotImplementedException("8C2EE250-62DE-478D-BE1F-82098D9676C0");
            }

            throw new NotImplementedException("7379B454-D4D8-4F88-B1C3-5708CBF5338F");
        }

        public uint? GetDistance(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetDistance(logger, subType, superType, localCodeExecutionContext, DefaultOptions);
        }

        public uint? GetDistance(IMonitorLogger logger, TypeInfo subType, TypeInfo superType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var weightedInheritanceItemsList = GetWeightedInheritanceItems(logger, subType, localCodeExecutionContext, options);

            if (!weightedInheritanceItemsList.Any())
            {
                return null;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperType.Equals(superType)).ToList();

            if (!targetWeightedInheritanceItemsList.Any())
            {
                return null;
            }

            if (targetWeightedInheritanceItemsList.Count == 1)
            {
                return targetWeightedInheritanceItemsList.First().Distance;
            }

            throw new NotImplementedException("8A0D58DA-A954-41AF-AAB6-A7FF9FB6830E");
        }
    }
}
