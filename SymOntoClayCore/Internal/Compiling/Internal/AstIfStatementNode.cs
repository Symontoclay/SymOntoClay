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
            //Log($"statement = {statement}");
#endif

            var hasElifs = !statement.ElifStatements.IsNullOrEmpty();
            var hasElse = !statement.ElseStatements.IsNullOrEmpty();

            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

            IntermediateScriptCommand firstElifCommand = null;
            IntermediateScriptCommand firstElseCommand = null;

            if (hasElifs)
            {
                firstElifCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };
            }

            if (hasElse)
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

            if(hasElifs || hasElse)
            {
                var ifFinalJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo, JumpToMe = afterCommand };

                AddCommand(ifFinalJumpCommand);
            }

#if DEBUG
            //DbgPrintCommands();
#endif

            if (hasElifs)
            {
                var n = 1;
                var elifsTotalCount = statement.ElifStatements.Count;

                foreach(var elifStatement in statement.ElifStatements)
                {
                    AddCommand(firstElifCommand);

                    var elifConditionCodeBlockNode = new ExpressionNode(_context);
                    elifConditionCodeBlockNode.Run(elifStatement.Condition);
                    AddCommands(elifConditionCodeBlockNode.Result);

#if DEBUG
                    //Log($"n = {n}");
                    //Log($"elifsTotalCount = {elifsTotalCount}");
#endif

                    var elifJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpToIfFalse };

                    if (elifsTotalCount == n)
                    {
                        if(hasElse)
                        {
                            elifJumpCommand.JumpToMe = firstElseCommand;
                        }
                        else
                        {
                            elifJumpCommand.JumpToMe = afterCommand;
                        }                        
                    }
                    else
                    {
                        firstElifCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };

                        elifJumpCommand.JumpToMe = firstElifCommand;
                    }

                    AddCommand(elifJumpCommand);

                    var elifCodeBlockNode = new CodeBlockNode(_context);
                    elifCodeBlockNode.Run(elifStatement.Statements);
                    AddCommands(elifCodeBlockNode.Result);

                    if(elifsTotalCount > n || hasElse)
                    {
                        var elifFinalJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo, JumpToMe = afterCommand };

                        AddCommand(elifFinalJumpCommand);
                    }

                    n++;
                }
            }

            if (hasElse)
            {
                AddCommand(firstElseCommand);

                var elseCodeBlockNode = new CodeBlockNode(_context);
                elseCodeBlockNode.Run(statement.ElseStatements);
                AddCommands(elseCodeBlockNode.Result);
            }

            AddCommand(afterCommand);

#if DEBUG
            //DbgPrintCommands();
#endif

            //throw new NotImplementedException();
        }
    }
}
