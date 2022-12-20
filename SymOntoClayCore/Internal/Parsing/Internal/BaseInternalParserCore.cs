using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseInternalParserCore : BaseLoggedComponent
    {
        protected BaseInternalParserCore(InternalParserContext context)
            : this(context, null)
        {
        }

        protected BaseInternalParserCore(InternalParserContext context, TerminationToken[] terminationTokens)
            : base(context.Logger)
        {
            _context = context;
            _terminationTokens = terminationTokens;
            _hasTerminationTokens = !terminationTokens.IsNullOrEmpty();
        }

        protected readonly InternalParserContext _context;
        protected readonly TerminationToken[] _terminationTokens;
        private readonly bool _hasTerminationTokens;

        protected virtual bool ShouldBeUsedTerminationToken() => true;

        public void Run()
        {
            OnEnter();

            while ((_currToken = _context.GetToken()) != null)
            {
                if (_hasTerminationTokens && ShouldBeUsedTerminationToken())
                {
                    var terminationToken = _terminationTokens.FirstOrDefault(p => p.Equals(_currToken));

                    if (terminationToken != null)
                    {
                        if (terminationToken.NeedRecovery)
                        {
                            _context.Recovery(_currToken);
                        }

                        OnFinish();
                        return;
                    }
                }

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
    }
}
