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
    public class FuzzyLogicResolver : BaseResolver
    {
        public FuzzyLogicResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly double _truthThreshold = 0.75;
        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public bool Equals(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
            Log($"value = {value}");
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
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            var targetItem = ChooseTargetItem(filteredList);

#if DEBUG
            Log($"targetItem = {targetItem}");
#endif

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            Log($"fuzzyValue = {fuzzyValue}");
#endif
            return fuzzyValue >= _truthThreshold;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> GetRawList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.FuzzyLogicStorage.GetNonNumericValuesDirectly(name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
