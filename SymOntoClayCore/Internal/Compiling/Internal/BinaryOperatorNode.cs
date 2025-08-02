/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using System;

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
            var kindOfOperator = expression.KindOfOperator;

#if DEBUG
            //Info("A626E223-CC31-4EA1-8985-878123435CB4", $"kindOfOperator = {kindOfOperator}");
#endif

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
            //Info("F5226D11-914F-4B58-8249-EFB8F6FA362C", $"expression.Right.Kind = {expression.Right.Kind}");
            //Info("25DEC754-44BF-49DF-B1AA-496AD2B76595", $"expression.Right = {expression.Right}");
#endif

            var rightNode = new ExpressionNode(_context);
            rightNode.Run(expression.Right);
            AddCommands(rightNode.Result);

            RunLeftAssignNode(expression.Left);

        }

        private void RunLeftAssignNode(AstExpression expression)
        {
            var kind = expression.Kind;

#if DEBUG
            //Info("465E1C24-D9C6-4124-8574-2378803CBA55", $"kind = {kind}");
            //Info("CC50F18A-656F-4BFC-9A0B-9F495BD44EB5", $"expression = {expression}");
#endif

            switch (kind)
            {
                case KindOfAstExpression.ConstValue:
                case KindOfAstExpression.Var:
                    {
                        var leftNode = new ExpressionNode(_context);
                        leftNode.Run(expression);
                        AddCommands(leftNode.Result);

                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator =  KindOfOperator.Assign;
                        command.AnnotatedItem = expression;

                        AddCommand(command);
                    }
                    break;

                case KindOfAstExpression.VarDecl:
                    {
                        var varDeclAstExpression = expression.AsVarDeclAstExpression;

                        CompileVarDecl(varDeclAstExpression);

                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Assign;
                        command.AnnotatedItem = expression;

                        AddCommand(command);

                    }
                    break;

                case KindOfAstExpression.BinaryOperator:
                    {
                        var binOpExpr = expression as BinaryOperatorAstExpression;

                        var kindOfOperator = binOpExpr.KindOfOperator;

#if DEBUG
                        //Info("587ADCE8-E906-4EAC-A946-E611B968F250", $"kindOfOperator = {kindOfOperator}");
                        //Info("56BC3CE3-B93E-408D-9C3A-963F1E36364A", $"binOpExpr.Right = {binOpExpr.Right}");
                        //Info("B823CD43-BA0A-46A6-A1EC-2C15864ADE3B", $"binOpExpr.Left = {binOpExpr.Left}");
#endif

                        switch (kindOfOperator)
                        {
                            case KindOfOperator.Assign:
                                {
                                    var rightNode = new ExpressionNode(_context);
                                    rightNode.Run(binOpExpr.Right);
                                    AddCommands(rightNode.Result);

                                    var command = new IntermediateScriptCommand();
                                    command.OperationCode = OperationCode.CallBinOp;
                                    command.KindOfOperator = KindOfOperator.Assign;
                                    command.AnnotatedItem = expression;

                                    AddCommand(command);

                                    RunLeftAssignNode(binOpExpr.Left);
                                }
                                break;

                            default:
                                {
                                    RunUsualBinaryOperator(expression as BinaryOperatorAstExpression);

                                    var command = new IntermediateScriptCommand();
                                    command.OperationCode = OperationCode.CallBinOp;
                                    command.KindOfOperator = KindOfOperator.Assign;
                                    command.AnnotatedItem = expression;

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

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = expression.KindOfOperator;
            command.AnnotatedItem = expression;

            AddCommand(command);
        }
    }
}
