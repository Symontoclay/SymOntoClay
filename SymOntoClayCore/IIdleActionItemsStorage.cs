using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IIdleActionItemsStorage : ISpecificStorage
    {
        void Append(IdleActionItem item);
        IList<WeightedInheritanceResultItem<IdleActionItem>> GetIdleActionsDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems);
    }
}
