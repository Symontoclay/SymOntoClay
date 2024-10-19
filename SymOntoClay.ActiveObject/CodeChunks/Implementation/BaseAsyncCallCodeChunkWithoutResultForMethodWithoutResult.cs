using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        protected abstract IAsyncMethodResponse OnRunHandler();

        private bool _isHandlerFinished;
        private IAsyncMethodResponse _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isFinished;

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
