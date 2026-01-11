/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
#if DEBUG
            //Info("C8E3E012-9FCA-4D38-9BF0-C6F0BBD52B5E", $"expression = {expression.ToHumanizedString()}");
#endif

            var kindOfOperator = expression.KindOfOperator;

#if DEBUG
            //Info("A626E223-CC31-4EA1-8985-878123435CB4", $"kindOfOperator = {kindOfOperator}");
#endif

            switch(kindOfOperator)
            {
                case KindOfOperator.Assign:
                    RunAssignBinaryOperator(expression);
                    break;

                case KindOfOperator.AddAssign:
                case KindOfOperator.SubAssign:
                case KindOfOperator.MulAssign:
                case KindOfOperator.DivAssign:
                    RunOpAssignBinaryOperator(expression);
                    break;

                default:
                    RunUsualBinaryOperator(expression);
                    break;
            }
        }

        private void RunAssignBinaryOperator(BinaryOperatorAstExpression expression)
        {
#if DEBUG
            //Info("2149E99F-774D-4147-AA61-025554A1A323", $"expression = {expression.ToHumanizedString()}");
            //Info("B6B181EE-2A94-4EEF-804E-21F8B348130E", $"expression.Left = {expression.Left.ToHumanizedString()}");
            //Info("FDA0A7CE-8F75-4AC3-9365-FFFC417C2D2C", $"expression.Right = {expression.Right.ToHumanizedString()}");
            //Info("F5226D11-914F-4B58-8249-EFB8F6FA362C", $"expression.Right.Kind = {expression.Right.Kind}");
            //Info("25DEC754-44BF-49DF-B1AA-496AD2B76595", $"expression.Right = {expression.Right}");
#endif

            var rightNode = new ExpressionNode(_context);
            rightNode.Run(expression.Right);
            AddCommands(rightNode.Result);

            RunLeftAssignNode(expression.Left);

#if DEBUG
            //DbgPrintCommands("B8A256A6-55F4-40F8-86FA-491EB97B0392");
#endif
        }

        private void RunOpAssignBinaryOperator(BinaryOperatorAstExpression expression)
        {
            var kindOfOperator = expression.KindOfOperator;

#if DEBUG
            //Info("9807FD7B-6C9A-492B-8F79-2D079CB747C0", $"expression = {expression.ToHumanizedString()}");
            //Info("7A3F4704-DE34-4536-BD21-40740F6755C1", $"kindOfOperator = {kindOfOperator}");
            //Info("5D6CD10C-9C53-41F6-A9D0-D0EFD38C8368", $"expression.Left = {expression.Left.ToHumanizedString()}");
            //Info("8487A0CD-3FA5-4277-A7C0-591F507E348B", $"expression.Right = {expression.Right.ToHumanizedString()}");
#endif

            {
                var leftNode = new ExpressionNode(_context);
                leftNode.Run(expression.Left);
                AddCommands(leftNode.Result);

                var rightNode = new ExpressionNode(_context);
                rightNode.Run(expression.Right);
                AddCommands(rightNode.Result);
            }

            switch (kindOfOperator)
            {
                case KindOfOperator.AddAssign:
                    {
                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Add;
                        command.AnnotatedItem = expression;

                        AddCommand(command);
                    }
                    break;

                case KindOfOperator.SubAssign:
                    {
                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Sub;
                        command.AnnotatedItem = expression;

                        AddCommand(command);
                    }
                    break;

                case KindOfOperator.MulAssign:
                    {
                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Mul;
                        command.AnnotatedItem = expression;

                        AddCommand(command);
                    }
                    break;

                case KindOfOperator.DivAssign:
                    {
                        var command = new IntermediateScriptCommand();
                        command.OperationCode = OperationCode.CallBinOp;
                        command.KindOfOperator = KindOfOperator.Div;
                        command.AnnotatedItem = expression;

                        AddCommand(command);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }

            {
                var leftNode = new ExpressionNode(_context);
                leftNode.Run(expression.Left);
                AddCommands(leftNode.Result);

                var command = new IntermediateScriptCommand();
                command.OperationCode = OperationCode.CallBinOp;
                command.KindOfOperator = KindOfOperator.Assign;
                command.AnnotatedItem = expression;

                AddCommand(command);
            }

#if DEBUG
            //DbgPrintCommands("531F5AF9-3D8B-40CA-A125-F0B1FAF049A7");
#endif

            //throw new NotImplementedException("39B121B7-7860-436C-9C45-CDAA5CBB77C8");
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
            //DbgPrintCommands("7AA016B5-181C-49D8-AFC8-F8D39AD2D9D2");
#endif
        }

        private void RunUsualBinaryOperator(BinaryOperatorAstExpression expression)
        {
#if DEBUG
            //Info("5621E83E-0373-4300-B17E-A9E478AF19D4", $"expression = {expression.ToHumanizedString()}");
            //Info("5BE4BA3B-3D41-4EA5-9C45-E1CF9FF074D7", $"expression.Left = {expression.Left.ToHumanizedString()}");
            //Info("6CA7B65C-FD16-4136-8741-3163577C89E4", $"expression.Right = {expression.Right.ToHumanizedString()}");
#endif

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

#if DEBUG
            //DbgPrintCommands("46A58625-8684-4714-AC06-F5F6D344903C");
#endif
        }
    }
}
