/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
                        var targetHolder = weightedInheritanceItem.SuperNameKey;

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
