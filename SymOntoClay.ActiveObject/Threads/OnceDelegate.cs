using SymOntoClay.Common.Cancellation;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public delegate void OnceDelegate(ICancellationContext cancellationContext);

    public delegate TResult OnceDelegateWithResult<TResult>(ICancellationContext cancellationContext);
}
