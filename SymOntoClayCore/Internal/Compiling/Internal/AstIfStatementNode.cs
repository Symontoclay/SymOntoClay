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

            IntermediateScriptCommand firstElifCommand = null;
            IntermediateScriptCommand firstElseCommand = null;

            if (!statement.ElifStatements.IsNullOrEmpty())
            {
                firstElifCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
            }

            if (!statement.ElseStatements.IsNullOrEmpty())
            {
                firstElseCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
            }

            var ifConditionCodeBlockNode = new ExpressionNode(_context);
            ifConditionCodeBlockNode.Run(statement.Condition);

            AddCommands(ifConditionCodeBlockNode.Result);

            var ifJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpToIfFalse };

            if(firstElifCommand != null)
            {
                ifJumpCommand.JumpToMe = firstElifCommand;
            }
            else
            {
                if(firstElseCommand != null)
                {
                    ifJumpCommand.JumpToMe = firstElseCommand;
                }
                else
                {
                    ifJumpCommand.JumpToMe = afterCommand;
                }
            }

            AddCommand(ifJumpCommand);

            var ifCodeBlockNode = new CodeBlockNode(_context);
            ifCodeBlockNode.Run(statement.IfStatements);
            AddCommands(ifCodeBlockNode.Result);

            if (!statement.ElifStatements.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

            if(!statement.ElseStatements.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

            AddCommand(afterCommand);

#if DEBUG
            //DbgPrintCommands();
#endif

            //throw new NotImplementedException();
        }
    }
}
