using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class LoggerImpementation: IEntityLogger
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [MethodForLoggingSupport]
        public void Log(string message)
        {
            var callInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, KindOfLogLevel.LOG.ToString(), callInfo.FullClassName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        [MethodForLoggingSupport]
        public void Warning(string message)
        {
            var callInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, KindOfLogLevel.WARNING.ToString(), callInfo.FullClassName, callInfo.MethodName, message);

            _logger.Warn(result);
        }

        [MethodForLoggingSupport]
        public void Error(string message)
        {
            var callInfo = DiagnosticsHelper.GetNotLoggingSupportCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, KindOfLogLevel.ERROR.ToString(), callInfo.FullClassName, callInfo.MethodName, message);

            _logger.Error(result);
        }

        private enum KindOfLogLevel
        {
            LOG,
            WARNING,
            ERROR
        }
    }
}
