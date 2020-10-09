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
    public class UseInheritanceStatementNode : BaseNode
    {
        public UseInheritanceStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstUseInheritanceStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            CompileValue(statement.SubName);
            CompileValue(statement.SuperName);
            CompileValue(statement.Rank);

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.GetAnnotationValue().GetIndexedValue(_context);

            AddCommand(command);

            command = new ScriptCommand();

            if (statement.HasNot)
            {
                command.OperationCode = OperationCode.UseNotInheritance;
            }
            else
            {
                command.OperationCode = OperationCode.UseInheritance;
            }
            
            AddCommand(command);

            AddCommand(new ScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}
