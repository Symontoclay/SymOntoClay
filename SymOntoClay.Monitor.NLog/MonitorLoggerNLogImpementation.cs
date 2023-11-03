using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.NLog
{
    public class MonitorLoggerNLogImpementation: IMonitorLogger
    {
        /// <summary>
        /// Gets default instance of the class.
        /// </summary>
        public static MonitorLoggerNLogImpementation Instance = new MonitorLoggerNLogImpementation();

        public MonitorLoggerNLogImpementation()
            : this(LogManager.GetCurrentClassLogger())
        {
        }

        public MonitorLoggerNLogImpementation(Logger logger)
        {
            _logger = logger;
        }

        private readonly Logger _logger;

        /// <inheritdoc/>
        public string Id => "MonitorLoggerNLogImpementation";

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Trace", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Debug", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Info", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Warn", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Error", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Error(messagePointId, exception?.ToString(), memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Fatal", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Fatal(messagePointId, exception?.ToString(), memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
