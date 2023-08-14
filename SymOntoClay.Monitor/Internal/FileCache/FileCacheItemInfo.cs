using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public static class FileCacheItemInfo
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static string PrepareString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Replace("_", "-").Replace(" ", "-").Trim();
        }

        public static string GetFileName(string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, KindOfMessage kindOfMessage)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
            //_globalLogger.Info($"messageNumber = {messageNumber}");
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            return $"{PrepareString(nodeId)}_{PrepareString(threadId)}_{messageNumber}_{globalMessageNumber}_{(int)kindOfMessage}.json";
        }

        public static (string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage) GetFileInfo(string fileName)
        {
#if DEBUG
            //_globalLogger.Info($"fileName = {fileName}");
#endif

            var slashPos = Math.Max(fileName.LastIndexOf('\\'), fileName.LastIndexOf('/'));

#if DEBUG
            //_globalLogger.Info($"slashPos = {slashPos}");
#endif

            var name = fileName.Substring(slashPos + 1);

#if DEBUG
            //_globalLogger.Info($"name = {name}");
#endif

            var nodeId = string.Empty;
            var threadId = string.Empty;
            ulong messageNumber = 0;
            ulong globalMessageNumber = 0;
            var kindOfMessage = KindOfMessage.Unknown;

            var pos = name.IndexOf('_');

#if DEBUG
            //_globalLogger.Info($"pos = {pos}");
#endif

            nodeId = name.Substring(0, pos);

            var oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos = {pos}");
#endif

            threadId = name.Substring(oldPos + 1, pos - oldPos - 1);

            oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos = {pos}");
#endif

            messageNumber = ulong.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));

            oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos = {pos}");
#endif

            globalMessageNumber = ulong.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));

            oldPos = pos;

            pos = name.IndexOf('.', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos = {pos}");
#endif

            kindOfMessage = (KindOfMessage)int.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));

#if DEBUG
            //_globalLogger.Info($"nodeId = '{nodeId}'");
            //_globalLogger.Info($"threadId = '{threadId}'");
            //_globalLogger.Info($"messageNumber = {messageNumber}");
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            return (nodeId, threadId, messageNumber, globalMessageNumber, kindOfMessage);
        }
    }
}
