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
    public class RelationsResolver : BaseResolver
    {
        public RelationsResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        private readonly InheritanceResolver _inheritanceResolver;

        public RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRelation(name, paramsCount, localCodeExecutionContext, DefaultOptions);
        }        

        public RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            Log($"storagesList.Count = {storagesList.Count}");
            foreach (var tmpStorage in storagesList)
            {
                Log($"tmpStorage.Storage = {tmpStorage.Storage}");
            }
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, paramsCount, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            var targetItem = ChooseTargetItem(filteredList);

            return targetItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>> GetRawList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.RelationsStorage.GetRelationsDirectly(name, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<RelationDescription>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
