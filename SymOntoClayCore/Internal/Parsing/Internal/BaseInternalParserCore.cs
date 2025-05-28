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

using SymOntoClay.Common.CollectionsHelpers;
using System.Linq;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseInternalParserCore : BaseLoggedComponent
    {
        protected BaseInternalParserCore(InternalParserCoreContext context)
            : this(context, null)
        {
        }

        protected BaseInternalParserCore(InternalParserCoreContext context, TerminationToken[] terminationTokens)
            : base(context.Logger)
        {
            _context = context;
            _terminationTokens = terminationTokens;
            _hasTerminationTokens = !terminationTokens.IsNullOrEmpty();
        }

        private readonly InternalParserCoreContext _context;

        protected InternalParserCoreContext CoreContext => _context;

        protected readonly TerminationToken[] _terminationTokens;
        private readonly bool _hasTerminationTokens;

        protected virtual bool ShouldBeUsedTerminationToken() => true;

        protected string Text => _context.Text;

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
