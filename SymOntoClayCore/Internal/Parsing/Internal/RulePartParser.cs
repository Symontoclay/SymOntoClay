/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
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
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
#endif

            if(_currToken.TokenKind != TokenKind.OpenFigureBracket)
            {
                _context.Recovery(_currToken);
            }

            var paser = new LogicalExpressionParser(_context, _terminatingTokenKind);
            paser.Run();

#if DEBUG
            //Log($"paser.Result = {paser.Result}");
#endif

            _baseRulePart.Expression = paser.Result;

            var nextToken = _context.GetToken();

#if DEBUG
            //Log($"nextToken = {nextToken}");
            //Log($"_terminatingTokenKind = {_terminatingTokenKind}");
#endif

            if ((nextToken.TokenKind != _terminatingTokenKind) || nextToken.TokenKind == TokenKind.CloseFactBracket)
            {
                _context.Recovery(nextToken);
            }

            Exit();
        }
    }
}
