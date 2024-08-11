namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseCodeChunkWithResult<TResult>: IBaseCodeChunk
    {
        void Finish(TResult result);
    }
}
