namespace SymOntoClay.ActiveObject.Functors
{
    public interface IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult
    {
        void ExecuteCodeChunksContext();

        bool IsFinished {  get; }
    }
}
