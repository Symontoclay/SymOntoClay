using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IPropertyStorage : ISpecificStorage
    {
        void Append(IMonitorLogger logger, PropertyInstance propertyInstance);

        IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);

        void AddOnChangedHandler(IOnChangedPropertyStorageHandler handler);
        void RemoveOnChangedHandler(IOnChangedPropertyStorageHandler handler);

        void AddOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler);
        void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler);
    }
}
