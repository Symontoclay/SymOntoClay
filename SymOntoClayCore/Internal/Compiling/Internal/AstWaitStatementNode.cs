using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstWaitStatementNode : BaseNode
    {
        public AstWaitStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstWaitStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
            //Log($"statement = {statement.ToHumanizedString()}");
#endif

            foreach(var item in statement.Items)
            {
                var node = new ExpressionNode(_context);
                node.Run(item);
                AddCommands(node.Result);
            }

            CompilePushAnnotation(statement);

            var command = new IntermediateScriptCommand();

            command.CountParams = statement.Items.Count;
            command.OperationCode = OperationCode.Wait;

            AddCommand(command);

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}
