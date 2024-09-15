using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>
    {
        protected BaseAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(IBaseCodeChunksContext codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private IBaseCodeChunksContext _codeChunksContext;

        public void Finish()
        {
            _isFinished = true;
            _codeChunksContext.Finish();
        }

        protected abstract IAsyncMethodResponse<MethodResult> OnRunPreHandler();
        protected abstract void OnRunPostHandler(MethodResult methodResult);

        private bool _isPreHandlerFinished;
        private IAsyncMethodResponse<MethodResult> _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isPostHandlerFinished;
        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_isPreHandlerFinished)
            {
                _syncMethodResponse = OnRunPreHandler();

                _isPreHandlerFinished = true;
            }

            if (!_isSyncMethodFinished)
            {
                _syncMethodResponse.Wait();

                _isSyncMethodFinished = true;
            }

            if (!_isPostHandlerFinished)
            {
                OnRunPostHandler(_syncMethodResponse.Result);

                _isPostHandlerFinished = true;
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
