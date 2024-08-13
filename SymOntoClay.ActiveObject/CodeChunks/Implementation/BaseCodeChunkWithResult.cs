namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseCodeChunkWithResult<TResult> : ICodeChunkWithResult<TResult>
    {
        protected BaseCodeChunkWithResult(ICodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        protected abstract void OnRunAction();

        private bool _isFinished;
        private ICodeChunksContextWithResult<TResult> _codeChunksContext;

        /// <inheritdoc/>
        public void Finish(TResult result)
        {
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

        /// <inheritdoc/>
        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            OnRunAction();

            _isFinished = true;
        }

        /// <inheritdoc/>
        public bool IsFinished => _isFinished;
    }
}
