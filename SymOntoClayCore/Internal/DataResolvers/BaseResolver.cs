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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class BaseResolver: BaseContextComponent
    {
        public BaseResolver(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        protected InheritanceResolver _inheritanceResolver;

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();
        }

        public Dictionary<StrongIdentifierValue, IStorage> GetSuperClassStoragesDict(IMonitorLogger logger, IStorage storage, IInstance instance)
        {
            return GetStoragesList(logger, storage, KindOfStoragesList.CodeItems).Select(p => p.Storage).Where(p => p.Kind == KindOfStorage.SuperClass && p.Instance == instance).ToDictionary(p => p.TargetClassName, p => p);
        }

        public List<StorageUsingOptions> GetStoragesList(IMonitorLogger logger, IStorage storage, KindOfStoragesList kindOfStoragesList = KindOfStoragesList.Full)
        {
            return GetStoragesList(logger, storage, null, kindOfStoragesList);
        }
        
        public List<StorageUsingOptions> GetStoragesList(IMonitorLogger logger, IStorage storage, CollectChainOfStoragesOptions options, KindOfStoragesList kindOfStoragesList = KindOfStoragesList.Full)
        {
            switch (kindOfStoragesList)
            {
                case KindOfStoragesList.Full:
                case KindOfStoragesList.CodeItems:
                    return NGetStoragesList(logger, storage, options, kindOfStoragesList);

                case KindOfStoragesList.Var:
                case KindOfStoragesList.Property:
                    return FilterStoragesForVarResolving(logger, NGetStoragesList(logger, storage, options, KindOfStoragesList.CodeItems));

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStoragesList), kindOfStoragesList, null);
            }
        }

        private List<StorageUsingOptions> FilterStoragesForVarResolving(IMonitorLogger logger, List<StorageUsingOptions> storagesList)
        {
            var result = new List<StorageUsingOptions>();

            var wasIsolatedStorage = false;

            foreach(var item in storagesList)
            {
                var storage = item.Storage;

#if DEBUG
                //Log("**************************************************");
                //Log($"wasIsolatedStorage = {wasIsolatedStorage}");
                //Log($"storage.Kind = {storage.Kind}");
                //Log($"storage.GetHashCode() = {storage.GetHashCode()}");
                //Log($"storage.TargetClassName = {storage.TargetClassName?.ToHumanizedString()}");
                //Log($"storage.IsIsolated = {storage.IsIsolated}");
                //storage.VarStorage.DbgPrintVariables();
#endif

                var kind = storage.Kind;

                switch (kind)
                {
                    case KindOfStorage.Local:
                        if(wasIsolatedStorage)
                        {
                            continue;
                        }
                        result.Add(item);
                        break;

                    case KindOfStorage.Action:
                    case KindOfStorage.State:
                    case KindOfStorage.Object:
                    case KindOfStorage.AppInstance:
                    case KindOfStorage.RootTaskInstance:
                    case KindOfStorage.StrategicTaskInstance:
                    case KindOfStorage.TacticalTaskInstance:
                    case KindOfStorage.CompoundTaskInstance:
                        if (wasIsolatedStorage)
                        {
                            continue;
                        }

                        if(storage.IsIsolated)
                        {
                            wasIsolatedStorage = true;
                        }
                        result.Add(item);
                        break;

                    case KindOfStorage.Global:
                    case KindOfStorage.Categories:
                    case KindOfStorage.World:
                    case KindOfStorage.SuperClass:
                        result.Add(item);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }

            return result;
        }

        private List<StorageUsingOptions> NGetStoragesList(IMonitorLogger logger, IStorage storage, CollectChainOfStoragesOptions options, KindOfStoragesList kindOfStoragesList)
        {
            switch (kindOfStoragesList)
            {
                case KindOfStoragesList.Full:
                    break;

                case KindOfStoragesList.CodeItems:
                    if (storage.CodeItemsStoragesList != null)
                    {
                        return storage.CodeItemsStoragesList;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStoragesList), kindOfStoragesList, null);
            }

            var result = new List<StorageUsingOptions>();

            var n = 0;

            if (options != null && options.InitialPriority.HasValue)
            {
                n = options.InitialPriority.Value;
            }

            var usedStorages = new List<IStorage>();

            storage.CollectChainOfStorages(logger, result, usedStorages, n, options);

            switch (kindOfStoragesList)
            {
                case KindOfStoragesList.Full:
                    break;

                case KindOfStoragesList.CodeItems:
                    result = result.Where(p => p.Storage.Kind != KindOfStorage.WorldPublicFacts && p.Storage.Kind != KindOfStorage.AdditionalPublicFacts && p.Storage.Kind != KindOfStorage.VisiblePublicFacts && p.Storage.Kind != KindOfStorage.BackpackStorage && p.Storage.Kind != KindOfStorage.Query && p.Storage.Kind != KindOfStorage.PerceptedFacts && p.Storage.Kind != KindOfStorage.PublicFacts && p.Storage.Kind != KindOfStorage.Sentence).ToList();
                    storage.CodeItemsStoragesList = result;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStoragesList), kindOfStoragesList, null);
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterCodeItems<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, ILocalCodeExecutionContext localCodeExecutionContext)
            where T : IFilteredCodeItem
        {
            return FilterCodeItems<T>(logger, source, localCodeExecutionContext.Holder, localCodeExecutionContext);
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterCodeItems<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext)
            where T : IFilteredCodeItem
        {
            if (!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            source = Filter(logger, source);

            if (!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            var holderIsEntity = holder.KindOfName == KindOfName.Entity;

            var hasHolderInItems = source.Any(p => p.ResultItem.Holder == holder);

            var inheritanceResolver = _inheritanceResolver;

            foreach (var item in source)
            {
                var resultItem = item.ResultItem;

                if (IsFitByTypeOfAccess(logger, resultItem, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, false))
                {
                    result.Add(item);
                }
            }


            return result;
        }

        public List<T> FilterByTypeOfAccess<T>(IMonitorLogger logger, IList<T> source, ILocalCodeExecutionContext localCodeExecutionContext, bool allowUnknown)
            where T : IReadOnlyMemberAccess
        {
            var holder = localCodeExecutionContext.Holder;

            if(holder == null)
            {
                return source.ToList();
            }

            var result = new List<T>();

            var holderIsEntity = holder.KindOfName == KindOfName.Entity;

            StrongIdentifierValue additionalHolder = null;
            var additionalHolderIsEntity = false;
            var hasAdditionalHolderInItems = false;

            var hasHolderInItems = source.Any(p => p.Holder == holder);

            if (!holderIsEntity)
            {
                var allStateNamesList = _context.Storage.GlobalStorage.StatesStorage.AllStateNames(logger);

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

            var inheritanceResolver = _inheritanceResolver;

            foreach (var item in source)
            {
                if(IsFitByTypeOfAccess(logger, item, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, allowUnknown))
                {
                    result.Add(item);
                }
                else
                {
                    if(additionalHolder != null)
                    {
                        if (IsFitByTypeOfAccess(logger, item, additionalHolder, inheritanceResolver, localCodeExecutionContext, additionalHolderIsEntity, hasAdditionalHolderInItems, allowUnknown))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private bool IsFitByTypeOfAccess(IMonitorLogger logger, IReadOnlyMemberAccess item, StrongIdentifierValue holder, InheritanceResolver inheritanceResolver, ILocalCodeExecutionContext localCodeExecutionContext, bool holderIsEntity, bool hasHolderInItems, bool allowUnknown)
        {
            var typeOfAccess = item.TypeOfAccess;

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
                            throw new NotImplementedException("D78D12DD-FAB1-44D3-BDAC-3DC3BB92C2D9");
                        }
                        else
                        {
                            var distance = inheritanceResolver.GetDistance(logger, holder, item.Holder, localCodeExecutionContext);

                            if (distance == 1)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("45425999-84F1-498A-9AD5-3EFC477E6456");
                    }
                    break;

                case TypeOfAccess.Protected:
                    {
                        if(holder == item.Holder)
                        {
                            return true;
                        }

                        var rank = inheritanceResolver.GetRawInheritanceRank(logger, holder, item.Holder, localCodeExecutionContext);

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

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> Filter<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T: IWeightedInheritanceResultItemParameter
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

                throw new NotImplementedException("E04C273B-6774-4FC2-8E42-DF45CC3FE7E4");
            }

            return result;
        }

        protected List<T> Filter<T>(IMonitorLogger logger, List<T> source)
            where T : IWeightedInheritanceResultItemParameter
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

                throw new NotImplementedException("549B1237-8743-44CB-9251-BBED0027C851");
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> OrderAndDistinctByInheritance<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, ResolverOptions options)
            where T : IWeightedInheritanceResultItemParameter
        {
            if(options.JustDistinct)
            {
                return source;
            }

            return source.OrderByDescending(p => p.IsSelf).ThenByDescending(p => p.Rank).ThenBy(p => p.Distance).ToList();
        }

        protected virtual T ChooseTargetItem<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source) 
            where T : IWeightedInheritanceResultItemParameter
        {
            if(!source.Any())
            {
                return default;
            }

            if(source.Count == 1)
            {
                return source.Single().ResultItem;
            }

            throw new NotImplementedException("ACD273A4-113C-40AD-9CA5-31F0AC779728");
        }
    }
}
