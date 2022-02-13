/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        public void LogChannel(string message)
        {
            Log(message);
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
