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

    public abstract class BaseLoggedSyncCodeChunkFunctor<TResult>
    {
        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new MethodResponseOfBaseLoggedSyncCodeChunkFunctor<TResult>(this);
        }

        public abstract TResult GetResult();
    }
}
