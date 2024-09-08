using SymOntoClay.ActiveObject.Functors;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class SyncMethodResponse: ISyncMethodResponse
    {
        public SyncMethodResponse(IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult functor)
        {
            _functor = functor;
        }

        private IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult _functor;

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
        public SyncMethodResponse(IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult> functor)
        {
            _functor = functor;
        }

        private IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult> _functor;

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
