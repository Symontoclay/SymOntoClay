/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
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
        public void Append(IMonitorLogger logger, Channel channel)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(logger, channel, _realStorageContext.MainStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
                channel.CheckDirty();

                var name = channel.Name;

                var holder = channel.Holder;

                if (_nonIndexedInfo.ContainsKey(name))
                {
                    var dict = _nonIndexedInfo[name];

                    if (dict.ContainsKey(holder))
                    {
                        var targetList = dict[holder];

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

        private static List<WeightedInheritanceResultItem<Channel>> _emptyChannelsList = new List<WeightedInheritanceResultItem<Channel>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Channel>> GetChannelsDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyChannelsList;
                }

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

                return _emptyChannelsList;
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
