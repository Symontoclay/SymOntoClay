/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public abstract class BaseLoggedComponent
    {
        protected BaseLoggedComponent(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public IMonitorLogger Logger => _logger;

        [MethodForLoggingSupport]
        protected void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Output(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Trace(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Debug(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Warn(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Fatal(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Fatal(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        [Obsolete("Use Info", true)]
        protected void Log(string message)
        {
            //_logger.Log(message);
        }
    }
}
