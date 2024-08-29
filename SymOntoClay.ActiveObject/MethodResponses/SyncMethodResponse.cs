using SymOntoClay.ActiveObject.Functors;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class SyncMethodResponse: ISyncMethodResponse
    {
        public SyncMethodResponse(IBaseLoggedCodeChunkSyncFunctorWithoutResult functor)
        {
            _functor = functor;
        }

        private IBaseLoggedCodeChunkSyncFunctorWithoutResult _functor;

        /// <inheritdoc/>
        public void Run()
        {
            _functor.ExecuteCodeChunksContext();
        }

        /// <inheritdoc/>
        public bool IsFinished => _functor.IsFinished;
    }

    public class SyncMethodResponse<TResult> : ISyncMethodResponse<TResult>
    {
        public SyncMethodResponse(IBaseLoggedCodeChunkSyncFunctorWithResult<TResult> functor)
        {
            _functor = functor;
        }

        private IBaseLoggedCodeChunkSyncFunctorWithResult<TResult> _functor;

        /// <inheritdoc/>
        public void Run()
        {
            _functor.ExecuteCodeChunksContext();
        }

        /// <inheritdoc/>
        public bool IsFinished => _functor.IsFinished;

        /// <inheritdoc/>
        public TResult Result => _functor.Result;
    }
}
