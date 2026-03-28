using SymOntoClay.Monitor.Common.Data;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public readonly record struct LogFileRowRecord(
        string NodeId,
        string ThreadId,
        ulong MessageNumber,
        ulong GlobalMessageNumber,
        KindOfMessage KindOfMessage,
        int DataLength,
        byte[] Data,
        string FileName);
}
