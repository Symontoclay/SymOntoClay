/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class ExpressionStatementNode : BaseNode
    {
        public ExpressionStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstExpressionStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var node = new ExpressionNode(_context);
            node.Run(statement.Expression);

            AddCommands(node.Result);

            AddCommand(new ScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}
