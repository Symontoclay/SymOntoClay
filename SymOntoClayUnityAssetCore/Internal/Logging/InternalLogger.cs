/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    public class InternalLogger : IEntityLogger
    {
        private readonly object _lockObj = new object();
        private readonly IList<IPlatformLogger> _platformLoggers;
        private readonly string _name;
        private NLog.ILogger _nLogLogger;
        private readonly ILoggerContext _context;

#if DEBUG
        private NLog.ILogger _dbgNLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public InternalLogger(ILoggerContext context, string name, CoreLoggerSettings settings)
        {
#if DEBUG
            //_dbgNLogger.Info($"name = {name}");
            //_dbgNLogger.Info($"settings = {settings}");
#endif
            _context = context;
            _name = name;
            _platformLoggers = settings.PlatformLoggers?.ToList();

            if (_platformLoggers == null)
            {
                _platformLoggers = new List<IPlatformLogger>();
            }

            if(!string.IsNullOrWhiteSpace(settings.LogDir))
            {
                var logFile = Path.Combine(settings.LogDir, $"{name}.log");

                InitNLog(logFile);
            }
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
            lock (_lockObj)
            {
                if (!_context.Enable)
                {
                    return;
                }

                WriteRawLog(message);

                var now = DateTime.Now;
                var tmpCallInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
                var result = LogHelper.BuildLogString(now, KindOfLogLevel.LOG.ToString(), tmpCallInfo.FullClassName, tmpCallInfo.MethodName, message);

                WriteRaw(result);
            }
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void LogChannel(string message)
        {
            lock (_lockObj)
            {
                if (!_context.Enable)
                {
                    return;
                }

                WriteRawLogChannel(message);
            }
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warning(string message)
        {
            lock (_lockObj)
            {
                if (!_context.Enable)
                {
                    return;
                }

                WriteRawWarning(message);

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

                WriteRawError(message);

                var now = DateTime.Now;
                var tmpCallInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
                var result = LogHelper.BuildLogString(now, KindOfLogLevel.ERROR.ToString(), tmpCallInfo.FullClassName, tmpCallInfo.MethodName, message);

                WriteRaw(result);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRaw(string message)
        {
            _nLogLogger?.Info(message);

            message = $"{_name}>>{message}";

            foreach (var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLn(message);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRawLog(string message)
        {
            foreach (var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLnRawLog(message);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRawLogChannel(string message)
        {
            _nLogLogger?.Info(message);

            foreach (var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLnRawLogChannel(message);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRawWarning(string message)
        {
            foreach (var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLnRawWarning(message);
            }
        }

        [MethodForLoggingSupport]
        private void WriteRawError(string message)
        {
            foreach (var platformLogger in _platformLoggers)
            {
                platformLogger.WriteLnRawError(message);
            }
        }
    }
}
