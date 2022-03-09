using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstRepeatStatementNode : BaseNode
    {
        public AstRepeatStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstRepeatStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var firstCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            AddCommand(firstCommand);

            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            var loopCompilingContext = new LoopCompilingContext() { FirstCommand = firstCommand, AfterCommand = afterCommand };

            var codeBlockNode = new CodeBlockNode(_context);
            codeBlockNode.Run(statement.Statements, loopCompilingContext);
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
