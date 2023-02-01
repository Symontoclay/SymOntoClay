using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ConstructorsStoraging
{
    public class EmptyConstructorsStorage : BaseEmptySpecificStorage, IConstructorsStorage
    {
        public EmptyConstructorsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(Constructor constructor)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Constructor>> GetConstructorsDirectly(int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<Constructor>>();
        }
    }
}
