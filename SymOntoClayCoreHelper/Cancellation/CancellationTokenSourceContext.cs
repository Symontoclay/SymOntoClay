using System.Threading;

namespace SymOntoClay.CoreHelper.Cancellation
{
    public class CancellationTokenSourceContext : BaseCancellationContext
    {
        public CancellationTokenSourceContext(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <inheritdoc/>
        public override bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        /// <inheritdoc/>
        public override CancellationToken Token => _cancellationTokenSource.Token;
    }
}
