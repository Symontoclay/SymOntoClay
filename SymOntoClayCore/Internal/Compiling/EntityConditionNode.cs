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
            Log($"expression = {expression}");
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
            Log($"count = {count}");
            Log($"isNamed = {isNamed}");
            Log($"kindOfEntityConditionAstExpression = {kindOfEntityConditionAstExpression}");
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
