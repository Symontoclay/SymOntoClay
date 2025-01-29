using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IThreadExecutor
    {
        ThreadTaskStatus RunningStatus { get; }
        void Cancel();
    }
}
