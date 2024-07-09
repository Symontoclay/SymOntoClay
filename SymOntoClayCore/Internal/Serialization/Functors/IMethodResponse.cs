using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public interface IMethodResponse
    {
        Task Task { get; }
        void Wait();
    }

    public interface IMethodResponse<TResult>
    {
        Task<TResult> Task { get; }
        TResult Result { get; }
        void Wait();
    }
}
