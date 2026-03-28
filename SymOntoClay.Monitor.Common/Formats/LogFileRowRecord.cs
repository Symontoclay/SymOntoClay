namespace SymOntoClay.Monitor.Common.Formats
{
    public readonly record struct LogFileRowRecord(
        string NodeId,
        string ThreadId,
        ulong MessageNumber,
        ulong GlobalMessageNumber,
        int KindOfMessage,
        int DataLength,
        byte[] Data
    );
}
