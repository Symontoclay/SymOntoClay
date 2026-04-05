using System.Threading;

namespace SymOntoClay.CoreHelper.Cancellation
{
    public class CancellationTokenSourceContext : BaseCancellationContext
    {
        public CancellationTokenSourceContext()
            : this (new CancellationTokenSource())
        {
        }

        public CancellationTokenSourceContext(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <inheritdoc/>
        public override bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        /// <inheritdoc/>
        public override CancellationToken Token => _cancellationTokenSource.Token;

        /// <inheritdoc/>
        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _cancellationTokenSource.Dispose();

            base.OnDisposing();
        }
    }
}
