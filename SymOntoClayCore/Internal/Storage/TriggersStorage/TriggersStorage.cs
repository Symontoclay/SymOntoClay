/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.TriggersStorage
{
    public class TriggersStorage: BaseLoggedComponent, ITriggersStorage
    {
        public TriggersStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>> _nonIndexedSystemEventsInfo = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>>();
        private readonly Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<ulong, List<IndexedInlineTrigger>>> _indexedSystemEventsInfo = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<ulong, List<IndexedInlineTrigger>>>();

        private readonly Dictionary<StrongIdentifierValue, List<InlineTrigger>> _nonIndexedLogicConditionalsInfo = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();
        private readonly Dictionary<ulong, List<IndexedInlineTrigger>> _indexedLogicConditionalsInfo = new Dictionary<ulong, List<IndexedInlineTrigger>>();

        /// <inheritdoc/>
        public void Append(InlineTrigger inlineTrigger)
        {
#if DEBUG
            //Log($"inlineTrigger = {inlineTrigger}");
#endif

            AnnotatedItemHelper.CheckAndFillHolder(inlineTrigger, _realStorageContext.MainStorageContext.CommonNamesStorage);

            var kind = inlineTrigger.Kind;

            switch (kind)
            {
                case KindOfInlineTrigger.SystemEvent:
                    AppendSystemEvent(inlineTrigger);
                    break;

                case KindOfInlineTrigger.LogicConditional:
                    AppendLogicConditional(inlineTrigger);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void AppendLogicConditional(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                var indexedItem = inlineTrigger.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                Log($"indexedItem = {indexedItem}");
#endif

                var indexedItemHolderKey = indexedItem.Holder.NameKey;

                if (_nonIndexedLogicConditionalsInfo.ContainsKey(inlineTrigger.Holder))
                {
                    var targetList = _nonIndexedLogicConditionalsInfo[inlineTrigger.Holder];

#if DEBUG
                    Log($"_nonIndexedLogicConditionalsInfo[superName].Count = {_nonIndexedLogicConditionalsInfo[inlineTrigger.Holder].Count}");
                    Log($"targetList = {targetList.WriteListToString()}");
#endif
                    var targetLongConditionalHashCode = indexedItem.GetLongConditionalHashCode();

#if DEBUG
                    Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                    var targetIndexedList = _indexedLogicConditionalsInfo[indexedItemHolderKey];

                    var indexedItemsWithTheSameLongConditionalHashCodeList = targetIndexedList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                    foreach (var indexedItemWithTheSameLongConditionalHashCode in indexedItemsWithTheSameLongConditionalHashCodeList)
                    {
                        targetIndexedList.Remove(indexedItemWithTheSameLongConditionalHashCode);
                    }

                    var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalInlineTrigger).ToList();

#if DEBUG
                    Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                    foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                    {
                        targetList.Remove(itemWithTheSameLongConditionalHashCode);
                    }

                    targetList.Add(inlineTrigger);
                    targetIndexedList.Add(indexedItem);
                }
                else
                {
                    _nonIndexedLogicConditionalsInfo[inlineTrigger.Holder] = new List<InlineTrigger>() { inlineTrigger };
                    _indexedLogicConditionalsInfo[indexedItemHolderKey] = new List<IndexedInlineTrigger>() { indexedItem };
                }
            }            
        }

        private void AppendSystemEvent(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                var indexedItem = inlineTrigger.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedItem = {indexedItem}");
#endif

                var indexedItemHolderKey = indexedItem.Holder.NameKey;

                var kindOfSystemEvent = inlineTrigger.KindOfSystemEvent;

                if (_nonIndexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _nonIndexedSystemEventsInfo[kindOfSystemEvent];
                    var indexedDict = _indexedSystemEventsInfo[kindOfSystemEvent];

                    if (dict.ContainsKey(inlineTrigger.Holder))
                    {
                        var targetList = dict[inlineTrigger.Holder];

#if DEBUG
                        Log($"dict[superName].Count = {dict[inlineTrigger.Holder].Count}");
                        Log($"targetList = {targetList.WriteListToString()}");
#endif
                        var targetLongConditionalHashCode = indexedItem.GetLongConditionalHashCode();

#if DEBUG
                        Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                        var targetIndexedList = indexedDict[indexedItemHolderKey];

                        var indexedItemsWithTheSameLongConditionalHashCodeList = targetIndexedList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                        foreach (var indexedItemWithTheSameLongConditionalHashCode in indexedItemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetIndexedList.Remove(indexedItemWithTheSameLongConditionalHashCode);
                        }

                        var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalInlineTrigger).ToList();

#if DEBUG
                        Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                        foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetList.Remove(itemWithTheSameLongConditionalHashCode);
                        }

                        targetList.Add(inlineTrigger);
                        targetIndexedList.Add(indexedItem);
                    }
                    else
                    {
                        dict[inlineTrigger.Holder] = new List<InlineTrigger>() { inlineTrigger };
                        indexedDict[indexedItemHolderKey] = new List<IndexedInlineTrigger>() { indexedItem };
                    }
                }
                else
                {
                    _nonIndexedSystemEventsInfo[kindOfSystemEvent] = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>() { { inlineTrigger.Holder, new List<InlineTrigger>() { inlineTrigger } } };
                    _indexedSystemEventsInfo[kindOfSystemEvent] = new Dictionary<ulong, List<IndexedInlineTrigger>>() { { indexedItemHolderKey, new List<IndexedInlineTrigger>() { indexedItem } } };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedInlineTrigger>> GetSystemEventsTriggersDirectly(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
#endif

            lock (_lockObj)
            {
                if (_indexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _indexedSystemEventsInfo[kindOfSystemEvent];

                    var result = new List<WeightedInheritanceResultItem<IndexedInlineTrigger>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperNameKey;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<IndexedInlineTrigger>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedInlineTrigger>>();
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedInlineTrigger>> GetLogicConditionalTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItem<IndexedInlineTrigger>>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
                var targetHolder = weightedInheritanceItem.SuperNameKey;

                if (_indexedLogicConditionalsInfo.ContainsKey(targetHolder))
                {
                    var targetList = _indexedLogicConditionalsInfo[targetHolder];

                    foreach (var targetVal in targetList)
                    {
                        result.Add(new WeightedInheritanceResultItem<IndexedInlineTrigger>(targetVal, weightedInheritanceItem));
                    }
                }
            }

            return result;
        }
    }
}
