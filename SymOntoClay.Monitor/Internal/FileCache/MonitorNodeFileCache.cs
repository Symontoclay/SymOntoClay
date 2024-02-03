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
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly string _nodeId;

        public MonitorNodeFileCache(string absoluteDirectory, string relativeDirectory, string nodeId)
            : base(Path.Combine(absoluteDirectory, nodeId), Path.Combine(relativeDirectory, nodeId))
        {
#if DEBUG
            //_globalLogger.Info($"absoluteDirectory = {absoluteDirectory}");
            //_globalLogger.Info($"nodeId = {nodeId}");
#endif

            _nodeId = nodeId;
        }

        public ThreadLoggerFileCache CreateThreadLoggerFileCache(string theadId)
        {
            return new ThreadLoggerFileCache(_absoluteDirectory, _relativeDirectory, _nodeId, theadId);
        }
    }
}
