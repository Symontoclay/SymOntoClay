using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IInheritanceStorage : ISpecificStorage
    {
        void SetInheritance(InheritanceItem inheritanceItem);
        void SetInheritance(InheritanceItem inheritanceItem, bool isPrimary);
        IList<WeightedInheritanceResultItem<IndexedInheritanceItem>> GetItemsDirectly(ulong subNameKey);
    }
}
