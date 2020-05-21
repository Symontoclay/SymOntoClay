using SymOntoClay.Core.Internal.CodeModel;
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
            _dictionary = context.Dictionary;
        }

        protected readonly InternalParserContext _context;
        private IEntityLogger _logger;
        private IEntityDictionary _dictionary;

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

        protected ulong GetKey(string name)
        {
            return _dictionary.GetKey(name);
        }

        protected Name ParseName(string text)
        {
#if DEBUG
            Log($"text = {text}");
#endif

            var name = new Name();
            name.OriginalNameValue = text;

            if (!text.Contains("::") && !text.Contains("("))
            {
                name.OriginalNameKey = GetKey(text);
                name.NameKey = name.OriginalNameKey;
                return name;
            }
            throw new NotImplementedException();
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
