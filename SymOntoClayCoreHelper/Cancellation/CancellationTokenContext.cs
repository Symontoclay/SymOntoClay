using System.Threading;

namespace SymOntoClay.CoreHelper.Cancellation
{
    public class CancellationTokenContext: BaseCancellationContext
    {
        public CancellationTokenContext(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        private readonly CancellationToken _cancellationToken;

        /// <inheritdoc/>
        public override bool IsCancellationRequested => _cancellationToken.IsCancellationRequested;

        /// <inheritdoc/>
        public override CancellationToken Token => _cancellationToken;
    }
}
