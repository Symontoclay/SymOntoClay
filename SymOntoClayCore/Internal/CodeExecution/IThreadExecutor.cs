using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IThreadExecutor
    {
        ThreadTaskStatus RunningStatus { get; }
        void Cancel();
        void Wait();

        void AddOnCompletedHandler(IOnCompletedThreadExecutorHandler handler);
        void RemoveOnCompletedHandler(IOnCompletedThreadExecutorHandler handler);
    }
}
