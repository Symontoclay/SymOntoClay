using System;
using System.IO;
using System.Threading;

namespace SymOntoClay.Monitor.Common.Formats
{
    public static class MonitorIndexFileRowFormat
    {
#if DEBUG
        //private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private static int _lengthForNull = -1;

        public static void Write(BinaryWriter writer, string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, int kindOfMessage, long startPosition, int dataLength)
        {
            writer.Write(nodeId?.Length ?? _lengthForNull);

            if (nodeId != null)
            {
                writer.Write(nodeId);
            }

            writer.Write(threadId?.Length ?? _lengthForNull);

            if (threadId != null)
            {
                writer.Write(threadId);
            }

            writer.Write(messageNumber);
            writer.Write(globalMessageNumber);
            writer.Write(kindOfMessage);
            writer.Write(startPosition);
            writer.Write(dataLength);
        }

        public static IndexFileRowRecord Read(BinaryReader reader)
        {
            var nodeIdLength = reader.ReadInt32();

#if DEBUG
            //_globalLogger.Info($"nodeIdLength = {nodeIdLength}");
#endif

            string nodeId = null;

            if (nodeIdLength != _lengthForNull)
            {
                nodeId = reader.ReadString();
            }

#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
#endif

            var threadIdLength = reader.ReadInt32();

#if DEBUG
            //_globalLogger.Info($"threadIdLength = {threadIdLength}");
#endif

            string threadId = null;

            if(threadIdLength != _lengthForNull)
            {
                threadId = reader.ReadString();
            }

#if DEBUG
            //_globalLogger.Info($"threadId = {threadId}");
#endif

            var messageNumber = reader.ReadUInt64();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = reader.ReadUInt64();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var kindOfMessage = reader.ReadInt32();

#if DEBUG
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            var startPosition = reader.ReadInt64();

#if DEBUG
            //_globalLogger.Info($"startPosition = {startPosition}");
#endif

            var dataLength = reader.ReadInt32();

#if DEBUG
            //_globalLogger.Info($"dataLength = {dataLength}");
#endif

            return new IndexFileRowRecord(
                NodeId: nodeId,
                ThreadId: threadId,
                MessageNumber: messageNumber,
                GlobalMessageNumber: globalMessageNumber,
                KindOfMessage: kindOfMessage,
                StartPosition: startPosition,
                DataLength: dataLength
            );
        }
    }
}
