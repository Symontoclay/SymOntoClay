/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
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
            //Log($"expression = {expression.ToHumanizedString()}");
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
            //Log($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            //Log($"expression = {expression.ToHumanizedString()}");
            //Log($"expression = {expression}");
#endif

            var rightNode = new ExpressionNode(_context);
            rightNode.Run(expression.Right);
            AddCommands(rightNode.Result);

            RunLeftAssingNode(expression.Left);

#if DEBUG
            //DbgPrintCommands();
            //throw new NotImplementedException();
#endif
        }

        private void RunLeftAssingNode(AstExpression expression)
        {
#if DEBUG
            //Log($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            //Log($"expression = {expression.ToHumanizedString()}");
            //Log($"expression = {expression}");
#endif

            var kind = expression.Kind;

            switch (kind)
            {
                case KindOfAstExpression.Var:
                    {
                        var leftNode = new ExpressionNode(_context);
                        leftNode.Run(expression);
                        AddCommands(leftNode.Result);

                        CompilePushAnnotation(expression);

                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator =  KindOfOperator.Assign;

                        AddCommand(command);
                    }
                    break;

                case KindOfAstExpression.VarDecl:
                    {
                        var varDeclAstExpression = expression.AsVarDeclAstExpression;

#if DEBUG
                        //Log($"varDeclAstExpression = {varDeclAstExpression.ToHumanizedString()}");
#endif

                        CompileVarDecl(varDeclAstExpression);

                        CompilePushAnnotation(expression);

                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Assign;

                        AddCommand(command);

#if DEBUG
                        //DbgPrintCommands();
#endif
                    }
                    break;

                case KindOfAstExpression.BinaryOperator:
                    {
                        var binOpExpr = expression as BinaryOperatorAstExpression;

                        var kindOfOperator = binOpExpr.KindOfOperator;

                        switch(kindOfOperator)
                        {
                            case KindOfOperator.Assign:
                                {
                                    var rightNode = new ExpressionNode(_context);
                                    rightNode.Run(binOpExpr.Right);
                                    AddCommands(rightNode.Result);

                                    CompilePushAnnotation(expression);

                                    var command = new IntermediateScriptCommand();
                                    command.OperationCode = OperationCode.CallBinOp;
                                    command.KindOfOperator = KindOfOperator.Assign;

                                    AddCommand(command);

#if DEBUG
                                    //DbgPrintCommands();
#endif

                                    RunLeftAssingNode(binOpExpr.Left);
                                }
                                break;

                            default:
                                {
                                    RunUsualBinaryOperator(expression as BinaryOperatorAstExpression);

                                    CompilePushAnnotation(expression);

                                    var command = new IntermediateScriptCommand();
                                    command.OperationCode = OperationCode.CallBinOp;
                                    command.KindOfOperator = KindOfOperator.Assign;

                                    AddCommand(command);
                                }                                
                                break;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

#if DEBUG
            //DbgPrintCommands();
            //throw new NotImplementedException();
#endif
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

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = expression.KindOfOperator;

            AddCommand(command);
        }
    }
}
