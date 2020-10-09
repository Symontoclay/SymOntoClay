/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class Compiler: BaseComponent, ICompiler
    {
        public Compiler(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<AstStatement> statements)
        {
            var node = new CodeBlockNode(_context);
            node.Run(statements);

            var resultCommandsList = node.Result;

#if DEBUG
            //Log($"resultCommandsList = {resultCommandsList.WriteListToString()}");
#endif

            var n = 0;
            var result = new CompiledFunctionBody();

            foreach (var command in resultCommandsList)
            {
                command.Position = n;             
                result.Commands.Add(n, command);
                n++;
            }

            return result;
        }
    }
}
