using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IRelationsStorage : ISpecificStorage
    {
        void Append(RelationDescription relation);
        IList<WeightedInheritanceResultItem<RelationDescription>> GetRelationsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems);
    }
}
