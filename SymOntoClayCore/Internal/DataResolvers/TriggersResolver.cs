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

        public List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> ResolveLogicConditionalTriggersList(StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetLogicConditionalRawList(holder, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList.Count = {rawList.Count}");
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            return OrderAndDistinct(rawList, localCodeExecutionContext, options);
        }

        public List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> ResolveAddFactTriggersList(StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetAddFactTriggersRawList(holder, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList.Count = {rawList.Count}");
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            return OrderAndDistinct(rawList, localCodeExecutionContext, options);
        }

        public List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
            //Log($"holder = {holder}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetSystemEventsRawList(kindOfSystemEvent, holder, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            return OrderAndDistinct(rawList, localCodeExecutionContext, options);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> OrderAndDistinct(List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceOrderOptions = options.Clone();

            return OrderAndDistinctByInheritance(source, inheritanceOrderOptions);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetLogicConditionalRawList(StrongIdentifierValue holder, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();
            }

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

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetAddFactTriggersRawList(StrongIdentifierValue holder, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();
            }

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

        private List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>> GetSystemEventsRawList(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
            //Log($"holder = {holder}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<InlineTrigger>>();
            }

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

        public INamedTriggerInstance ResolveNamedTriggerInstance(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveNamedTriggerInstance(name, localCodeExecutionContext, _defaultOptions);
        }

        public INamedTriggerInstance ResolveNamedTriggerInstance(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var rawList = GetNamedTriggerInstanceRawList(name, storagesList, localCodeExecutionContext);

#if DEBUG
            Log($"rawList.Count = {rawList.Count}");
#endif

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

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = NGetNamedTriggerInstanceRawList(name, storagesList);

            if(result.IsNullOrEmpty())
            {
                result = GetNamedTriggerInstanceRawListFromSynonyms(name, storagesList, localCodeExecutionContext);
            }

            return result;
        }

        private List<INamedTriggerInstance> GetNamedTriggerInstanceRawListFromSynonyms(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(name, localCodeExecutionContext);

#if DEBUG
            Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetNamedTriggerInstanceRawList(name, storagesList);

                if (rawList.IsNullOrEmpty())
                {
                    continue;
                }

                return rawList;
            }

            return new List<INamedTriggerInstance>();
        }

        private List<INamedTriggerInstance> NGetNamedTriggerInstanceRawList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
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
