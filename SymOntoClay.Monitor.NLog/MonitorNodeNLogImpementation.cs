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

using NLog;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SymOntoClay.Monitor.NLog
{
    public class MonitorNodeNLogImpementation: MonitorLoggerNLogImpementation, IMonitorNode
    {
        public MonitorNodeNLogImpementation()
            : this(LogManager.GetCurrentClassLogger())
        {
        }

        public MonitorNodeNLogImpementation(Logger logger)
            : base(logger)
        {
            _logger = logger;
            _threadLogger = new ThreadLoggerNLogImpementation(_logger);
        }

        private readonly Logger _logger;
        private readonly ThreadLoggerNLogImpementation _threadLogger;

        /// <inheritdoc/>
        public bool Enable { get; set; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get; set; }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _threadLogger;
        }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentTheadId, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _threadLogger;
        }
    }
}
