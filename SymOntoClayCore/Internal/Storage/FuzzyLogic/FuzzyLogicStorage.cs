using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.FuzzyLogic
{
    public class FuzzyLogicStorage : BaseLoggedComponent, IFuzzyLogicStorage
    {
        public FuzzyLogicStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
            _commonNamesStorage = realStorageContext.MainStorageContext.CommonNamesStorage;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;
        private readonly ICommonNamesStorage _commonNamesStorage;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<FuzzyLogicNonNumericValue>>> _valuesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<FuzzyLogicNonNumericValue>>>();
        private Dictionary<StrongIdentifierValue, FuzzyLogicOperator> _defaultOperatorsDict = new Dictionary<StrongIdentifierValue, FuzzyLogicOperator>();

        /// <inheritdoc/>
        public void Append(LinguisticVariable linguisticVariable)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(linguisticVariable, _commonNamesStorage);

            lock (_lockObj)
            {
#if DEBUG
                Log($"linguisticVariable = {linguisticVariable}");
#endif

                linguisticVariable.CheckDirty();

                var holder = linguisticVariable.Holder;

                foreach (var fuzzyValue in linguisticVariable.Values)
                {
                    AnnotatedItemHelper.CheckAndFillUpHolder(fuzzyValue, _commonNamesStorage);

                    NAppendValue(fuzzyValue, holder);
                }
            }
        }

        private void NAppendValue(FuzzyLogicNonNumericValue value, StrongIdentifierValue holder)
        {
#if DEBUG
            Log($"value = {value}");
#endif

            var name = value.Name;

            if (_valuesDict.ContainsKey(name))
            {
                var dict = _valuesDict[name];

                if (dict.ContainsKey(holder))
                {
                    var targetList = dict[holder];

#if DEBUG
                    Log($"dict[holder].Count = {dict[holder].Count}");
                    Log($"targetList = {targetList.WriteListToString()}");
#endif
                    var targetLongConditionalHashCode = value.GetLongConditionalHashCode();

#if DEBUG
                    Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                    var itemsWithTheSameLongConditionalHashCodeList = targetList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

#if DEBUG
                    Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                    foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                    {
                        targetList.Remove(itemWithTheSameLongConditionalHashCode);
                    }

                    targetList.Add(value);
                }
                else
                {
                    dict[holder] = new List<FuzzyLogicNonNumericValue>() { value };
                }                    
            }
            else
            {
                var dict = new Dictionary<StrongIdentifierValue, List<FuzzyLogicNonNumericValue>>();
                dict[holder] = new List<FuzzyLogicNonNumericValue>() { value };
                _valuesDict[name] = dict;
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>> GetNonNumericValuesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"name = {name}");
#endif

                if(_valuesDict.ContainsKey(name))
                {
                    var dict = _valuesDict[name];

                    var result = new List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>>();
            }            
        }

        /// <inheritdoc/>
        public void AppendDefaultOperator(FuzzyLogicOperator fuzzyLogicOperator)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(fuzzyLogicOperator, _commonNamesStorage);

            lock (_lockObj)
            {
                _defaultOperatorsDict[fuzzyLogicOperator.Name] = fuzzyLogicOperator;
            }
        }

        /// <inheritdoc/>
        public FuzzyLogicOperator GetDefaultOperator(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if(_defaultOperatorsDict.ContainsKey(name))
                {
                    return _defaultOperatorsDict[name];
                }

                return null;
            }
        }
    }
}
