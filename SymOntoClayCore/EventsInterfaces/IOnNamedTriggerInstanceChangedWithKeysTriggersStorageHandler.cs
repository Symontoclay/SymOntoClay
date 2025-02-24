using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler
    {
        void Invoke(IList<StrongIdentifierValue> value);
    }
}
