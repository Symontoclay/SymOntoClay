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

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class RulePartParser : BaseInternalParser
    {
        protected RulePartParser(InternalParserContext context, TokenKind terminatingTokenKind)
            : base(context)
        {
            _terminatingTokenKind = terminatingTokenKind;
        }

        private TokenKind _terminatingTokenKind;
        protected BaseRulePart _baseRulePart;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            if(_currToken.TokenKind != TokenKind.OpenFigureBracket)
            {
                _context.Recovery(_currToken);
            }

            var logicalExpressionParserContext = new LogicalExpressionParserContext(_context);

            var parser = new LogicalExpressionParser(logicalExpressionParserContext, _terminatingTokenKind);
            parser.Run();

            if(logicalExpressionParserContext.AliasesDict.Any())
            {
                _baseRulePart.AliasesDict = logicalExpressionParserContext.AliasesDict;
            }
            else
            {
                _baseRulePart.AliasesDict = new Dictionary<StrongIdentifierValue, LogicalQueryNode>();
            }

            _baseRulePart.Expression = parser.Result;

            var nextToken = _context.GetToken();

            if ((nextToken.TokenKind != _terminatingTokenKind) || nextToken.TokenKind == TokenKind.CloseFactBracket)
            {
                _context.Recovery(nextToken);
            }

            Exit();
        }
    }
}
