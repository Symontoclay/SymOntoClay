using SymOntoClay.Core.Internal.CodeModel;
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

        private readonly Dictionary<Name, List<Channel>> _nonIndexedInfo = new Dictionary<Name, List<Channel>>();
        private readonly Dictionary<Name, List<IndexedChannel>> _indexedInfo = new Dictionary<Name, List<IndexedChannel>>();

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
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
                    var list = _nonIndexedInfo[name];

                    if (!list.Contains(channel))
                    {
                        list.Add(channel);
                        _indexedInfo[name].Add(indexedChannel);
                    }
                }
                else
                {
                    _nonIndexedInfo[name] = new List<Channel>() { channel };
                    _indexedInfo[name] = new List<IndexedChannel>() { indexedChannel };
                }              
            }         
        }

        /// <inheritdoc/>
        public IList<IndexedChannel> GetChannelsDirectly(Name name)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"name = {name}");
#endif

                if (_indexedInfo.ContainsKey(name))
                {
                    return _indexedInfo[name];
                }

                return new List<IndexedChannel>();
            }
        }
    }
}
