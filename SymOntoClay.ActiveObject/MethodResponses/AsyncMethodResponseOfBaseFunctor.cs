using SymOntoClay.ActiveObject.Functors;
using System.Threading.Tasks;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class AsyncMethodResponseOfBaseFunctor : IAsyncMethodResponse
    {
        public AsyncMethodResponseOfBaseFunctor(BaseFunctor source)
        {
            _source = source;
        }

        private BaseFunctor _source;

        /// <inheritdoc/>
        public Task Task => _source.TaskValue.StandardTask;

        /// <inheritdoc/>
        public void Wait()
        {
            _source.TaskValue.StandardTask.Wait();
        }
    }

    public class AsyncMethodResponseOfBaseFunctor<TResult> : IAsyncMethodResponse<TResult>
    {
        public AsyncMethodResponseOfBaseFunctor(BaseFunctor<TResult> source)
        {
            _source = source;
        }

        private BaseFunctor<TResult> _source;

        /// <inheritdoc/>
        public Task<TResult> Task => _source.TaskValue.StandardTaskWithResult;

        /// <inheritdoc/>
        public TResult Result => _source.Result;

        /// <inheritdoc/>
        public void Wait()
        {
            _source.TaskValue.StandardTask.Wait();
        }
    }
}
