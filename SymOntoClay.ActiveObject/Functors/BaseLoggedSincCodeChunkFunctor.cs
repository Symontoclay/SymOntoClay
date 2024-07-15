using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseLoggedSincCodeChunkFunctor
    {
        public IMethodResponse ToMethodResponse()
        {
            return new MethodResponseOfBaseLoggedSincCodeChunkFunctor(this);
        }
    }
}
