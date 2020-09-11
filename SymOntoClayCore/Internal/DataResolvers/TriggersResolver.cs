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
    public class TriggersResolver : BaseResolver
    {
        public TriggersResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>> ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IndexedStrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetSystemEventsRawList(kindOfSystemEvent, holder, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            return OrderAndDistinct(rawList, localCodeExecutionContext, options);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>> OrderAndDistinct(List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>> source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceOrderOptions = options.Clone();

            var result = OrderAndDistinctByInheritance(source, inheritanceOrderOptions);

            result = result.OrderBy(p => p.Distance).ToList();

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>> GetSystemEventsRawList(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IndexedStrongIdentifierValue holder, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
            //Log($"holder = {holder}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>>();

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
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IndexedInlineTrigger>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
