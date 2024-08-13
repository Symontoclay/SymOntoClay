namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface IBaseCodeChunkWithResult<TResult> : IBaseCodeChunk
    {
        void Finish(TResult result);
    }
}
