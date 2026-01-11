/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Monitor.NLog.PlatformLoggers
{
    public class CommonNLogPlatformLogger : IPlatformLogger
    {
        private static readonly CommonNLogPlatformLogger __instance = new CommonNLogPlatformLogger();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static CommonNLogPlatformLogger Instance => __instance;

        private CommonNLogPlatformLogger()
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawOutput(string message)
        {
            //throw new NotImplementedException(message);

            _logger.Info(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawTrace(string messagePointId, string message)
        {
            _logger.Trace($"{messagePointId} {message}");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawDebug(string messagePointId, string message)
        {
            _logger.Debug($"{messagePointId} {message}");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawInfo(string messagePointId, string message)
        {
            _logger.Info($"{messagePointId} {message}");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawWarn(string messagePointId, string message)
        {
            _logger.Warn($"{messagePointId} {message}");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawError(string messagePointId, string message)
        {
            _logger.Error($"{messagePointId} {message}");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WriteLnRawFatal(string messagePointId, string message)
        {
            _logger.Fatal($"{messagePointId} {message}");
        }
    }
}
