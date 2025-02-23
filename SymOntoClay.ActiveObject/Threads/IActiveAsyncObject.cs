using SymOntoClay.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IActiveAsyncObject : IActiveObject
    {
        IThreadTask TaskValue { get; }
        IThreadTask Start();
    }
}
