using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ChannelsStorage
{
    public class EmptyChannelsStorage : BaseEmptySpecificStorage, IChannelsStorage
    {
        public EmptyChannelsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(Channel channel)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Channel>> GetChannelsDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<Channel>>();
        }
    }
}
