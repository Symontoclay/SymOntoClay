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
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class StatesResolver : BaseResolver
    {
        public StatesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private SynonymsResolver _synonymsResolver;

        public StrongIdentifierValue ResolveDefaultStateName(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveDefaultStateName(logger, localCodeExecutionContext, _defaultOptions);
        }

        public StrongIdentifierValue ResolveDefaultStateName(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            if (!storagesList.Any())
            {
                return null;
            }

            foreach (var storageItem in storagesList)
            {
                var item = storageItem.Storage.StatesStorage.GetDefaultStateNameDirectly(logger);

                if(item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public StateDef Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, localCodeExecutionContext, _defaultOptions);
        }

        public StateDef Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawStatesList(logger, name, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(logger, filteredList, options).FirstOrDefault()?.ResultItem;
        }

        public List<StateDef> ResolveAllStatesList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveAllStatesList(logger, localCodeExecutionContext, _defaultOptions);
        }

        public List<StateDef> ResolveAllStatesList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            if (!storagesList.Any())
            {
                return new List<StateDef>();
            }

            var result = new List<StateDef>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.StatesStorage.GetAllStatesListDirectly(logger);

                result.AddRange(itemsList);
            }

            return result;
        }

        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveActivationInfoOfStateList(logger, localCodeExecutionContext, _defaultOptions);
        }
        
        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            if (!storagesList.Any())
            {
                return new List<ActivationInfoOfStateDef>();
            }

            var result = new List<ActivationInfoOfStateDef>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.StatesStorage.GetActivationInfoOfStateListDirectly(logger);

                result.AddRange(itemsList);
            }

            return result;
        }

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveMutuallyExclusiveStatesSetsList(logger, localCodeExecutionContext, _defaultOptions);
        }

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            if (!storagesList.Any())
            {
                return new List<MutuallyExclusiveStatesSet>();
            }

            var result = new List<MutuallyExclusiveStatesSet>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.StatesStorage.GetMutuallyExclusiveStatesSetsListDirectly(logger);

                result.AddRange(itemsList);
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> GetRawStatesList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = NGetRawStatesList(logger, name, storagesList, weightedInheritanceItems);

            if (result.IsNullOrEmpty())
            {
                result = GetRawStatesListFromSynonyms(logger, name, storagesList, weightedInheritanceItems);
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> GetRawStatesListFromSynonyms(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, storagesList);

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetRawStatesList(logger, synonym, storagesList, weightedInheritanceItems);

                if (rawList.IsNullOrEmpty())
                {
                    continue;
                }

                return rawList;
            }

            return new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> NGetRawStatesList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.StatesStorage.GetStatesDirectly(logger, name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<StateDef>(item, distance, storage));
                }
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
