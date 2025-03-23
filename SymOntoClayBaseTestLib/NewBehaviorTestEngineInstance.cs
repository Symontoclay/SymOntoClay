namespace SymOntoClay.BaseTestLib
{
    public class NewBehaviorTestEngineInstance: INewBehaviorTestEngineInstance
    {
        public NewBehaviorTestEngineInstance()
        {
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;


        }
    }
}
