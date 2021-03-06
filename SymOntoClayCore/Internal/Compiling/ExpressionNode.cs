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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class ExpressionNode : BaseNode
    {
        public ExpressionNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

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
                    CompilePushVal((expression as ConstValueAstExpression).Value);
                    break;

                case KindOfAstExpression.Var:
                    CompilePushValFromVar((expression as VarAstExpression).Name);
                    break;

                case KindOfAstExpression.CallingFunction:
                    {
                        var node = new CallingFunctionNode(_context);
                        node.Run(expression as CallingFunctionAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                case KindOfAstExpression.EntityCondition:
                    {
                        var node = new EntityConditionNode(_context);
                        node.Run(expression as EntityConditionAstExpression);
                        AddCommands(node.Result);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
