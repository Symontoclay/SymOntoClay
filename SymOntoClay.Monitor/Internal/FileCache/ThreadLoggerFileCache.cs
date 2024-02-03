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

        public ThreadLoggerFileCache(string absoluteDirectory, string relativeDirectory, string nodeId, string threadId)
            : base(Path.Combine(absoluteDirectory, threadId), Path.Combine(relativeDirectory, threadId))
        {
#if DEBUG
            //_globalLogger.Info($"absoluteDirectory = {absoluteDirectory}");
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
#endif
        }
    }
}
