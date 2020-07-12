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

                case KindOfAstExpression.ConstValue:
                    {
                        var command = new ScriptCommand();
                        command.OperationCode = OperationCode.PushVal;
                        command.Value = (expression as ConstValueAstExpression).Value;

                        AddCommand(command);
                    }
                    break;

                default: 
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
