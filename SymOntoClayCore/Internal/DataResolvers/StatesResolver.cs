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
    public class StatesResolver : BaseResolver
    {
        public StatesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawStatesList(name, storagesList, weightedInheritanceItems);

#if DEBUG
            Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(rawList, localCodeExecutionContext);

#if DEBUG
            Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> GetRawStatesList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                Log($"storageItem = {storageItem}");
#endif

                var itemsList = storageItem.Storage.StatesStorage.GetStatesDirectly(name, weightedInheritanceItems);

#if DEBUG
                Log($"itemsList = {itemsList.WriteListToString()}");
#endif

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
