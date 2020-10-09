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

namespace SymOntoClay.Core.Internal.Storage.ChannelsStorage
{
    public class ChannelsStorage: BaseLoggedComponent, IChannelsStorage
    {
        public ChannelsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
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

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Channel>>> _nonIndexedInfo = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Channel>>>();
        private readonly Dictionary<ulong, Dictionary<ulong, List<IndexedChannel>>> _indexedInfo = new Dictionary<ulong, Dictionary<ulong, List<IndexedChannel>>>();

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
            AnnotatedItemHelper.CheckAndFillHolder(channel, _realStorageContext.MainStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
#if DEBUG
                //Log($"channel = {channel}");
#endif

                var indexedChannel = channel.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedChannel = {indexedChannel}");
#endif

                var name = channel.Name;
                var indexedNameKey = indexedChannel.Name.NameKey;

                if (_nonIndexedInfo.ContainsKey(name))
                {
                    var dict = _nonIndexedInfo[name];
                    var indexedDict = _indexedInfo[indexedNameKey];

                    if (dict.ContainsKey(channel.Holder))
                    {
                        var targetList = dict[channel.Holder];

#if DEBUG
                        Log($"dict[superName].Count = {dict[channel.Holder].Count}");
                        Log($"targetList = {targetList.WriteListToString()}");
#endif
                        var targetLongConditionalHashCode = indexedChannel.GetLongConditionalHashCode();

#if DEBUG
                        Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                        var targetIndexedList = indexedDict[indexedChannel.Holder.NameKey];

                        var indexedItemsWithTheSameLongConditionalHashCodeList = targetIndexedList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                        foreach (var indexedItemWithTheSameLongConditionalHashCode in indexedItemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetIndexedList.Remove(indexedItemWithTheSameLongConditionalHashCode);
                        }

                        var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalChannel).ToList();

#if DEBUG
                        Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                        foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetList.Remove(itemWithTheSameLongConditionalHashCode);
                        }

                        targetList.Add(channel);

                        targetIndexedList.Add(indexedChannel);
                    }
                    else
                    {
                        dict[channel.Holder] = new List<Channel>() { channel };
                        indexedDict[indexedChannel.Holder.NameKey] = new List<IndexedChannel>() { indexedChannel };
                    }
                }
                else
                {
                    _nonIndexedInfo[name] = new Dictionary<StrongIdentifierValue, List<Channel>>() { { channel.Holder, new List<Channel>() { channel} } };
                    _indexedInfo[indexedNameKey] = new Dictionary<ulong, List<IndexedChannel>>() { { indexedChannel.Holder.NameKey, new List<IndexedChannel>() { indexedChannel } } };
                }              
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedChannel>> GetChannelsDirectly(ulong nameKey, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                if (_indexedInfo.ContainsKey(nameKey))
                {
                    var dict = _indexedInfo[nameKey];

                    var result = new List<WeightedInheritanceResultItem<IndexedChannel>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperNameKey;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<IndexedChannel>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedChannel>>();
            }
        }
    }
}
