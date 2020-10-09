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
