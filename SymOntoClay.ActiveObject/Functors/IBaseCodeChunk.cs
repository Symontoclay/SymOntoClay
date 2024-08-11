namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseCodeChunk
    {
        void Run();

        bool IsFinished { get; }
    }
}
