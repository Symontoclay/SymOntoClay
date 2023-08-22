using NLog;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SymOntoClay.Monitor.NLog
{
    public class MonitorNodeNLogImpementation: MonitorLoggerNLogImpementation, IMonitorNode
    {
        public MonitorNodeNLogImpementation()
            : this(LogManager.GetCurrentClassLogger())
        {
        }

        public MonitorNodeNLogImpementation(Logger logger)
            : base(logger)
        {
            _logger = logger;
            _threadLogger = new ThreadLoggerNLogImpementation(_logger);
        }

        private readonly Logger _logger;
        private readonly ThreadLoggerNLogImpementation _threadLogger;

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _threadLogger;
        }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentTheadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _threadLogger;
        }
    }
}
