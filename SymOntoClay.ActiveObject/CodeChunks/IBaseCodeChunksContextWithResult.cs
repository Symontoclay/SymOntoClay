namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface IBaseCodeChunksContextWithResult<TResult> : IBaseCodeChunk
    {
        void Finish(TResult result);
    }
}
