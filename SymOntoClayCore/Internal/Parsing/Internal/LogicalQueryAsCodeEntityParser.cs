/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryAsCodeEntityParser : BaseInternalParser
    {
        public LogicalQueryAsCodeEntityParser(InternalParserContext context)
            : base(context)
        {
        }

        public RuleInstance Result => _ruleInstance;
        private RuleInstance _ruleInstance;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
#endif

            _context.Recovery(_currToken);
            var parser = new LogicalQueryParser(_context);
            parser.Run();

            _ruleInstance = parser.Result;

#if DEBUG
            Log($"DebugHelperForRuleInstance.ToString(_ruleInstance) = {DebugHelperForRuleInstance.ToString(_ruleInstance)}");
            Log($"_context.CurrentDefaultSetings?.TypeOfAccess = {_context.CurrentDefaultSetings?.TypeOfAccess}");
            Log($"_context.CurrentDefaultSetings?.Holder = {_context.CurrentDefaultSetings?.Holder}");
#endif

            if (_context.CurrentDefaultSetings != null)
            {
                _ruleInstance.TypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
                _ruleInstance.Holder = _context.CurrentDefaultSetings.Holder;
            }

#if DEBUG
            Log($"DebugHelperForRuleInstance.ToString(_ruleInstance) = {DebugHelperForRuleInstance.ToString(_ruleInstance)}");
            Log($"_ruleInstance = {_ruleInstance}");
#endif

            Exit();            
        }
    }
}
