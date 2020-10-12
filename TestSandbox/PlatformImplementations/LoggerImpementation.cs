/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
