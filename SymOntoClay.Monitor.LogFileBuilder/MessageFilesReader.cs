using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class MessageFilesReader
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)> GetFileNames(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var fileNamesList = new List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)>();

            FillUpFileNames(ref fileNamesList, targetDirectoryName, targetKindOfMessages);

            return fileNamesList;
        }

        private static void FillUpFileNames(ref List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)> result, string targetDirectoryName, IEnumerable<KindOfMessage>? targetKindOfMessages)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var filesList = Directory.GetFiles(targetDirectoryName).Select(p => (FileCacheItemInfo.GetFileInfo(p), p));

            if (targetKindOfMessages?.Any() ?? false)
            {
                filesList = filesList.Where(p => targetKindOfMessages != null && targetKindOfMessages.Contains(p.Item1.KindOfMessage));
            }

            result.AddRange(filesList);

            var subDirectories = Directory.GetDirectories(targetDirectoryName);

            foreach (var subDirectory in subDirectories)
            {
                FillUpFileNames(ref result, subDirectory, targetKindOfMessages);
            }
        }
    }
}
