using SymOntoClay.CoreHelper.Cancellation;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public delegate void OnceDelegate(ICancellationContext cancellationTokenContext);

    public delegate TResult OnceDelegateWithResult<TResult>(ICancellationContext cancellationTokenContext);
}
