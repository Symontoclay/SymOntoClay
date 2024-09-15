using System.Threading.Tasks;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public interface IAsyncMethodResponse
    {
        Task Task { get; }
        void Wait();
    }

    public interface IAsyncMethodResponse<TResult>
    {
        Task<TResult> Task { get; }
        TResult Result { get; }
        void Wait();
    }
}
