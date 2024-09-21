using SymOntoClay.Core.Internal.Instances;

namespace SymOntoClay.Core.EventsInterfaces
{
    public interface IOnStateInstanceFinishedStateInstanceHandler
    {
        void Invoke(StateInstance value);
    }
}
