namespace SymOntoClay.ActiveObject.Functors
{
    public interface ICodeChunk
    {
        void AddChild(ICodeChunk child);

        void Run();
    }
}
