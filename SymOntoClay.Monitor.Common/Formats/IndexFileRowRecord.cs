namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

namespace SymOntoClay.Monitor.Common.Formats
{
    public readonly record struct IndexFileRowRecord(
        string NodeId,
        string ThreadId,
        ulong MessageNumber,
        ulong GlobalMessageNumber,
        int KindOfMessage,
        long StartPosition,
        int DataLength);
}
