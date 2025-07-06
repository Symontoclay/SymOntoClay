using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStoraging
{
    public class EmptyInheritanceStorage : BaseEmptySpecificStorage, IInheritanceStorage
    {
        public EmptyInheritanceStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
        }

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem, bool isPrimary)
        {
        }

        /// <inheritdoc/>
        public void RemoveInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
        }

        private static List<WeightedInheritanceResultItem<InheritanceItem>> _getItemsDirectlyEmptyList = new List<WeightedInheritanceResultItem<InheritanceItem>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InheritanceItem>> GetItemsDirectly(IMonitorLogger logger, StrongIdentifierValue subName)
        {
            return _getItemsDirectlyEmptyList;
        }
    }
}
