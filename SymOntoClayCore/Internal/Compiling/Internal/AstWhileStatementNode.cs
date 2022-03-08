using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstWhileStatementNode : BaseNode
    {
        public AstWhileStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstWhileStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var firstCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            AddCommand(firstCommand);

            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            var conditionCodeBlockNode = new ExpressionNode(_context);
            conditionCodeBlockNode.Run(statement.Condition);

            AddCommands(conditionCodeBlockNode.Result);

            var jumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpToIfFalse, JumpToMe = afterCommand };

            AddCommand(jumpCommand);

            var codeBlockNode = new CodeBlockNode(_context);
            codeBlockNode.Run(statement.Statements);
            AddCommands(codeBlockNode.Result);

            var finalJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo, JumpToMe = firstCommand };

            AddCommand(finalJumpCommand);

            AddCommand(afterCommand);

#if DEBUG
            //DbgPrintCommands();
#endif

            //throw new NotImplementedException();
        }
    }
}
