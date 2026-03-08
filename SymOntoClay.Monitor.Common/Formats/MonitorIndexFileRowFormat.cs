using System.IO;

namespace SymOntoClay.Monitor.Common.Formats
{
    public static class MonitorIndexFileRowFormat
    {
        public static void Write(BinaryWriter writer, string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, int kindOfMessage, long startPosition, int dataLength)
        {
            writer.Write(nodeId?.Length ?? 0);

            if (nodeId != null)
            {
                writer.Write(nodeId);
            }

            writer.Write(threadId?.Length ?? 0);

            if (threadId != null)
            {
                writer.Write(threadId);
            }

            writer.Write(messageNumber);
            writer.Write(globalMessageNumber);
            writer.Write((int)kindOfMessage);
            writer.Write(startPosition);
            writer.Write(dataLength);
        }
    }
}
