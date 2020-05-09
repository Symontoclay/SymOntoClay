using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class LoggerImpementation: ILogger
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [MethodForLoggingSupport]
        public void Log(string message)
        {
            _logger.Info(message);
        }

        [MethodForLoggingSupport]
        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        [MethodForLoggingSupport]
        public void Error(string message)
        {
            _logger.Error(message);
        }
    }
}
