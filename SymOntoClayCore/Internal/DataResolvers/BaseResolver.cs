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

using NLog;
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
    public class BaseResolver: BaseLoggedComponent
    {
        public BaseResolver(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        public Dictionary<StrongIdentifierValue, IStorage> GetSuperClassStoragesDict(IStorage storage, IInstance instance)
        {
            return GetStoragesList(storage, KindOfStoragesList.CodeItems).Select(p => p.Storage).Where(p => p.Kind == KindOfStorage.SuperClass && p.Instance == instance).ToDictionary(p => p.TargetClassName, p => p);
        }

        public List<StorageUsingOptions> GetStoragesList(IStorage storage, KindOfStoragesList kindOfStoragesList = KindOfStoragesList.Full)
        {
            return GetStoragesList(storage, null, kindOfStoragesList);
        }
        
        public List<StorageUsingOptions> GetStoragesList(IStorage storage, CollectChainOfStoragesOptions options, KindOfStoragesList kindOfStoragesList = KindOfStoragesList.Full)
        {
            switch(kindOfStoragesList)
            {
                case KindOfStoragesList.Full:
                    break;

                case KindOfStoragesList.CodeItems:
                    if(storage.CodeItemsStoragesList != null)
                    {
                        return storage.CodeItemsStoragesList;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStoragesList), kindOfStoragesList, null);
            }

            var result = new List<StorageUsingOptions>();

            var n = 0;

            if(options != null && options.InitialPriority.HasValue)
            {
                n = options.InitialPriority.Value;
            }

            var usedStorages = new List<IStorage>();

            storage.CollectChainOfStorages(result, usedStorages, n, options);

            switch (kindOfStoragesList)
            {
                case KindOfStoragesList.Full:
                    break;

                case KindOfStoragesList.CodeItems:
                    result = result.Where(p => p.Storage.Kind != KindOfStorage.WorldPublicFacts && p.Storage.Kind != KindOfStorage.Query && p.Storage.Kind != KindOfStorage.PerceptedFacts && p.Storage.Kind != KindOfStorage.PublicFacts && p.Storage.Kind != KindOfStorage.Sentence).ToList();
                    storage.CodeItemsStoragesList = result;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStoragesList), kindOfStoragesList, null);
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterCodeItems<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, LocalCodeExecutionContext localCodeExecutionContext)
            where T : CodeItem
        {
            return FilterCodeItems<T>(source, localCodeExecutionContext.Holder, localCodeExecutionContext);
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterCodeItems<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext)
            where T : CodeItem
        {
            if (!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            source = Filter(source);

            if (!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

#if DEBUG
            //Log($"holder = {holder}");
#endif

            var holderIsEntity = holder.KindOfName == KindOfName.Entity;

#if DEBUG
            //Log($"holderIsEntity = {holderIsEntity}");
#endif

            var hasHolderInItems = source.Any(p => p.ResultItem.Holder == holder);

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

#if DEBUG
            //Log($"hasHolderInItems = {hasHolderInItems}");
#endif

            foreach (var item in source)
            {
                var resultItem = item.ResultItem;

#if DEBUG
                //Log($"item = {item}");
                //Log($"resultItem.TypeOfAccess = {resultItem.TypeOfAccess}");
                //Log($"resultItem.Holder = {resultItem.Holder}");
#endif

                if (IsFitByTypeOfAccess(resultItem, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, false))
                {
                    result.Add(item);
                }
            }

#if DEBUG
            //Log($"result.Count = {result.Count}");
#endif

            //throw new NotImplementedException();

            return result;
        }

        public List<T> FilterByTypeOfAccess<T>(IList<T> source, LocalCodeExecutionContext localCodeExecutionContext, bool allowUnknown)
            where T : IReadOnlyMemberAccess
        {
            var result = new List<T>();

            var holder = localCodeExecutionContext.Holder;

#if DEBUG
            //Log($"holder = {holder}");
#endif

            var holderIsEntity = holder.KindOfName == KindOfName.Entity;

#if DEBUG
            //Log($"holderIsEntity = {holderIsEntity}");
#endif

            StrongIdentifierValue additionalHolder = null;
            var additionalHolderIsEntity = false;
            var hasAdditionalHolderInItems = false;

            var hasHolderInItems = source.Any(p => p.Holder == holder);

#if DEBUG
            //Log($"hasHolderInItems = {hasHolderInItems}");
#endif

            if (!holderIsEntity)
            {
                var allStateNamesList = _context.Storage.GlobalStorage.StatesStorage.AllStateNames();

#if DEBUG
                //Log($"allStateNamesList = {allStateNamesList.WriteListToString()}");
#endif

                if (allStateNamesList.Any(p => p == holder))
                {
                    additionalHolder = _context.InstancesStorage.MainEntity?.Name;

                    if (additionalHolder != null)
                    {
                        additionalHolderIsEntity = additionalHolder.KindOfName == KindOfName.Entity;
                        hasAdditionalHolderInItems = source.Any(p => p.Holder == additionalHolder);
                    }
                }
            }

#if DEBUG
            //Log($"additionalHolder = {additionalHolder}");
            //Log($"additionalHolderIsEntity = {additionalHolderIsEntity}");
            //Log($"hasAdditionalHolderInItems = {hasAdditionalHolderInItems}");
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            foreach (var item in source)
            {
#if DEBUG
                //Log($"item = {item}");
                //Log($"item.TypeOfAccess = {item.TypeOfAccess}");
                //Log($"item.Holder = {item.Holder}");
#endif

                if(IsFitByTypeOfAccess(item, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, allowUnknown))
                {
                    result.Add(item);
                }
                else
                {
                    if(additionalHolder != null)
                    {
                        if (IsFitByTypeOfAccess(item, additionalHolder, inheritanceResolver, localCodeExecutionContext, additionalHolderIsEntity, hasAdditionalHolderInItems, allowUnknown))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

#if DEBUG
            //Log($"result.Count = {result.Count}");
#endif

            //throw new NotImplementedException();

            return result;
        }

        private bool IsFitByTypeOfAccess(IReadOnlyMemberAccess item, StrongIdentifierValue holder, InheritanceResolver inheritanceResolver, LocalCodeExecutionContext localCodeExecutionContext, bool holderIsEntity, bool hasHolderInItems, bool allowUnknown)
        {
            var typeOfAccess = item.TypeOfAccess;

#if DEBUG
            //Log($"typeOfAccess = {typeOfAccess}");
            //Log($"holder = {holder}");
#endif

            switch (typeOfAccess)
            {
                case TypeOfAccess.Private:
                    if (holder == item.Holder)
                    {
                        return true;
                    }

                    if (holderIsEntity)
                    {
                        if (hasHolderInItems)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            var distance = inheritanceResolver.GetDistance(holder, item.Holder, localCodeExecutionContext);

#if DEBUG
                            //Log($"distance = {distance}");
#endif

                            if (distance == 1)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case TypeOfAccess.Protected:
                    {
#if DEBUG
                        //Log($"inheritanceResolver != null = {inheritanceResolver != null}");
                        //Log($"holder?.NameValue = {holder?.NameValue}");
                        //Log($"item.Holder?.NameValue = {item.Holder?.NameValue}");
#endif

                        if(holder == item.Holder)
                        {
                            return true;
                        }

                        var rank = inheritanceResolver.GetRawInheritanceRank(holder, item.Holder, localCodeExecutionContext);

#if DEBUG
                        //Log($"rank = {rank}");
#endif

                        if (rank > 0)
                        {
                            return true;
                        }
                    }
                    break;

                case TypeOfAccess.Public:
                case TypeOfAccess.Local:
                    return true;

                case TypeOfAccess.Unknown:
                    if (allowUnknown)
                    {
                        return true;
                    }
                    throw new ArgumentOutOfRangeException(nameof(typeOfAccess), typeOfAccess, null);

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeOfAccess), typeOfAccess, null);
            }

            return false;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> Filter<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T: AnnotatedItem
        {
            if(!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach(var filteredItem in source)
            {
                if(!filteredItem.ResultItem.HasConditionalSections)
                {
                    result.Add(filteredItem);
                    continue;
                }

                throw new NotImplementedException();
            }

            return result;
        }

        protected List<T> Filter<T>(List<T> source)
            where T : AnnotatedItem
        {
            if (!source.Any())
            {
                return new List<T>();
            }

            var result = new List<T>();

            foreach (var filteredItem in source)
            {
                if (!filteredItem.HasConditionalSections)
                {
                    result.Add(filteredItem);
                    continue;
                }

                throw new NotImplementedException();
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> OrderAndDistinctByInheritance<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, ResolverOptions options)
            where T : AnnotatedItem
        {
            if(options.JustDistinct)
            {
                return source;
            }

            return source.OrderByDescending(p => p.IsSelf).ThenByDescending(p => p.Rank).ThenBy(p => p.Distance).ToList();
        }

        protected virtual T ChooseTargetItem<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source) 
            where T : AnnotatedItem
        {
            if(!source.Any())
            {
                return default;
            }

            if(source.Count == 1)
            {
                return source.Single().ResultItem;
            }

            throw new NotImplementedException();
        }
    }
}
