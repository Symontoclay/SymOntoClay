namespace SymOntoClay.ActiveObject.MethodResponses
{
    public interface IDrivenSyncMethodResponse
    {
        void Run();
        bool IsFinished { get; }
    }

    public interface IDrivenSyncMethodResponse<TResult>: IDrivenSyncMethodResponse
    {
        TResult Result { get; }
    }
}
