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
    public class EntityConditionNode : BaseNode
    {
        public EntityConditionNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(EntityConditionAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            var count = 0;

            if(expression.FirstCoordinate != null)
            {
                count++;

                var node = new ExpressionNode(_context);
                node.Run(expression.FirstCoordinate);
                AddCommands(node.Result);

                if(expression.SecondCoordinate != null)
                {
                    count++;

                    node = new ExpressionNode(_context);
                    node.Run(expression.SecondCoordinate);
                    AddCommands(node.Result);
                }
            }

            var isNamed = false;

            if(expression.Name != null && !expression.Name.IsEmpty && expression.Name.NameValue != "#@")
            {
                isNamed = true;
                CompilePushVal(expression.Name);
            }

            count++;

            CompilePushAnnotation(expression);

            var kindOfEntityConditionAstExpression = expression.KindOfEntityConditionAstExpression;

#if DEBUG
            //Log($"count = {count}");
            //Log($"isNamed = {isNamed}");
            //Log($"kindOfEntityConditionAstExpression = {kindOfEntityConditionAstExpression}");
#endif

            switch(kindOfEntityConditionAstExpression)
            {
                case KindOfEntityConditionAstExpression.Waypoint:
                    {
                        var command = new ScriptCommand();

                        if (isNamed)
                        {                        
                            command.OperationCode = OperationCode.AllocateNamedWaypoint;
                        }
                        else
                        {
                            command.OperationCode = OperationCode.AllocateAnonymousWaypoint;
                        }

                        command.CountParams = count;

                        AddCommand(command);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntityConditionAstExpression), kindOfEntityConditionAstExpression, null);
            }     
        }
    }
}
