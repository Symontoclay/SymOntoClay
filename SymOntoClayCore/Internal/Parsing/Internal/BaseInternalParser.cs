using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseInternalParser
    {
        protected BaseInternalParser(InternalParserContext context)
        {
            _context = context;
            _logger = context.Logger;
        }

        protected readonly InternalParserContext _context;
        private ILogger _logger;

        public void Run()
        {
            OnEnter();

            while ((_currToken = _context.GetToken()) != null)
            {
                OnRun();

                if (_isExited)
                {
                    OnFinish();
                    return;
                }
            }

            OnFinish();
            OnFinishByEmpty();
        }

        protected Token _currToken = null;
        private bool _isExited = false;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnRun()
        {
        }

        protected virtual void OnFinish()
        {
        }

        protected virtual void OnFinishByEmpty()
        {
        }

        protected void Exit()
        {
            _isExited = true;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Log(string message)
        {
            _logger.Log(message);
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
    }
}
