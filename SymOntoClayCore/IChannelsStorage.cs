using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IChannelsStorage : ISpecificStorage
    {
        void Append(Channel channel);
        IList<WeightedInheritanceResultItem<IndexedChannel>> GetChannelsDirectly(IndexedStrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);
    }
}
