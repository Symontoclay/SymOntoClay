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

using SymOntoClay.Monitor.Common;

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
        public void WriteLnRawOutput(string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawTrace(string messagePointId, string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawDebug(string messagePointId, string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawInfo(string messagePointId, string message)
        {
            if (_enableWriteLnRawLog)
            {
                _logChannel?.Invoke(message);
            }
        }

        /// <inheritdoc/>
        public void WriteLnRawWarn(string messagePointId, string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawError(string messagePointId, string message)
        {
            _error?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawFatal(string messagePointId, string message)
        {
            _error?.Invoke(message);
        }
    }
}
