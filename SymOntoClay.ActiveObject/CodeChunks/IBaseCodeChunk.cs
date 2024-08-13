namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface IBaseCodeChunk
    {
        void Run();

        bool IsFinished { get; }
    }
}
