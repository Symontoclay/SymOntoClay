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
using System;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class ExpressionNode : BaseNode
    {
        public ExpressionNode(IMainStorageContext context, KindOfCompilePushVal? kindOfCompilePushVal)
            : base(context)
        {
            _kindOfCompilePushVal = kindOfCompilePushVal;
        }

        private readonly KindOfCompilePushVal? _kindOfCompilePushVal;

        public void Run(AstExpression expression)
        {
            var kind = expression.Kind;

            switch(kind)
            {
                case KindOfAstExpression.BinaryOperator:
                    {
                        var node = new BinaryOperatorNode(_context);
                        node.Run(expression as BinaryOperatorAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.UnaryOperator:
                    {
                        var node = new UnaryOperatorNode(_context);
                        node.Run(expression as UnaryOperatorAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.ConstValue:
                    CompilePushVal((expression as ConstValueAstExpression).Value, _kindOfCompilePushVal ?? KindOfCompilePushVal.GetProp);
                    break;

                case KindOfAstExpression.Var:
                    CompilePushVal((expression as VarAstExpression).Name, _kindOfCompilePushVal ?? KindOfCompilePushVal.GetProp);
                    break;

                case KindOfAstExpression.CallingFunction:
                    {
                        var node = new CallingFunctionNode(_context);
                        node.Run(expression as CallingFunctionAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.VarDecl:
                    CompileVarDecl(expression as VarDeclAstExpression);
                    break;

                case KindOfAstExpression.Group:
                    {
                        var node = new ExpressionNode(_context, null);
                        node.Run((expression as GroupAstExpression).Expression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.CodeItem:
                    {
                        var node = new CodeItemNode(_context);
                        node.Run(expression.AsCodeItemAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.New:
                    {
                        var node = new CallingFunctionNode(_context);
                        node.Run(expression as CallingFunctionAstExpression, CallingFunctionNode.KindOfCallingFunction.New);
                        AddCommands(node.Result);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
