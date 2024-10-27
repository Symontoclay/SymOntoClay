namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseCodeChunkWithResult<TResult>
    {
        protected BaseCodeChunkWithResult(IBaseCodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }
        
        protected abstract void OnRunAction();

        private bool _isFinished;
        private IBaseCodeChunksContextWithResult<TResult> _codeChunksContext;

        public void Finish(TResult result)
        {
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            OnRunAction();

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
