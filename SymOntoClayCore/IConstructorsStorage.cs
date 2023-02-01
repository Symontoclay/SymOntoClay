using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IConstructorsStorage : ISpecificStorage
    {
        void Append(Constructor constructor);
        IList<WeightedInheritanceResultItem<Constructor>> GetConstructorsDirectly(int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems);
    }
}
