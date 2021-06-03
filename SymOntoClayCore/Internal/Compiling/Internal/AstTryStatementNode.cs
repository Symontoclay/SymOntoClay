using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstTryStatementNode : BaseNode
    {
        public AstTryStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstTryStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            IntermediateScriptCommand firstElseCommand = null;
            IntermediateScriptCommand firstEnsureComand = null;
            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            if (!statement.ElseStatements.IsNullOrEmpty())
            {
                firstElseCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
            }

            if(!statement.EnsureStatements.IsNullOrEmpty())
            {
                firstEnsureComand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
            }

            var sehGroup = new IntermediateSEHGroup();

            var tryCommand = new IntermediateScriptCommand();
            tryCommand.OperationCode = OperationCode.SetSEHGroup;
            tryCommand.SEHGroup = sehGroup;
            AddCommand(tryCommand);

            var tryCodeBlockNode = new CodeBlockNode(_context);
            tryCodeBlockNode.Run(statement.TryStatements);
            AddCommands(tryCodeBlockNode.Result);

            var removeSEHCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.RemoveSEHGroup };

            AddCommand(removeSEHCommand);

            var tryAfterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo };

            if (firstElseCommand != null)
            {
                tryAfterCommand.JumpToMe = firstElseCommand;
            }
            else
            {
                if(firstEnsureComand == null)
                {
                    tryAfterCommand.JumpToMe = afterCommand;
                }
                else
                {
                    tryAfterCommand.JumpToMe = firstEnsureComand;
                }
            }

            AddCommand(tryAfterCommand);

            if(!statement.CatchStatements.IsNullOrEmpty())
            {
                foreach(var catchStatement in statement.CatchStatements)
                {
                    var firstCatchCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
                    AddCommand(firstCatchCommand);

                    var sehItem = new IntermediateSEHItem();
                    sehGroup.Items.Add(sehItem);
                    sehItem.VariableName = catchStatement.VariableName;
                    sehItem.Condition = catchStatement.Condition;
                    sehItem.JumpToMe = firstCatchCommand;

                    var catchCodeBlockNode = new CodeBlockNode(_context);
                    catchCodeBlockNode.Run(catchStatement.Statements);
                    AddCommands(catchCodeBlockNode.Result);

                    var catchAfterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo };

                    if (firstEnsureComand == null)
                    {
                        catchAfterCommand.JumpToMe = afterCommand;
                    }
                    else
                    {
                        catchAfterCommand.JumpToMe = firstEnsureComand;
                    }

                    AddCommand(catchAfterCommand);
                }
            }

            if (!statement.ElseStatements.IsNullOrEmpty())
            {
                AddCommand(firstElseCommand);

                var elseCodeBlockNode = new CodeBlockNode(_context);
                elseCodeBlockNode.Run(statement.ElseStatements);
                AddCommands(elseCodeBlockNode.Result);

                var elseAfterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo };

                if (firstEnsureComand == null)
                {
                    elseAfterCommand.JumpToMe = afterCommand;
                }
                else
                {
                    elseAfterCommand.JumpToMe = firstEnsureComand;
                }

                AddCommand(elseAfterCommand);
            }

            if(!statement.EnsureStatements.IsNullOrEmpty())
            {
                AddCommand(firstEnsureComand);

                var ensureCodeBlockNode = new CodeBlockNode(_context);
                ensureCodeBlockNode.Run(statement.EnsureStatements);
                AddCommands(ensureCodeBlockNode.Result);

                var ensureAfterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo };
                ensureAfterCommand.JumpToMe = afterCommand;
                AddCommand(ensureAfterCommand);
            }

            AddCommand(afterCommand);
        }
    }
}
