namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface IBaseCodeChunksContextWithResult<TResult>
    {
        void Finish(TResult result);

        bool IsFinished { get; }
    }
}
