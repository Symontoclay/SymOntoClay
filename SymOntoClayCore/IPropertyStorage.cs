using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IPropertyStorage
    {
        void Append(IMonitorLogger logger, PropertyInstance propertyInstance);

        IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);

        event Action OnChanged;
        event Action<StrongIdentifierValue> OnChangedWithKeys;
    }
}
