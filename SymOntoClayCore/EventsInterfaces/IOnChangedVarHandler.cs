using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnChangedVarHandler
    {
        void Invoke(StrongIdentifierValue value);
    }
}
