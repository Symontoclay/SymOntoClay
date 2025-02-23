using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnChangedPropertyHandler
    {
        void Invoke(StrongIdentifierValue value);
    }
}
