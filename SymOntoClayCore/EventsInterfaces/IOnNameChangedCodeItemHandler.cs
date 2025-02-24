using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnNameChangedCodeItemHandler
    {
        void Invoke(StrongIdentifierValue value);
    }
}
