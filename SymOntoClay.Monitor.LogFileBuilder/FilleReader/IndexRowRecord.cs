using SymOntoClay.Monitor.Common.Data;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public readonly record struct IndexFileRowRecord(
        string NodeId,
        string ThreadId,
        ulong MessageNumber,
        ulong GlobalMessageNumber,
        KindOfMessage KindOfMessage,
        long StartPosition,
        int DataLength,
        string FileName);
}
