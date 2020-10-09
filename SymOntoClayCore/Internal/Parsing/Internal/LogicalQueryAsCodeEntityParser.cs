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
    public class LogicalQueryAsCodeEntityParser : BaseInternalParser
    {
        public LogicalQueryAsCodeEntityParser(InternalParserContext context)
            : base(context)
        {
        }

        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntity();
            Result.Kind = KindOfCodeEntity.RuleOrFact;

            Result.CodeFile = _context.CodeFile;
            Result.ParentCodeEntity = CurrentCodeEntity;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
#endif

            _context.Recovery(_currToken);
            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

#if DEBUG
            //Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

            Result.Name = ruleInstanceItem.Name;
            Result.RuleInstance = ruleInstanceItem;

            Exit();            
        }
    }
}
