﻿using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class ThreadLogger : MonitorLogger, IThreadLogger
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly MonitorNodeContext _monitorNodeContext;
        private readonly MessageProcessor _messageProcessor;
        private readonly MonitorFeatures _features;
        private readonly ThreadLoggerFileCache _fileCache;
        private readonly MessageNumberGenerator _globalMessageNumberGenerator;

        private readonly string _nodeId;
        private readonly string _threadId;

        private readonly MessageNumberGenerator _messageNumberGenerator = new MessageNumberGenerator();

        /// <inheritdoc/>
        public override string Id => _threadId;

        public ThreadLogger(string threadId, MonitorNodeContext monitorNodeContext)
            : base(monitorNodeContext.OutputHandler, monitorNodeContext.ErrorHandler)
        {
#if DEBUG
            _globalLogger.Info($"threadId = {threadId}");
#endif

            _monitorNodeContext = monitorNodeContext;
            _features = monitorNodeContext.Features;
            _fileCache = monitorNodeContext.FileCache.CreateThreadLoggerFileCache(threadId);

            var monitorContext = monitorNodeContext.MonitorContext;

            _messageProcessor = monitorContext.MessageProcessor;
            _globalMessageNumberGenerator = monitorContext.GlobalMessageNumberGenerator;

            _nodeId = _monitorNodeContext.NodeId;
            _threadId = threadId;

            Init(_messageProcessor, _features, monitorContext.PlatformLoggers, _fileCache, _globalMessageNumberGenerator, _messageNumberGenerator, _nodeId, _threadId);
        }
    }
}
