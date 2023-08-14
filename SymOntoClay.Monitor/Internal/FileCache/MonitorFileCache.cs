using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public class MonitorFileCache : BaseFileCache
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorFileCache(string messagesDir, string sessionName)
            : base(Path.Combine(messagesDir, sessionName))
        {
#if DEBUG
            _globalLogger.Info($"messagesDir = {messagesDir}");
            _globalLogger.Info($"sessionName = {sessionName}");
#endif
        }

        public MonitorNodeFileCache CreateMonitorNodeFileCache(string nodeId)
        {
            return new MonitorNodeFileCache(_directory, nodeId);
        }
    }
}
