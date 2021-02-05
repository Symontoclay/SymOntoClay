/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
