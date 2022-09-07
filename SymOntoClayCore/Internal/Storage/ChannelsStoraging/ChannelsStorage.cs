/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

namespace SymOntoClay.Core.Internal.Storage.ChannelsStoraging
{
    public class ChannelsStorage: BaseSpecificStorage, IChannelsStorage
    {
        public ChannelsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Channel>>> _nonIndexedInfo = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Channel>>>();

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(channel, _realStorageContext.MainStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
#if DEBUG
                //Log($"channel = {channel}");
#endif

                channel.CheckDirty();

                var name = channel.Name;

                var holder = channel.Holder;

                if (_nonIndexedInfo.ContainsKey(name))
                {
                    var dict = _nonIndexedInfo[name];

                    if (dict.ContainsKey(holder))
                    {
                        var targetList = dict[holder];

#if DEBUG
                        //Log($"dict[holder].Count = {dict[holder].Count}");
                        //Log($"targetList = {targetList.WriteListToString()}");
#endif

                        StorageHelper.RemoveSameItems(targetList, channel);

                        targetList.Add(channel);
                    }
                    else
                    {
                        dict[holder] = new List<Channel>() { channel };
                    }
                }
                else
                {
                    _nonIndexedInfo[name] = new Dictionary<StrongIdentifierValue, List<Channel>>() { { holder, new List<Channel>() { channel} } };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Channel>> GetChannelsDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
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

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _nonIndexedInfo.Clear();

            base.OnDisposed();
        }
    }
}
