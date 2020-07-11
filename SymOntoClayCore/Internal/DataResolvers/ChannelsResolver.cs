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
    public class ChannelsResolver : BaseResolver
    {
        public ChannelsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedChannel GetChannel(Name name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            Log($"storagesList.Count = {storagesList.Count}");
            foreach (var tmpStorage in storagesList)
            {
                Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            }
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, storagesList, weightedInheritanceItems);

#if DEBUG
            Log($"rawList = {rawList.WriteListToString()}");
#endif

            var filteredList = Filter(rawList);

#if DEBUG
            Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            var targetChannel = ChooseTargetItem(filteredList);

            return targetChannel;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedChannel>> GetRawList(Name name, List<KeyValuePair<uint, IStorage>> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IndexedChannel>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IndexedChannel>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Value.ChannelsStorage.GetChannelsDirectly(name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Key;
                var storage = storageItem.Value;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IndexedChannel>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
