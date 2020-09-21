using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class UnaryOperatorNode : BaseNode
    {
        public UnaryOperatorNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(UnaryOperatorAstExpression expression)
        {
#if DEBUG
            Log($"expression = {expression}");
#endif

            var leftNode = new ExpressionNode(_context);
            leftNode.Run(expression.Left);
            AddCommands(leftNode.Result);

            CompilePushAnnotation(expression);


            var command = new ScriptCommand();
            command.OperationCode = OperationCode.CallUnOp;
            command.KindOfOperator = expression.KindOfOperator;

            AddCommand(command);
        }
    }
}
