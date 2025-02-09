using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core
{
    public interface IPropertyStorage
    {
        void Append(IMonitorLogger logger, PropertyInstance propertyInstance);

        event Action OnChanged;
        event Action<StrongIdentifierValue> OnChangedWithKeys;
    }
}
