using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class ExpressionStatementNode : BaseNode
    {
        public ExpressionStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstExpressionStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var node = new ExpressionNode(_context);
            node.Run(statement.Expression);

            AddCommands(node.Result);

            AddCommand(new ScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}
