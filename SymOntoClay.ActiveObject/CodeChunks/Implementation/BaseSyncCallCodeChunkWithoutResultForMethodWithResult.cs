using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseSyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>
    {
        protected abstract IDrivenSyncMethodResponse<MethodResult> OnRunPreHandler();
        protected abstract void OnRunPostHandler(MethodResult methodResult); 

        private bool _isPreHandlerFinished;
        private IDrivenSyncMethodResponse<MethodResult> _syncMethodResponse;
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
                _syncMethodResponse.Run();

                _isSyncMethodFinished = true;
            }

            if(!_isPostHandlerFinished)
            {
                OnRunPostHandler(_syncMethodResponse.Result);

                _isPostHandlerFinished = true;
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
