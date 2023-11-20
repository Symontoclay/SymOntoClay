using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public class ThreadLoggerFileCache : BaseFileCache
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ThreadLoggerFileCache(string rootMessagesDir, string nodeId, string theadId)
            : base(Path.Combine(rootMessagesDir, theadId))
        {
#if DEBUG
            //_globalLogger.Info($"rootMessagesDir = {rootMessagesDir}");
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"theadId = {theadId}");
#endif
        }
    }
}
