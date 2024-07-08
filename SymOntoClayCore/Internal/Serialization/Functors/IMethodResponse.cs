using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public interface IMethodResponse
    {
        Task Task { get; }
    }

    public interface IMethodResponse<TResult>
    {
        Task<TResult> Task { get; }
        TResult Result { get; }
    }
}
