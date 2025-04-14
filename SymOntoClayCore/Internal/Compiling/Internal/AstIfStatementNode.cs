/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstIfStatementNode : BaseNode
    {
        public AstIfStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstIfStatement statement, LoopCompilingContext loopCompilingContext)
        {
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

            var ifConditionCodeBlockNode = new ExpressionNode(_context, KindOfCompilePushVal.GetAllCases);
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
            ifCodeBlockNode.Run(statement.IfStatements, loopCompilingContext);
            AddCommands(ifCodeBlockNode.Result);

            if(hasElifs || hasElse)
            {
                var ifFinalJumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo, JumpToMe = afterCommand };

                AddCommand(ifFinalJumpCommand);
            }

            if (hasElifs)
            {
                var n = 1;
                var elifsTotalCount = statement.ElifStatements.Count;

                foreach(var elifStatement in statement.ElifStatements)
                {
                    AddCommand(firstElifCommand);

                    var elifConditionCodeBlockNode = new ExpressionNode(_context, KindOfCompilePushVal.GetAllCases);
                    elifConditionCodeBlockNode.Run(elifStatement.Condition);
                    AddCommands(elifConditionCodeBlockNode.Result);

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
                    elifCodeBlockNode.Run(elifStatement.Statements, loopCompilingContext);
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
                elseCodeBlockNode.Run(statement.ElseStatements, loopCompilingContext);
                AddCommands(elseCodeBlockNode.Result);
            }

            AddCommand(afterCommand);

        }
    }
}
