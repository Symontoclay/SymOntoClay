namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseLoggedCodeChunkSyncFunctorWithoutResult
    {
        void ExecuteCodeChunksContext();

        bool IsFinished {  get; }
    }
}
