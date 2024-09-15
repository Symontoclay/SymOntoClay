using System.Threading.Tasks;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class CompletedAsyncMethodResponse : IAsyncMethodResponse
    {
        public static CompletedAsyncMethodResponse Instance = new CompletedAsyncMethodResponse();

        /// <inheritdoc/>
        public Task Task => Task.CompletedTask;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }

    public class CompletedAsyncMethodResponse<TResult> : IAsyncMethodResponse<TResult>
    {
        public static CompletedAsyncMethodResponse<TResult> Instance = new CompletedAsyncMethodResponse<TResult>();

        public CompletedAsyncMethodResponse()
            : this(default(TResult))
        {
        }

        public CompletedAsyncMethodResponse(TResult result)
        {
            _result = result;
        }

        private readonly TResult _result;

        /// <inheritdoc/>
        public Task<TResult> Task => Task<TResult>.FromResult(_result);

        /// <inheritdoc/>
        public TResult Result => _result;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}
