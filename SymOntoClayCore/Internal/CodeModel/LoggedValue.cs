using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class LoggedValue : Value
    {
        protected LoggedValue(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public IEntityLogger Logger => _logger;

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Log(string message)
        {
            _logger.Log(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void LogChannel(string message)
        {
            _logger.LogChannel(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Warning(string message)
        {
            _logger.Warning(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Error(string message)
        {
            _logger.Error(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Error(Exception e)
        {
            _logger.Error(e.ToString());
        }
    }
}
