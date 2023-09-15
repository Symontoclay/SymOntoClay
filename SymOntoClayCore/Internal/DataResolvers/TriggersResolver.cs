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
    public class TriggersResolver : BaseResolver
    {
        public TriggersResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly SynonymsResolver _synonymsResolver;

        public List<InlineTrigger> ResolveLogicConditionalTriggersList(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetLogicConditionalRawList(storagesList, weightedInheritanceItems);

            return OrderAndDistinct(rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        public List<InlineTrigger> ResolveAddFactTriggersList(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetAddFactTriggersRawList(storagesList, weightedInheritanceItems);

            return OrderAndDistinct(rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        public List<InlineTrigger> ResolveSystemEventsTriggersList(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetSystemEventsRawList(kindOfSystemEvent, storagesList, weightedInheritanceItems);

            return OrderAndDistinct(rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> OrderAndDistinct(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> source, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceOrderOptions = options.Clone();

            return OrderAndDistinctByInheritance(source, inheritanceOrderOptions);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetLogicConditionalRawList(IMonitorLogger logger, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.TriggersStorage.GetLogicConditionalTriggersDirectly(weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>(item, distance, storage));
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetAddFactTriggersRawList(IMonitorLogger logger, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.TriggersStorage.GetAddFactTriggersDirectly(weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>(item, distance, storage));
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetSystemEventsRawList(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.TriggersStorage.GetSystemEventsTriggersDirectly(kindOfSystemEvent, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>(item, distance, storage));
                }
            }

            return result;
        }

        public INamedTriggerInstance ResolveNamedTriggerInstance(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveNamedTriggerInstance(name, localCodeExecutionContext, _defaultOptions);
        }

        public INamedTriggerInstance ResolveNamedTriggerInstance(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

            var rawList = GetNamedTriggerInstanceRawList(name, storagesList, localCodeExecutionContext);

            if(!rawList.Any())
            {
                return null;
            }

            if(rawList.Count == 1)
            {
                return rawList.SingleOrDefault();
            }

            throw new NotImplementedException();
        }

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = NGetNamedTriggerInstanceRawList(name, storagesList);

            if(result.IsNullOrEmpty())
            {
                result = GetNamedTriggerInstanceRawListFromSynonyms(name, storagesList, localCodeExecutionContext);
            }

            return result;
        }

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawListFromSynonyms(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(name, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetNamedTriggerInstanceRawList(synonym, storagesList);

                if (rawList.IsNullOrEmpty())
                {
                    continue;
                }

                return rawList;
            }

            return new List<INamedTriggerInstance>();
        }

        private List<INamedTriggerInstance> NGetNamedTriggerInstanceRawList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
            if (!storagesList.Any())
            {
                return new List<INamedTriggerInstance>();
            }

            var result = new List<INamedTriggerInstance>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.TriggersStorage.GetNamedTriggerInstancesDirectly(name);

                if (itemsList.IsNullOrEmpty())
                {
                    continue;
                }

                result.AddRange(itemsList);
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
