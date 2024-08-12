namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public abstract partial class BaseCodeChunk: ICodeChunk
    {
        protected abstract void OnRunAction();

        private bool _isFinished;

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
