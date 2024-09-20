using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnChangedWithKeysVarStorageHandler
    {
        void Invoke(StrongIdentifierValue value);
    }
}
