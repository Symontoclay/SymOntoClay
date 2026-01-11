/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class MessageFilesReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private const int _nodeLevel = 2;
        private const int _threadLevel = 3;

        public static List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)> GetFileNames(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var fileNamesList = new List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)>();

            FillUpFileNames(ref fileNamesList, targetDirectoryName, targetKindOfMessages, 1, targetNodes, targetThreads);

            return fileNamesList;
        }

        private static void FillUpFileNames(ref List<((string NodeId, string ThreadId, ulong MessageNumber, ulong GlobalMessageNumber, KindOfMessage KindOfMessage), string)> result, string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, int levelNum, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
            //_globalLogger.Info($"levelNum = {levelNum}");
#endif

            switch(levelNum)
            {
                case _nodeLevel:
                    if(targetNodes != null)
                    {
                        var directoryInfo = new DirectoryInfo(targetDirectoryName);

#if DEBUG
                        //_globalLogger.Info($"directoryInfo.Name = {directoryInfo.Name}");
#endif

                        if(!targetNodes.Contains(directoryInfo.Name))
                        {
                            return;
                        }
                    }
                    break;

                case _threadLevel:
                    if(targetThreads != null)
                    {
                        var directoryInfo = new DirectoryInfo(targetDirectoryName);

#if DEBUG
                        //_globalLogger.Info($"directoryInfo.Name = {directoryInfo.Name}");
#endif

                        if(!targetThreads.Contains(directoryInfo.Name))
                        {
                            return;
                        }
                    }
                    break;

                default:
                    break;
            }

            var filesList = Directory.GetFiles(targetDirectoryName).Where(p => p.EndsWith(FileCacheItemInfo.FileExt)).Select(p => (FileCacheItemInfo.GetFileInfo(p), p));

            if (targetKindOfMessages?.Any() ?? false)
            {
                filesList = filesList.Where(p => targetKindOfMessages != null && targetKindOfMessages.Contains(p.Item1.KindOfMessage));
            }

            result.AddRange(filesList);

            var subDirectories = Directory.GetDirectories(targetDirectoryName);

            var nextLevelNum = levelNum + 1;

            foreach (var subDirectory in subDirectories)
            {
                FillUpFileNames(ref result, subDirectory, targetKindOfMessages, nextLevelNum, targetNodes, targetThreads);
            }
        }
    }
}
