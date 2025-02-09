using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;

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
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<StrongIdentifierValue> OnChangedWithKeys;
    }
}
