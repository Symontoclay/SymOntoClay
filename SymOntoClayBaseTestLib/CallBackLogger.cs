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

using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.BaseTestLib
{
    public class CallBackLogger : IPlatformLogger
    {
        public CallBackLogger(Action<string> logChannel, Action<string> error, bool enableWriteLnRawLog = false)
        {
            _enableWriteLnRawLog = enableWriteLnRawLog;
            _logChannel = logChannel;
            _error = error;
        }

        private readonly bool _enableWriteLnRawLog;
        private readonly Action<string> _logChannel;
        private readonly Action<string> _error;

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLn(string message)
        {
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawLogChannel(string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawLog(string message)
        {
            if (_enableWriteLnRawLog)
            {
                _logChannel?.Invoke(message);
            }
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawWarning(string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public void WriteLnRawError(string message)
        {
            _error?.Invoke(message);
        }
    }
}
