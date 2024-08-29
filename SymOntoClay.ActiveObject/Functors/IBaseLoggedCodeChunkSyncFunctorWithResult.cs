namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseLoggedCodeChunkSyncFunctorWithResult<TResult>
    {
        void ExecuteCodeChunksContext();

        bool IsFinished { get; }

        TResult Result { get; }
    }
}
