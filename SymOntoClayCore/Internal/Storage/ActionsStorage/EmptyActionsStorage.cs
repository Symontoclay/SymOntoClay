using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ActionsStorage
{
    public class EmptyActionsStorage : BaseEmptySpecificStorage, IActionsStorage
    {
        public EmptyActionsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(ActionDef action)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<ActionPtr>> GetActionsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<ActionPtr>>();
        }
    }
}
