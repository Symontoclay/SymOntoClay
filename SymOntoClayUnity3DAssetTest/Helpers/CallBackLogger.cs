using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public class CallBackLogger : IPlatformLogger
    {
        public CallBackLogger(Action<string> logChannel, Action<string> error)
        {
            _logChannel = logChannel;
            _error = error;
        }

        private readonly Action<string> _logChannel;
        private readonly Action<string> _error;

        /// <inheritdoc/>
        public void WriteLn(string message)
        {
        }

        /// <inheritdoc/>
        public void WriteLnRawLogChannel(string message)
        {
            _logChannel?.Invoke(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawLog(string message)
        {
        }

        /// <inheritdoc/>
        public void WriteLnRawWarning(string message)
        {
        }

        /// <inheritdoc/>
        public void WriteLnRawError(string message)
        {
            _error?.Invoke(message);
        }
    }
}
