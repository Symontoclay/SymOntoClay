/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.BaseTestLib
{
    public class EmptyLogger : IMonitorLogger, IPlatformLogger
    {
        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void Error(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void LogChannel(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void Log(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void Warning(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLn(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawLogChannel(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawLog(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawWarning(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawError(string message)
        {
        }
    }
}
