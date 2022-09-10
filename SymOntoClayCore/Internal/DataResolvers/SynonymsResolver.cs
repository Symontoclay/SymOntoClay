using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class SynonymsResolver : BaseResolver
    {
        public SynonymsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        public List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
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

            var result = new List<StrongIdentifierValue>();

            var currentProcessedList = new List<StrongIdentifierValue>() { name };

            while (currentProcessedList.Any())
            {
                var futureProcessedList = new List<StrongIdentifierValue>();

#if DEBUG
                Log($"currentProcessedList = {currentProcessedList.WriteListToString()}");
#endif

                foreach (var processedItem in currentProcessedList)
                {
                    var synonymsList = GetSynonymsDirectly(processedItem, storagesList);

#if DEBUG
                    Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

                    if (synonymsList == null)
                    {
                        continue;
                    }

                    foreach (var item in synonymsList)
                    {
#if DEBUG
                        Log($"item = {item}");
#endif

                        if (item == name || result.Contains(item))
                        {
                            continue;
                        }

                        result.Add(item);
                        futureProcessedList.Add(item);
                    }
                }

#if DEBUG
                Log($"futureProcessedList = {futureProcessedList.WriteListToString()}");
#endif

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<StrongIdentifierValue> GetSynonymsDirectly(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return null;
            }

            var result = new List<StrongIdentifierValue>();

            foreach(var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.SynonymsStorage.GetSynonymsDirectly(name);

                if(itemsList == null)
                {
                    continue;
                }

                result.AddRange(itemsList);
            }

            return result;
        }
    }
}
