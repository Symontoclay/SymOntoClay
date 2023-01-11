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
    public abstract class BaseResolver: BaseLoggedComponent
    {
        protected BaseResolver(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        [Obsolete("Make the name none static.")]
        public static List<StorageUsingOptions> GetStoragesList(IStorage storage)
        {
            return GetStoragesList(storage, null);
        }

        [Obsolete("Make the name none static.")]
        public static List<StorageUsingOptions> GetStoragesList(IStorage storage, CollectChainOfStoragesOptions options)
        {
            var result = new List<StorageUsingOptions>();

            var n = 0;

            if(options != null && options.InitialPriority.HasValue)
            {
                n = options.InitialPriority.Value;
            }

            var usedStorages = new List<IStorage>();

            storage.CollectChainOfStorages(result, usedStorages, n, options);
            
            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterCodeItems<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, LocalCodeExecutionContext localCodeExecutionContext)
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

            var holder = localCodeExecutionContext.Holder;

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

                if (IsFitByTypeOfAccess(resultItem, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, false, _context.Logger))
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

        [Obsolete("Make the name none static.")]
        public static List<T> FilterByTypeOfAccess<T>(IList<T> source, IMainStorageContext context, LocalCodeExecutionContext localCodeExecutionContext, bool allowUnknown)
            where T : IReadOnlyMemberAccess
        {
            var result = new List<T>();

            var holder = localCodeExecutionContext.Holder;

#if DEBUG
            //context.Logger.Log($"holder = {holder}");
#endif

            var holderIsEntity = holder.KindOfName == KindOfName.Entity;

#if DEBUG
            //context.Logger.Log($"holderIsEntity = {holderIsEntity}");
#endif

            StrongIdentifierValue additionalHolder = null;
            var additionalHolderIsEntity = false;
            var hasAdditionalHolderInItems = false;

            var hasHolderInItems = source.Any(p => p.Holder == holder);

#if DEBUG
            //context.Logger.Log($"hasHolderInItems = {hasHolderInItems}");
#endif

            if (!holderIsEntity)
            {
                var allStateNamesList = context.Storage.GlobalStorage.StatesStorage.AllStateNames();

#if DEBUG
                //context.Logger.Log($"allStateNamesList = {allStateNamesList.WriteListToString()}");
#endif

                if (allStateNamesList.Any(p => p == holder))
                {
                    additionalHolder = context.InstancesStorage.MainEntity?.Name;

                    if (additionalHolder != null)
                    {
                        additionalHolderIsEntity = additionalHolder.KindOfName == KindOfName.Entity;
                        hasAdditionalHolderInItems = source.Any(p => p.Holder == additionalHolder);
                    }
                }
            }

#if DEBUG
            //context.Logger.Log($"additionalHolder = {additionalHolder}");
            //context.Logger.Log($"additionalHolderIsEntity = {additionalHolderIsEntity}");
            //context.Logger.Log($"hasAdditionalHolderInItems = {hasAdditionalHolderInItems}");
#endif

            var inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();

            foreach (var item in source)
            {
#if DEBUG
                //context.Logger.Log($"item = {item}");
                //context.Logger.Log($"item.TypeOfAccess = {item.TypeOfAccess}");
                //context.Logger.Log($"item.Holder = {item.Holder}");
#endif

                if(IsFitByTypeOfAccess(item, holder, inheritanceResolver, localCodeExecutionContext, holderIsEntity, hasHolderInItems, allowUnknown, context.Logger))
                {
                    result.Add(item);
                }
                else
                {
                    if(additionalHolder != null)
                    {
                        if (IsFitByTypeOfAccess(item, additionalHolder, inheritanceResolver, localCodeExecutionContext, additionalHolderIsEntity, hasAdditionalHolderInItems, allowUnknown, context.Logger))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

#if DEBUG
            //context.Logger.Log($"result.Count = {result.Count}");
#endif

            //throw new NotImplementedException();

            return result;
        }

        [Obsolete("Make the name none static.")]
        private static bool IsFitByTypeOfAccess(IReadOnlyMemberAccess item, StrongIdentifierValue holder, InheritanceResolver inheritanceResolver, LocalCodeExecutionContext localCodeExecutionContext, bool holderIsEntity, bool hasHolderInItems, bool allowUnknown, IEntityLogger logger)
        {
            var typeOfAccess = item.TypeOfAccess;

#if DEBUG
            //logger.Log($"typeOfAccess = {typeOfAccess}");
            //logger.Log($"holder = {holder}");
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
                            //logger.Log($"distance = {distance}");
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
                        //logger.Log($"inheritanceResolver != null = {inheritanceResolver != null}");
                        //logger.Log($"holder?.NameValue = {holder?.NameValue}");
                        //logger.Log($"item.Holder?.NameValue = {item.Holder?.NameValue}");
#endif

                        if(holder == item.Holder)
                        {
                            return true;
                        }

                        var rank = inheritanceResolver.GetRawInheritanceRank(holder, item.Holder, localCodeExecutionContext);

#if DEBUG
                        //logger.Log($"rank = {rank}");
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
