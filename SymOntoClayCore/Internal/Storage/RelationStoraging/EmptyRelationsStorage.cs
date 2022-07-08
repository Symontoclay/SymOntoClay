using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.RelationStoraging
{
    public class EmptyRelationsStorage : BaseEmptySpecificStorage, IRelationsStorage
    {
        public EmptyRelationsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(RelationDescription relation)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<RelationDescription>> GetRelationsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<RelationDescription>>();
        }
    }
}
