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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConstructorsResolver : BaseMethodsResolver
    {
        public ConstructorsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly List<Constructor> _emptyConstructorsList = new List<Constructor>();
        private readonly List<PreConstructor> _emptyPreConstructorsList = new List<PreConstructor>();

        public Constructor ResolveOnlyOwn(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var rawList = GetRawList(logger, 0, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(logger, holder) });

            return FilterRawListAndGetItem(logger, rawList, localCodeExecutionContext, options);
        }

        public Constructor ResolveOnlyOwn(IMonitorLogger logger, StrongIdentifierValue holder, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var rawList = GetRawList(logger, namedParameters.Count, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(logger, holder) });

            return FilterRawListAndGetItem(logger, rawList, namedParameters, localCodeExecutionContext, options);
        }

        public Constructor ResolveOnlyOwn(IMonitorLogger logger, StrongIdentifierValue holder, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var rawList = GetRawList(logger, positionedParameters.Count, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(logger, holder) });

            return FilterRawListAndGetItem(logger, rawList, positionedParameters, localCodeExecutionContext, options);
        }

        public Constructor ResolveWithSelfAndDirectInheritance(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, 0, storagesList, weightedInheritanceItems);

            return FilterRawListAndGetItem(logger, rawList, localCodeExecutionContext, options);
        }

        public Constructor ResolveWithSelfAndDirectInheritance(IMonitorLogger logger, StrongIdentifierValue holder, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder.ToTypeInfo(), localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, namedParameters.Count, storagesList, weightedInheritanceItems);

            return FilterRawListAndGetItem(logger, rawList, namedParameters, localCodeExecutionContext, options);
        }

        public Constructor ResolveWithSelfAndDirectInheritance(IMonitorLogger logger, StrongIdentifierValue holder, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder.ToTypeInfo(), localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, positionedParameters.Count, storagesList, weightedInheritanceItems);

            return FilterRawListAndGetItem(logger, rawList, positionedParameters, localCodeExecutionContext, options);
        }

        public List<Constructor> ResolveListWithSelfAndDirectInheritance(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance= true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder.ToTypeInfo(), localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, 0, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return _emptyConstructorsList;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return _emptyConstructorsList;
            }

            return filteredList.Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Constructor>> GetRawList(IMonitorLogger logger, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Constructor>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ConstructorsStorage.GetConstructorsDirectly(logger, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<Constructor>(item, distance, storage));
                }
            }

            return result;
        }

        public List<PreConstructor> ResolvePreConstructors(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder.ToTypeInfo(), localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawPreConstructorsList(logger, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return _emptyPreConstructorsList;
            }

            return rawList.OrderByDescending(p => p.Distance).Select(p => p.ResultItem).ToList();
        }

        private Constructor FilterRawListAndGetItem(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<Constructor>> rawList, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, 0, localCodeExecutionContext, options);
        }

        private Constructor FilterRawListAndGetItem(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<Constructor>> rawList, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, namedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        private Constructor FilterRawListAndGetItem(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<Constructor>> rawList, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, positionedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, positionedParameters.Count, localCodeExecutionContext, options);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<PreConstructor>> GetRawPreConstructorsList(IMonitorLogger logger, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<PreConstructor>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ConstructorsStorage.GetPreConstructorsDirectly(logger, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<PreConstructor>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
