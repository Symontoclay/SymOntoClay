using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnChangedWithKeysPropertyStorageHandler
    {
        void Invoke(StrongIdentifierValue value);
    }
}
