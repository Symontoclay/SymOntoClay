using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseLoggedSyncCodeChunkFunctor
    {
        public IMethodResponse ToMethodResponse()
        {
            return new MethodResponseOfBaseLoggedSyncCodeChunkFunctor(this);
        }
    }
}
