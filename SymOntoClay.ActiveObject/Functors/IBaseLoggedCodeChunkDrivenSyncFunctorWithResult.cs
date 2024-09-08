namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult>
    {
        void ExecuteCodeChunksContext();

        bool IsFinished { get; }

        TResult Result { get; }
    }
}
