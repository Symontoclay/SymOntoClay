using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ChannelsStorage
{
    public class ChannelsStorage: BaseLoggedComponent, IChannelsStorage
    {
        public ChannelsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
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

        private readonly Dictionary<Name, Dictionary<Name, List<Channel>>> _nonIndexedInfo = new Dictionary<Name, Dictionary<Name, List<Channel>>>();
        private readonly Dictionary<Name, Dictionary<Name, List<IndexedChannel>>> _indexedInfo = new Dictionary<Name, Dictionary<Name, List<IndexedChannel>>>();

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
            AnnotatedItemHelper.CheckAndFillHolder(channel, _realStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
#if DEBUG
                Log($"channel = {channel}");
#endif

                var indexedChannel = channel.GetIndexed(_realStorageContext.EntityDictionary);

#if DEBUG
                Log($"indexedChannel = {indexedChannel}");
#endif

                var name = channel.Name;

                if (_nonIndexedInfo.ContainsKey(name))
                {
                    var dict = _nonIndexedInfo[name];
                    var indexedDict = _indexedInfo[name];

                    if (dict.ContainsKey(channel.Holder))
                    {
                        var list = dict[channel.Holder];

                        if (!list.Contains(channel))
                        {
                            list.Add(channel);

                            indexedDict[indexedChannel.Holder].Add(indexedChannel);
                        }
                    }
                    else
                    {
                        dict[channel.Holder] = new List<Channel>() { channel };
                        indexedDict[indexedChannel.Holder] = new List<IndexedChannel>() { indexedChannel };
                    }
                }
                else
                {
                    _nonIndexedInfo[name] = new Dictionary<Name, List<Channel>>() { { channel.Holder, new List<Channel>() { channel} } };
                    _indexedInfo[name] = new Dictionary<Name, List<IndexedChannel>>() { { indexedChannel.Holder, new List<IndexedChannel>() { indexedChannel } } };
                }              
            }         
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedChannel>> GetChannelsDirectly(Name name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"name = {name}");
#endif

                if (_indexedInfo.ContainsKey(name))
                {
                    var dict = _indexedInfo[name];

                    var result = new List<WeightedInheritanceResultItem<IndexedChannel>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

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
