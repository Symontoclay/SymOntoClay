using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    public class InternalLogger: IEntityLogger
    {
        private readonly object _lockObj = new object();
        private readonly IList<IPlatformLogger> _platformLoggers;
        private readonly string _name;
        private NLog.ILogger _nLogLogger;
        private readonly ILoggerContext _context;

        private NLog.ILogger _dbgNLogger = NLog.LogManager.GetCurrentClassLogger();

        public InternalLogger(ILoggerContext context, string name, CoreLoggerSettings settings)
        {
            _dbgNLogger.Info($"name = {name}");
            _dbgNLogger.Info($"settings = {settings}");

            _context = context;
            _name = name;
            _platformLoggers = settings.PlatformLoggers?.ToList();

            if(_platformLoggers == null)
            {
                _platformLoggers = new List<IPlatformLogger>();
            }

            var logFile = Path.Combine(settings.LogDir, $"{name}.log");

            InitNLog(logFile);
        }

        private void InitNLog(string logFile)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile, Layout = "${message}" };
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);

            var logFactory = new NLog.LogFactory(config);

            _nLogLogger = logFactory.GetCurrentClassLogger();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Log(string message)
        {
            lock(_lockObj)
            {
                if (!_context.Enable)
                {
                    return;
                }

                var now = DateTime.Now;
                var tmpCallInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
                var result = LogHelper.BuildLogString(now, KindOfLogLevel.LOG.ToString(), tmpCallInfo.FullClassName, tmpCallInfo.MethodName, message);

                WriteRaw(result);
            }
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warning(string message)
        {
            lock (_lockObj)
            {
                if(!_context.Enable)
                {
                    return;
                }

                var now = DateTime.Now;
                var tmpCallInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
                var result = LogHelper.BuildLogString(now, KindOfLogLevel.WARNING.ToString(), tmpCallInfo.FullClassName, tmpCallInfo.MethodName, message);

                WriteRaw(result);
            }
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string message)
        {
            lock (_lockObj)
            {
                if (!_context.Enable)
                {
                    return;
                }

                var now = DateTime.Now;
                var tmpCallInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
                var result = LogHelper.BuildLogString(now, KindOfLogLevel.ERROR.ToString(), tmpCallInfo.FullClassName, tmpCallInfo.MethodName, message);

                WriteRaw(result);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRaw(string message)
        {
            _nLogLogger.Info(message);

            message = $"{_name}>>{message}";

            foreach(var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLn(message);
            }
        }
    }
}
