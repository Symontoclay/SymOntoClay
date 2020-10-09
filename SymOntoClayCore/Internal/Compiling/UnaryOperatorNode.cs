/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class UnaryOperatorNode : BaseNode
    {
        public UnaryOperatorNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(UnaryOperatorAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            var leftNode = new ExpressionNode(_context);
            leftNode.Run(expression.Left);
            AddCommands(leftNode.Result);

            CompilePushAnnotation(expression);


            var command = new ScriptCommand();
            command.OperationCode = OperationCode.CallUnOp;
            command.KindOfOperator = expression.KindOfOperator;

            AddCommand(command);
        }
    }
}
