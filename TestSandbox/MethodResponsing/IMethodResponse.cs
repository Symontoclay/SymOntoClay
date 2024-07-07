using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
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
