using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public delegate void OnceDelegate(CancellationToken cancellationToken);

    public delegate TResult OnceDelegateWithResult<TResult>(CancellationToken cancellationToken);
}
