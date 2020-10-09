/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class BinaryOperatorNode : BaseNode
    {
        public BinaryOperatorNode(IMainStorageContext context)
            : base(context)
        {
        }
        
        public void Run(BinaryOperatorAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            var kindOfOperator = expression.KindOfOperator;

            switch(kindOfOperator)
            {
                case KindOfOperator.Assign:
                    RunAssignBinaryOperator(expression);
                    break;

                default:
                    RunUsualBinaryOperator(expression);
                    break;
            }
        }

        private void RunAssignBinaryOperator(BinaryOperatorAstExpression expression)
        {
#if DEBUG
            Log($"expression = {expression}");
#endif

            var rightBranch = expression.Right;

#if DEBUG
            Log($"rightBranch = {rightBranch}");
#endif

            if(rightBranch.Kind == KindOfAstExpression.ConstValue)
            {
                var rightNode = new ExpressionNode(_context);
                rightNode.Run(rightBranch);
                AddCommands(rightNode.Result);
            }
            else
            {
                var command = new ScriptCommand();
                command.OperationCode = OperationCode.PushValToVar;
                command.Value = (rightBranch as VarAstExpression).Name.GetIndexedValue(_context);

                AddCommand(command);
            }

            var leftBranch = expression.Left;

#if DEBUG
            Log($"leftBranch = {leftBranch}");
#endif

            if (leftBranch.Kind == KindOfAstExpression.Var)
            {
                var command = new ScriptCommand();
                command.OperationCode = OperationCode.PushValToVar;
                command.Value = (leftBranch as VarAstExpression).Name.GetIndexedValue(_context);

                AddCommand(command);
            }
            else
            {
                var rightNode = new ExpressionNode(_context);
                rightNode.Run(leftBranch);
                AddCommands(rightNode.Result);
            }
        }

        private void RunUsualBinaryOperator(BinaryOperatorAstExpression expression)
        {
            var leftNode = new ExpressionNode(_context);
            leftNode.Run(expression.Left);
            AddCommands(leftNode.Result);

            var rightNode = new ExpressionNode(_context);
            rightNode.Run(expression.Right);
            AddCommands(rightNode.Result);

            CompilePushAnnotation(expression);

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = expression.KindOfOperator;

            AddCommand(command);
        }
    }
}
