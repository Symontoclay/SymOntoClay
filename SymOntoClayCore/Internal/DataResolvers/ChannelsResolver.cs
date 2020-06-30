using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
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

        public IndexedChannel GetChannel(Name name, IStorage storage)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            var storagesList = GetStoragesList(storage);

#if DEBUG
            Log($"storagesList.Count = {storagesList.Count}");
            foreach (var tmpStorage in storagesList)
            {
                Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            }
#endif

            var rawList = GetRawList(name, storagesList);

#if DEBUG
            Log($"rawList.Count = {rawList.Count}");
            foreach (var tmpItem in rawList)
            {
                Log($"tmpItem.Key = {tmpItem.Key}");
                Log($"tmpItem.Value = {tmpItem.Value}");
            }
#endif

            var filteredList = Filter(rawList);

#if DEBUG
            Log($"filteredList.Count = {filteredList.Count}");
            foreach (var tmpFilteredItem in filteredList)
            {
                Log($"tmpFilteredItem.Key = {tmpFilteredItem.Key}");
                Log($"tmpFilteredItem.Value = {tmpFilteredItem.Value}");
            }
#endif

            var targetChannel = ChooseTargetItem(filteredList);

            return targetChannel;
        }

        private List<KeyValuePair<uint, IndexedChannel>> GetRawList(Name name, List<KeyValuePair<uint, IStorage>> storagesList)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<KeyValuePair<uint, IndexedChannel>>();
            }

            var result = new List<KeyValuePair<uint, IndexedChannel>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Value.ChannelsStorage.GetChannelsDirectly(name);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Key;

                foreach (var item in itemsList)
                {
                    result.Add(new KeyValuePair<uint, IndexedChannel>(distance, item));
                }
            }

            return result;
        }
    }
}
