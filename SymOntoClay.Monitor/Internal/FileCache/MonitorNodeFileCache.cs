using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public class MonitorNodeFileCache : BaseFileCache
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly string _nodeId;

        public MonitorNodeFileCache(string rootMessagesDir, string nodeId)
            : base(Path.Combine(rootMessagesDir, nodeId))
        {
#if DEBUG
            _globalLogger.Info($"rootMessagesDir = {rootMessagesDir}");
            _globalLogger.Info($"nodeId = {nodeId}");
#endif

            _nodeId = nodeId;
        }

        public ThreadLoggerFileCache CreateThreadLoggerFileCache(string theadId)
        {
            return new ThreadLoggerFileCache(_directory, _nodeId, theadId);
        }
    }
}
