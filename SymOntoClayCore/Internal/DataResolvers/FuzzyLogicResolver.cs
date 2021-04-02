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

            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, localCodeExecutionContext, options);

#if DEBUG
            Log($"targetItem = {targetItem}");
#endif

            if(targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            Log($"fuzzyValue = {fuzzyValue}");
#endif

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
            Log($"value = {value}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, localCodeExecutionContext, options);

#if DEBUG
            Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            Log($"fuzzyValue = {fuzzyValue}");
#endif

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators);

#if DEBUG
            Log($"operatorsList.Count = {operatorsList.Count}");
#endif

            foreach (var op in operatorsList)
            {
#if DEBUG
                Log($"op = {op}");
#endif

                fuzzyValue = op.Handler.SystemCall(fuzzyValue);
            }

#if DEBUG
            Log($"fuzzyValue (after) = {fuzzyValue}");
#endif

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        private bool FuzzyNumericValueToSystemBool(double fuzzyValue)
        {
            return fuzzyValue >= _truthThreshold;
        }

        private List<FuzzyLogicOperator> GetFuzzyLogicOperators(LinguisticVariable linguisticVariable, IEnumerable<StrongIdentifierValue> operatorsIdentifiers)
        {
            if(operatorsIdentifiers.IsNullOrEmpty())
            {
                return new List<FuzzyLogicOperator>();
            }

            var result = new List<FuzzyLogicOperator>();

            var globalFuzzyLogicStorage = _context.Storage.GlobalStorage.FuzzyLogicStorage;

            foreach (var op in operatorsIdentifiers)
            {
#if DEBUG
                Log($"op = {op}");
#endif

                var item = linguisticVariable.GetOperator(op);

#if DEBUG
                Log($"item = {item}");
#endif

                if(item == null)
                {
                    item = globalFuzzyLogicStorage.GetDefaultOperator(op);

#if DEBUG
                    Log($"item (2) = {item}");
#endif

                    if(item == null)
                    {
                        throw new Exception($"Unexpected fuzzy logic operator `{op.NameValue}`!");
                    }
                }

                result.Add(item);
            }

            return result;
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValue(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            var targetItem = ChooseTargetItem(filteredList);

            return targetItem;
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
