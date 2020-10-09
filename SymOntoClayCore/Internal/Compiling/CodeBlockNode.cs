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
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class CodeBlockNode: BaseNode
    {
        public CodeBlockNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<AstStatement> statements)
        {
#if DEBUG
            //Log($"statements = {statements.WriteListToString()}");
#endif

            foreach (var statement in statements)
            {
#if DEBUG
                //Log($"statement = {statement}");
#endif

                var kind = statement.Kind;

                switch(kind)
                {
                    case KindOfAstStatement.Expression:
                        {
                            var node = new ExpressionStatementNode(_context);
                            node.Run(statement as AstExpressionStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.UseInheritance:
                        {
                            var node = new UseInheritanceStatementNode(_context);
                            node.Run(statement as AstUseInheritanceStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    default: 
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }

            var lastCommand = Result.LastOrDefault();

            if (lastCommand == null || lastCommand.OperationCode != OperationCode.Return || lastCommand.OperationCode != OperationCode.ReturnVal)
            {
                var returnCmd = new ScriptCommand();
                returnCmd.OperationCode = OperationCode.Return;

                AddCommand(returnCmd);
            }
        }
    }
}
