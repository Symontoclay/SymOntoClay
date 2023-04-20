using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstEventDeclStatementNode : BaseNode
    {
        public AstEventDeclStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstEventDeclStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement.ToHumanizedString()}");
#endif

            var node = new ExpressionNode(_context);
            node.Run(statement.Expression);
            AddCommands(node.Result);

            CompilePushVal(statement.KindOfLifeCycleEvent);

            CompilePushVal(statement.Handler);

            CompilePushAnnotation(statement);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.AddLifeCycleEvent
            });

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}
