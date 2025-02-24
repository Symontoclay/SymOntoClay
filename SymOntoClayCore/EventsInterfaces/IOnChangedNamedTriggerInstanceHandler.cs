using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnChangedNamedTriggerInstanceHandler
    {
        void Invoke(IList<StrongIdentifierValue> value);
    }
}
