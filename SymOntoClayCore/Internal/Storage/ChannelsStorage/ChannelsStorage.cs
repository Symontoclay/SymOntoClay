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

        private readonly Dictionary<string, Dictionary<string, List<Channel>>> _nonIndexedInfo = new Dictionary<string, Dictionary<string, List<Channel>>>();
        //private readonly Dictionary<ulong, Dictionary<ulong, List<IndexedChannel>>> _indexedInfo = new Dictionary<ulong, Dictionary<ulong, List<IndexedChannel>>>();

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
            AnnotatedItemHelper.CheckAndFillHolder(channel, _realStorageContext.MainStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
#if DEBUG
                //Log($"channel = {channel}");
#endif

                //var indexedChannel = channel.GetIndexed(_realStorageContext.MainStorageContext);

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
        public IList<WeightedInheritanceResultItem<Channel>> GetChannelsDirectly(string name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                if (_nonIndexedInfo.ContainsKey(name))
                {
                    var dict = _nonIndexedInfo[name];

                    var result = new List<WeightedInheritanceResultItem<Channel>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<Channel>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<Channel>>();
            }
        }
    }
}
