using SymOntoClay.Monitor.Common.Data;

namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IFileWriter
    {
        void WriteData(string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, KindOfMessage kindOfMessage, byte[] data);
    }
}
