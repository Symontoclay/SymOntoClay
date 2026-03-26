namespace SymOntoClay.Monitor.Common.Formats
{
    public readonly record struct ELogRecord
    (
        string NodeId,
        string ThreadId,
        ulong MessageNumber,
        ulong GlobalMessageNumber,
        int KindOfMessage
    );
}
