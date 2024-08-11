namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseCodeChunksContextWithResult<TResult>
    {
        void Finish(TResult result);
    }
}
