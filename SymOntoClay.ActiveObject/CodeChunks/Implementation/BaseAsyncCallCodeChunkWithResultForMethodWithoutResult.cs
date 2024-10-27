using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>
    {
        protected BaseAsyncCallCodeChunkWithResultForMethodWithoutResult(IBaseCodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private IBaseCodeChunksContextWithResult<TResult> _codeChunksContext;

        protected abstract IAsyncMethodResponse OnRunHandler();

        private bool _isHandlerFinished;
        private IAsyncMethodResponse _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isFinished;

        private TResult _result = default;

        public TResult Result => _result;

        public void Finish(TResult result)
        {
            _result = result;
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_isHandlerFinished)
            {
                _syncMethodResponse = OnRunHandler();

                _isHandlerFinished = true;
            }

            if (!_isSyncMethodFinished)
            {
                _syncMethodResponse.Wait();

                _isSyncMethodFinished = true;
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
