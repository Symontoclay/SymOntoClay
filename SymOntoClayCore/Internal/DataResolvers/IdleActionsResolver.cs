using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class IdleActionsResolver : BaseResolver
    {
        public IdleActionsResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public List<IdleActionItem> Resolve(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(localCodeExecutionContext, _defaultOptions);
        }

        public List<IdleActionItem> Resolve(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawIdleActionItemList(storagesList, weightedInheritanceItems);

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

            return filteredList.Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IdleActionItem>> GetRawIdleActionItemList(List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IdleActionItem>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IdleActionItem>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.IdleActionItemsStorage.GetIdleActionsDirectly(weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IdleActionItem>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
