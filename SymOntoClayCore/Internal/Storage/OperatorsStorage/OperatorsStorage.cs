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
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.OperatorsStorage
{
    public class OperatorsStorage : BaseLoggedComponent, IOperatorsStorage
    {
        public OperatorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private readonly Dictionary<KindOfOperator, Dictionary<StrongIdentifierValue, List<Operator>>> _nonIndexedInfo = new Dictionary<KindOfOperator, Dictionary<StrongIdentifierValue, List<Operator>>>();
        private readonly Dictionary<KindOfOperator, Dictionary<ulong, List<IndexedOperator>>> _indexedInfo = new Dictionary<KindOfOperator, Dictionary<ulong, List<IndexedOperator>>>();

        /// <inheritdoc/>
        public void Append(Operator op)
        {
#if DEBUG
            //Log($"op = {op}");
#endif

            AnnotatedItemHelper.CheckAndFillHolder(op, _realStorageContext.MainStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
                var indexedOp = op.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedOp = {indexedOp}");
#endif

                var kindOfOperator = indexedOp.KindOfOperator;
                var indexedOpHolderKey = indexedOp.Holder.NameKey;

                if (_nonIndexedInfo.ContainsKey(kindOfOperator))
                {
                    var dict = _nonIndexedInfo[kindOfOperator];
                    var indexedDict = _indexedInfo[kindOfOperator];

                    if (dict.ContainsKey(op.Holder))
                    {
                        var targetList = dict[op.Holder];

#if DEBUG
                        Log($"dict[superName].Count = {dict[op.Holder].Count}");
                        Log($"targetList = {targetList.WriteListToString()}");
#endif
                        var targetLongConditionalHashCode = indexedOp.GetLongConditionalHashCode();

#if DEBUG
                        Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                        var targetIndexedList = indexedDict[indexedOpHolderKey];

                        var indexedItemsWithTheSameLongConditionalHashCodeList = targetIndexedList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                        foreach (var indexedItemWithTheSameLongConditionalHashCode in indexedItemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetIndexedList.Remove(indexedItemWithTheSameLongConditionalHashCode);
                        }

                        var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalOperator).ToList();

#if DEBUG
                        Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                        foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetList.Remove(itemWithTheSameLongConditionalHashCode);
                        }

                        targetList.Add(op);

                        targetIndexedList.Add(indexedOp);
                    }
                    else
                    {
                        dict[op.Holder] = new List<Operator>() { op };
                        indexedDict[indexedOpHolderKey] = new List<IndexedOperator>() { indexedOp };
                    }
                }
                else
                {
                    _nonIndexedInfo[kindOfOperator] = new Dictionary<StrongIdentifierValue, List<Operator>>() { { op.Holder, new List<Operator>() { op } } };
                    _indexedInfo[kindOfOperator] = new Dictionary<ulong, List<IndexedOperator>>() { { indexedOpHolderKey, new List<IndexedOperator>() { indexedOp } } };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedOperator>> GetOperatorsDirectly(KindOfOperator kindOfOperator, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            lock (_lockObj)
            {
                if(_indexedInfo.ContainsKey(kindOfOperator))
                {
                    var dict = _indexedInfo[kindOfOperator];

                    var result = new List<WeightedInheritanceResultItem<IndexedOperator>>();

                    foreach(var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach(var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<IndexedOperator>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedOperator>>();
            }
        }
    }
}
