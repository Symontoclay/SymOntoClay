using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstIfStatementNode : BaseNode
    {
        public AstIfStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstIfStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            var ifCodeBlockNode = new ExpressionNode(_context);
            ifCodeBlockNode.Run(statement.ConditionalExpression);

            AddCommands(ifCodeBlockNode.Result);

            if(!statement.ElifStatements.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}
