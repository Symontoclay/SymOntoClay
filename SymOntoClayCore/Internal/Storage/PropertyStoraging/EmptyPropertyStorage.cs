using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Storage.PropertyStoraging
{
    public class EmptyPropertyStorage : BaseEmptySpecificStorage, IPropertyStorage
    {
        public EmptyPropertyStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PropertyInstance propertyInstance)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return null;
        }

        /// <inheritdoc/>
        public void AddOnChangedHandler(IOnChangedPropertyStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void RemoveOnChangedHandler(IOnChangedPropertyStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void AddOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
        {
        }
    }
}
