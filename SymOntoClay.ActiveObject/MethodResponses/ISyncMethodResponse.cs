namespace SymOntoClay.ActiveObject.MethodResponses
{
    public interface ISyncMethodResponse
    {
    }

    public interface ISyncMethodResponse<TResult> : ISyncMethodResponse
    {
        TResult Result { get; }
    }
}
