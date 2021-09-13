using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IActionsStorage : ISpecificStorage
    {
        void Append(ActionDef action);
        IList<WeightedInheritanceResultItem<ActionPtr>> GetActionsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems);
    }
}
