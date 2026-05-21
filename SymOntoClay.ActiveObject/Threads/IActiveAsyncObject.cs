using SymOntoClay.ActiveObject.Pointers;
using SymOntoClay.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IActiveAsyncObject : IActiveObject
    {
        IThreadTaskPointer TaskValue { get; }
        IThreadTaskPointer Start();
    }
}
