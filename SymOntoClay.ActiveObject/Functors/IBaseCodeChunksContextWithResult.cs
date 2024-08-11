namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseCodeChunksContextWithResult<TResult>: IBaseCodeChunk
    {
        void Finish(TResult result);
    }
}
