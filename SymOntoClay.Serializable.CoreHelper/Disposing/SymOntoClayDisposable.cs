using SymOntoClay.Common.Disposing;
using SymOntoClay.Serialization;

namespace SymOntoClay.Serializable.CoreHelper.Disposing
{
    [SocSerialization]
    public abstract partial class SymOntoClayDisposable : ISymOntoClayDisposable
    {
        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            OnDisposing();
        }

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_lockObj)
                {
                    return _isDisposed;
                }
            }
        }

        private bool _isDisposed;
        private object _lockObj = new object();

        protected virtual void OnDisposing()
        {
        }
    }
}
