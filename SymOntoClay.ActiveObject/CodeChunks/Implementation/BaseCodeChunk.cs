namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseCodeChunk
    {
        public void Finish()
        {
            _isFinished = true;
        }

        protected abstract void OnRunAction();

        private bool _isFinished;

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
