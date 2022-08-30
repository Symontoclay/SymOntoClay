using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstExecStatementNode : BaseNode
    {
        public AstExecStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstExecStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var node = new ExpressionNode(_context);
            node.Run(statement.Expression);

            AddCommands(node.Result);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.Exec
            });
        }
    }
}
