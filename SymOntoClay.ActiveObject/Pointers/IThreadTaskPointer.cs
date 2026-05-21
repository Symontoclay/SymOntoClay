using SymOntoClay.Threading;

namespace SymOntoClay.ActiveObject.Pointers
{
    public interface IThreadTaskPointer
    {
        IThreadTask Task { get; set; }
    }

    public interface IThreadTaskPointer<TResult>: IThreadTaskPointer
    {
        IThreadTask<TResult> TaskWithResult { get; set; }
    }
}
