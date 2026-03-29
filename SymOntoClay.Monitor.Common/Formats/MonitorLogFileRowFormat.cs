using System;
using System.IO;
using System.Threading;

namespace SymOntoClay.Monitor.Common.Formats
{
    public static class MonitorLogFileRowFormat
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private const byte _startMarker = 0xAA;
        private const byte _endMarker = 0x55;

        public static void Write(BinaryWriter writer, string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, int kindOfMessage, byte[] data)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
            //_globalLogger.Info($"messageNumber = {messageNumber}");
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
            //_globalLogger.Info($"data.Length = {data.Length}");
#endif

            writer.Write(_startMarker);

            if (nodeId == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(nodeId);
            }

            if (threadId == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(threadId);
            }

            writer.Write(messageNumber);
            writer.Write(globalMessageNumber);
            writer.Write(kindOfMessage);
            writer.Write(data.Length);
            writer.Write(data);
            writer.Write(_endMarker);
        }

        public static LogFileRowRecord Read(BinaryReader reader)
        {
            var startByte = reader.ReadByte();

#if DEBUG
            _globalLogger.Info($"startByte = {startByte.ToString("X2")}");
#endif

            var hasNodeId = reader.ReadBoolean();

#if DEBUG
            _globalLogger.Info($"hasNodeId = {hasNodeId}");
#endif

            string nodeId = null;

            if (hasNodeId)
            {
                nodeId = reader.ReadString();

#if DEBUG
                _globalLogger.Info($"nodeId = {nodeId}");
#endif
            }

            var hasThreadId = reader.ReadBoolean();

#if DEBUG
            _globalLogger.Info($"hasThreadId = {hasThreadId}");
#endif

            string threadId = null;

            if (hasThreadId)
            {
                threadId = reader.ReadString();

#if DEBUG
                _globalLogger.Info($"threadId = {threadId}");
#endif
            }

            var messageNumber = reader.ReadUInt64();

#if DEBUG
            _globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = reader.ReadUInt64();

#if DEBUG
            _globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var kindOfMessage = reader.ReadInt32();

#if DEBUG
            _globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            var dataLength = reader.ReadInt32();

#if DEBUG
            _globalLogger.Info($"dataLength = {dataLength}");
#endif

            var data = reader.ReadBytes(dataLength);

#if DEBUG
            //_globalLogger.Info($" = {}");
#endif

            var endByte = reader.ReadByte();

#if DEBUG
            _globalLogger.Info($"endByte = {endByte.ToString("X2")}");
#endif

            return new LogFileRowRecord(
                NodeId: nodeId,
                ThreadId: threadId,
                MessageNumber: messageNumber,
                GlobalMessageNumber: globalMessageNumber,
                KindOfMessage: kindOfMessage,
                DataLength: dataLength,
                Data: data
            );
        }
    }
}
