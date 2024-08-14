namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface IBaseCodeChunksContext
    {
        void Finish();

        bool IsFinished { get; }
    }
}
