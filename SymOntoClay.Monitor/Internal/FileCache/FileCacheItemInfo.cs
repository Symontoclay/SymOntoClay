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

        public static string FileExt = ".soc_msg";

        public static string GetFileName(string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, KindOfMessage kindOfMessage)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
            //_globalLogger.Info($"messageNumber = {messageNumber}");
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            return $"{PrepareString(nodeId)}_{PrepareString(threadId)}_{messageNumber}_{globalMessageNumber}_{(int)kindOfMessage}{FileExt}";
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

            if(pos > -1)
            {
                nodeId = name.Substring(0, pos);
            }            

            var oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos (2) = {pos}");
#endif

            if(pos > -1)
            {
                threadId = name.Substring(oldPos + 1, pos - oldPos - 1);
            }            

            oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos (3) = {pos}");
#endif

            if(pos > -1)
            {
                messageNumber = ulong.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));
            }            

            oldPos = pos;

            pos = name.IndexOf('_', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos (4) = {pos}");
#endif

            if(pos > -1)
            {
                globalMessageNumber = ulong.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));
            }

            oldPos = pos;

            pos = name.IndexOf('.', oldPos + 1);

#if DEBUG
            //_globalLogger.Info($"pos (5) = {pos}");
#endif

            if(pos > -1)
            {
                kindOfMessage = (KindOfMessage)int.Parse(name.Substring(oldPos + 1, pos - oldPos - 1));
            }

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
