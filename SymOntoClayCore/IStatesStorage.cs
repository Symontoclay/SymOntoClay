using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStatesStorage : ISpecificStorage
    {
        void Append(StateDef state);
        IList<WeightedInheritanceResultItem<StateDef>> GetStatesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);
        void SetDefaultStateName(StrongIdentifierValue name);
    }
}
