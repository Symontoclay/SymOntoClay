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
    public class TriggersResolver : BaseResolver
    {
        public TriggersResolver(IMainStorageContext context)
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

        public List<InlineTrigger> ResolveLogicConditionalTriggersList(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetLogicConditionalRawList(logger, storagesList, weightedInheritanceItems);

            return OrderAndDistinct(logger, rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        public List<InlineTrigger> ResolveAddFactTriggersList(IMonitorLogger logger, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetAddFactTriggersRawList(logger, storagesList, weightedInheritanceItems);

            return OrderAndDistinct(logger, rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        public List<InlineTrigger> ResolveSystemEventsTriggersList(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, holder, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetSystemEventsRawList(logger, kindOfSystemEvent, storagesList, weightedInheritanceItems);

            return OrderAndDistinct(logger, rawList, localCodeExecutionContext, options).Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> OrderAndDistinct(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> source, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceOrderOptions = options.Clone();

            return OrderAndDistinctByInheritance(logger, source, inheritanceOrderOptions);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetLogicConditionalRawList(IMonitorLogger logger, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.TriggersStorage.GetLogicConditionalTriggersDirectly(logger, weightedInheritanceItems);

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
                var itemsList = storageItem.Storage.TriggersStorage.GetAddFactTriggersDirectly(logger, weightedInheritanceItems);

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
                var itemsList = storageItem.Storage.TriggersStorage.GetSystemEventsTriggersDirectly(logger, kindOfSystemEvent, weightedInheritanceItems);

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
            return ResolveNamedTriggerInstance(logger, name, localCodeExecutionContext, _defaultOptions);
        }

        public INamedTriggerInstance ResolveNamedTriggerInstance(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var rawList = GetNamedTriggerInstanceRawList(logger, name, storagesList, localCodeExecutionContext);

            if(!rawList.Any())
            {
                return null;
            }

            if(rawList.Count == 1)
            {
                return rawList.SingleOrDefault();
            }

            throw new NotImplementedException("A57E2E2B-3019-4AD5-BBDB-4E4F817947A6");
        }

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = NGetNamedTriggerInstanceRawList(logger, name, storagesList);

            if(result.IsNullOrEmpty())
            {
                result = GetNamedTriggerInstanceRawListFromSynonyms(logger, name, storagesList, localCodeExecutionContext);
            }

            return result;
        }

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawListFromSynonyms(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetNamedTriggerInstanceRawList(logger, synonym, storagesList);

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
                var itemsList = storageItem.Storage.TriggersStorage.GetNamedTriggerInstancesDirectly(logger, name);

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
