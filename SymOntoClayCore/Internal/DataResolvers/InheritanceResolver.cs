/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

        public Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetInheritanceRank(subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new LogicalValue(GetRawInheritanceRank(subName, superName, localCodeExecutionContext, options));
            return result;
        }

        public float GetRawInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRawInheritanceRank(subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public float GetRawInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
#endif

            var weightedInheritanceItemsList = GetWeightedInheritanceItems(subName, localCodeExecutionContext, options);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

            if (!weightedInheritanceItemsList.Any())
            {
                return 0;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

#if DEBUG
            //Log($"targetWeightedInheritanceItemsList = {targetWeightedInheritanceItemsList.WriteListToString()}");
#endif

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

        public List<StrongIdentifierValue> GetSuperClassesKeysList(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetSuperClassesKeysList(subName, localCodeExecutionContext, DefaultOptions);
        }

        public List<StrongIdentifierValue> GetSuperClassesKeysList(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return GetWeightedInheritanceItems(subName, localCodeExecutionContext, options).Select(p => p.SuperName).Where(p => !p.IsEmpty).Distinct().ToList();
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
            return GetWeightedInheritanceItems(subName, localCodeExecutionContext, DefaultOptions);
        }

        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Storage = {tmpStorage.Storage}");
            //}
#endif

            var rawResult = new Dictionary<StrongIdentifierValue, WeightedInheritanceItem>();

            if(options == null || options.SkipRealSearching == false)
            {
                GetWeightedInheritanceItemsBySubName(subName, localCodeExecutionContext, rawResult, 1, 0, storagesList);
            }

#if DEBUG
//            Log($"rawResult.Count = {rawResult.Count}");
//            foreach (var resultItem in rawResult)
//            {
//                Log($"resultItem.Key = {resultItem.Key}");
//                Log($"resultItem.Value.Rank = {resultItem.Value.Rank}");
//                Log($"resultItem.Value.Distance = {resultItem.Value.Distance}");
//            }
#endif

            foreach (var resultItem in rawResult.ToList())
            {
#if DEBUG
                //Log($"resultItem.Key = {resultItem.Key}");
#endif

                var synonymsList = _synonymsResolver.GetSynonyms(resultItem.Key, storagesList);

#if DEBUG
                //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

                if(!synonymsList.IsNullOrEmpty())
                {
                    var rank = resultItem.Value.Rank;
                    var distance = resultItem.Value.Distance;

                    foreach (var synonym in synonymsList)
                    {
#if DEBUG
                        //Log($"synonym = {synonym}");
#endif

                        if (rawResult.ContainsKey(synonym))
                        {
#if DEBUG
                            //Log("rawResult.ContainsKey(synonym)");
#endif

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
#if DEBUG
                            //Log("not rawResult.ContainsKey(synonym)");
#endif

                            var newResultItem = new WeightedInheritanceItem();
                            newResultItem.Rank = rank;
                            newResultItem.Distance = distance;
                            newResultItem.SuperName = synonym;

                            rawResult[synonym] = newResultItem;
                        }
                    }
                }
            }

#if DEBUG
            //Log($"rawResult.Count (after) = {rawResult.Count}");
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
            item.Distance = TopTypeDistance;

            return item;
        }

        private WeightedInheritanceItem GetSelfWeightedInheritanceItem(StrongIdentifierValue subName)
        {
            var item = new WeightedInheritanceItem();
            item.SuperName = subName;
            item.Rank = 1F;
            item.Distance = SelfDistance;
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
            //Log($"subName = {subName}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
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

            if(!rawList.Any())
            {
                return;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if(!filteredList.Any())
            {
                return;
            }

            foreach (var filteredItem in filteredList)
            {
                var targetItem = filteredItem.ResultItem;

#if DEBUG
                //Log($"targetItem = {targetItem}");
#endif

                var resolvedRankValue = _logicalValueLinearResolver.Resolve(targetItem.Rank, localCodeExecutionContext, ResolverOptions.GetDefaultOptions(), true);

#if DEBUG
                //Log($"resolvedRankValue = {resolvedRankValue}");
#endif

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
            //Log($"subName = {subName}");
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

        public bool IsFit(IList<StrongIdentifierValue> typeNamesList, Value value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(GetDistance(typeNamesList, value, localCodeExecutionContext, options).HasValue)
            {
                return true;
            }

            return false;
        }

        public uint? GetDistance(IList<StrongIdentifierValue> typeNamesList, Value value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"typeNamesList = {typeNamesList.WriteListToString()}");
            //Log($"value = {value}");
#endif

            if(typeNamesList.IsNullOrEmpty())
            {
                return TopTypeDistance;
            }

            value.CheckDirty();

            var distancesList = new List<uint>();

            foreach(var typeName in typeNamesList)
            {
                var typeDistance = GetDistance(typeName, value, localCodeExecutionContext, options);

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

        public uint? GetDistance(StrongIdentifierValue typeName, Value value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"typeName = {typeName}");
            //Log($"value = {value}");
            //if(value.KindOfValue == KindOfValue.StrongIdentifierValue)
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var biltInSuperTypesList = value.BuiltInSuperTypes;

#if DEBUG
            //Log($"biltInSuperTypesList = {biltInSuperTypesList.WriteListToString()}");
#endif

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
#if DEBUG
                //Log($"typeName = {typeName}");
                //Log($"buildInSuperType = {buildInSuperType}");
#endif

#if DEBUG
                //Log($"options = {options}");
#endif

                var weightedInheritanceItemsList = GetWeightedInheritanceItems(buildInSuperType, localCodeExecutionContext, newOptions);

#if DEBUG
                //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

                if(!weightedInheritanceItemsList.Any())
                {
                    return null;
                }

                var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(typeName)).ToList();

#if DEBUG
                //Log($"targetWeightedInheritanceItemsList = {targetWeightedInheritanceItemsList.WriteListToString()}");
#endif

                if (!targetWeightedInheritanceItemsList.Any())
                {
                    return null;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public uint? GetDistance(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetDistance(subName, superName, localCodeExecutionContext, DefaultOptions);
        }

        public uint? GetDistance(StrongIdentifierValue subName, StrongIdentifierValue superName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
#endif

            var weightedInheritanceItemsList = GetWeightedInheritanceItems(subName, localCodeExecutionContext, options);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

            if (!weightedInheritanceItemsList.Any())
            {
                return null;
            }

            var targetWeightedInheritanceItemsList = weightedInheritanceItemsList.Where(p => p.SuperName.Equals(superName)).ToList();

#if DEBUG
            //Log($"targetWeightedInheritanceItemsList = {targetWeightedInheritanceItemsList.WriteListToString()}");
#endif

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
