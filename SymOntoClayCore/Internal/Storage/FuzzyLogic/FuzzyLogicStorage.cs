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
    public class FuzzyLogicStorage : BaseComponent, IFuzzyLogicStorage
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
                //Log($"linguisticVariable = {linguisticVariable}");

                //if(linguisticVariable.Name.NameValue == "age")
                //{
                    //var constraintItem = new LinguisticVariableConstraintItem();
                    //constraintItem.Kind = KindOfLinguisticVariableСonstraintItem.Inheritance;

                    //Log($"constraintItem.ToDbgString() = {constraintItem.ToDbgString()}");

                    //linguisticVariable.Constraint.Items.Add(constraintItem);

                    //constraintItem = new LinguisticVariableConstraintItem();
                    //constraintItem.Kind = KindOfLinguisticVariableСonstraintItem.Relation;
                    //constraintItem.RelationName = NameHelper.CreateName("age");

                    //Log($"constraintItem.ToDbgString() = {constraintItem.ToDbgString()}");

                    //linguisticVariable.Constraint.Items.Add(constraintItem);

                    //Log($"linguisticVariable (2) = {linguisticVariable}");
                //}
#endif

                linguisticVariable.CheckDirty();

#if DEBUG
                //if (linguisticVariable.Name.NameValue == "age")
                //{
                //    Log($"linguisticVariable (3) = {linguisticVariable}");

                //    //throw new NotImplementedException();
                //}
#endif

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
            //Log($"value = {value}");
#endif

            var name = value.Name;

            if (_valuesDict.ContainsKey(name))
            {
                var dict = _valuesDict[name];

                if (dict.ContainsKey(holder))
                {
                    var targetList = dict[holder];

#if DEBUG
                    //Log($"dict[holder].Count = {dict[holder].Count}");
                    //Log($"targetList = {targetList.WriteListToString()}");
#endif

                    StorageHelper.RemoveSameItems(targetList, value);

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
                //Log($"name = {name}");
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

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _valuesDict.Clear();
            _defaultOperatorsDict.Clear();

            base.OnDisposed();
        }
    }
}
