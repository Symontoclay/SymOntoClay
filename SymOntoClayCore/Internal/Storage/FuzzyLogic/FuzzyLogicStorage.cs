/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.FuzzyLogic
{
    public class FuzzyLogicStorage : BaseSpecificStorage, IFuzzyLogicStorage
    {
        public FuzzyLogicStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _commonNamesStorage = realStorageContext.MainStorageContext.CommonNamesStorage;
        }

        private readonly object _lockObj = new object();

        private readonly ICommonNamesStorage _commonNamesStorage;

        private Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<FuzzyLogicNonNumericValue>>> _valuesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<FuzzyLogicNonNumericValue>>>();
        private Dictionary<StrongIdentifierValue, FuzzyLogicOperator> _defaultOperatorsDict = new Dictionary<StrongIdentifierValue, FuzzyLogicOperator>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, LinguisticVariable linguisticVariable)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(logger, linguisticVariable, _commonNamesStorage);

            lock (_lockObj)
            {
                linguisticVariable.CheckDirty();

                var holder = linguisticVariable.Holder;

                foreach (var fuzzyValue in linguisticVariable.Values)
                {
                    NAppendValue(logger, fuzzyValue, holder);
                }
            }
        }

        private void NAppendValue(IMonitorLogger logger, FuzzyLogicNonNumericValue value, StrongIdentifierValue holder)
        {
            var name = value.Name;

            if (_valuesDict.ContainsKey(name))
            {
                var dict = _valuesDict[name];

                if (dict.ContainsKey(holder))
                {
                    var targetList = dict[holder];

                    StorageHelper.RemoveSameItems(logger, targetList, value);

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

        private static List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>> _emptyNonNumericValuesList = new List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>> GetNonNumericValuesDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyNonNumericValuesList;
                }

                if (_valuesDict.ContainsKey(name))
                {
                    var dict = _valuesDict[name];

                    var result = new List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperType.Name;

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

                return _emptyNonNumericValuesList;
            }            
        }

        /// <inheritdoc/>
        public void AppendDefaultOperator(IMonitorLogger logger, FuzzyLogicOperator fuzzyLogicOperator)
        {
            lock (_lockObj)
            {
                _defaultOperatorsDict[fuzzyLogicOperator.Name] = fuzzyLogicOperator;
            }
        }

        /// <inheritdoc/>
        public FuzzyLogicOperator GetDefaultOperator(IMonitorLogger logger, StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                if (_defaultOperatorsDict.ContainsKey(name))
                {
                    return _defaultOperatorsDict[name];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _valuesDict.Clear();
            _defaultOperatorsDict.Clear();

            base.OnDisposed();
        }
    }
}
