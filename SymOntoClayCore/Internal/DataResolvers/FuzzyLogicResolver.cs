/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
    public class FuzzyLogicResolver : BaseResolver
    {
        public FuzzyLogicResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly double _truthThreshold = 0.75;
        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public NumberValue Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"reason = {reason}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, null, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var fuzzyValue = targetItem.Handler.Defuzzificate();

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return fuzzyValue;
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, null, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators).Select(p => p.Handler);

#if DEBUG
            //Log($"operatorsList.Count = {operatorsList.Count}");
#endif

            var fuzzyValue = targetItem.Handler.Defuzzificate(operatorsList).SystemValue.Value;

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return new NumberValue(fuzzyValue);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, value, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if(targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
            //Log($"value = {value}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, value, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators);

#if DEBUG
            //Log($"operatorsList.Count = {operatorsList.Count}");
#endif

            foreach (var op in operatorsList)
            {
#if DEBUG
                //Log($"op = {op}");
#endif

                fuzzyValue = op.Handler.SystemCall(fuzzyValue);
            }

#if DEBUG
            //Log($"fuzzyValue (after) = {fuzzyValue}");
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
                //Log($"op = {op}");
#endif

                var item = linguisticVariable.GetOperator(op);

#if DEBUG
                //Log($"item = {item}");
#endif

                if(item == null)
                {
                    item = globalFuzzyLogicStorage.GetDefaultOperator(op);

#if DEBUG
                    //Log($"item (2) = {item}");
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

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValue(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            if(reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance)
            {
#if DEBUG
                //Log("^%^%^%^%^%^% reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance");
#endif

                optionsForInheritanceResolver.SkipRealSearching = true;
                optionsForInheritanceResolver.AddSelf = false;
            }

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, storagesList, weightedInheritanceItems);

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

            if (!filteredList.Any())
            {
                return null;
            }

            if((reason == null || reason.Kind != KindOfReasonOfFuzzyLogicResolving.Inheritance) && value != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByRange(value)).ToList();

#if DEBUG
                //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            if(reason != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByСonstraintOrDontHasСonstraint(reason)).ToList();

#if DEBUG
                //Log($"filteredList (3) = {filteredList.WriteListToString()}");
#endif

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            return GetTargetFuzzyLogicNonNumericValueFromList(filteredList, reason);
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValueFromList(List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> list, ReasonOfFuzzyLogicResolving reason)
        {
            if (!list.Any())
            {
                return null;
            }

            if (list.Count == 1)
            {
                return list.FirstOrDefault()?.ResultItem;
            }

            if(list.Any(p => p.ResultItem.Parent.IsFitByСonstraint(reason)) && list.Any(p => p.ResultItem.Parent.IsConstraintNullOrEmpty))
            {
                list = list.Where(p => p.ResultItem.Parent.IsFitByСonstraint(reason)).ToList();

                if (!list.Any())
                {
                    return null;
                }

                if (list.Count == 1)
                {
                    return list.FirstOrDefault()?.ResultItem;
                }
            }

            var minLengthOfRange = list.Min(p => p.ResultItem.Parent.Range.Length);

#if DEBUG
            //Log($"minLengthOfRange = {minLengthOfRange}");
#endif

            var targetItem = list.FirstOrDefault(p => p.ResultItem.Parent.Range.Length == minLengthOfRange)?.ResultItem;

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
