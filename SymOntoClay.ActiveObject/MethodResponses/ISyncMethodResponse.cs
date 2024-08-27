namespace SymOntoClay.ActiveObject.MethodResponses
{
    public interface ISyncMethodResponse
    {
        void Run();
        bool IsFinished { get; }
    }

    public interface ISyncMethodResponse<TResult>: ISyncMethodResponse
    {
        TResult Result { get; }
    }
}
