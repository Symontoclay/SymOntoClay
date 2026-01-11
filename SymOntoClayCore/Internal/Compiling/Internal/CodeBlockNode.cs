/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Linq;
using static SymOntoClay.Core.Internal.Compiling.Internal.CallingFunctionNode;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class CodeBlockNode: BaseNode
    {
        public CodeBlockNode(IMainStorageContext context)
            : base(context)
        {
            _defaultCtorName = context.CommonNamesStorage.DefaultCtorName;
        }

        private readonly StrongIdentifierValue _defaultCtorName;

        public void Run(List<AstStatement> statements, LoopCompilingContext loopCompilingContext)
        {
            Run(statements, loopCompilingContext, null, KindOfCompilation.Usual);
        }

        public void Run(List<AstStatement> statements, LoopCompilingContext loopCompilingContext, List<AstExpression> callSuperClassConstructorsExpressions, KindOfCompilation kindOfCompilation)
        {
            if (kindOfCompilation == KindOfCompilation.Constructor)
            {
                var callsSelf = false;

                if(!callSuperClassConstructorsExpressions.IsNullOrEmpty())
                {
                    var targetCtorsExprsList = callSuperClassConstructorsExpressions.Select(p => p as CallingFunctionAstExpression);

                    callsSelf = targetCtorsExprsList.Any(p => (p.Left as ConstValueAstExpression).Value.AsStrongIdentifierValue == _defaultCtorName);

                    foreach (var callCtorExpr in targetCtorsExprsList)
                    {
                        var node = new CallingFunctionNode(_context);
                        node.Run(callCtorExpr, KindOfCallingFunction.CallConstructor);
                        AddCommands(node.Result);
                    }
                }

                if(!callsSelf)
                {
                    AddCommand(new IntermediateScriptCommand() { OperationCode = OperationCode.CallDefaultCtors });
                }                

                AddCommand(new IntermediateScriptCommand()
                {
                    OperationCode = OperationCode.ClearStack
                });

            }

            foreach (var statement in statements)
            {
                var kind = statement.Kind;

                switch(kind)
                {
                    case KindOfAstStatement.Expression:
                        {
                            var node = new ExpressionStatementNode(_context);
                            node.Run(statement as AstExpressionStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.SetInheritance:
                        {
                            var node = new SetInheritanceStatementNode(_context);
                            node.Run(statement as AstSetInheritanceStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.ErrorStatement:
                        {
                            var node = new AstErrorStatementNode(_context);
                            node.Run(statement as AstErrorStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.TryStatement:
                        {
                            var node = new AstTryStatementNode(_context);
                            node.Run(statement as AstTryStatement, loopCompilingContext);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.AwaitStatement:
                        {
                            var node = new AstAwaitStatementNode(_context);
                            node.Run(statement as AstAwaitStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.WaitStatement:
                        {
                            var node = new AstWaitStatementNode(_context);
                            node.Run(statement as AstWaitStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.CompleteStatement:
                        {
                            var node = new AstCompleteStatementNode(_context);
                            node.Run(statement as AstCompleteStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.BreakStatement:
                        {
                            var node = new AstBreakStatementNode(_context);
                            node.Run(statement as AstBreakStatement, loopCompilingContext);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.ContinueStatement:
                        {
                            var node = new AstContinueStatementNode(_context);
                            node.Run(statement as AstContinueStatement, loopCompilingContext);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.ReturnStatement:
                        {
                            var node = new AstReturnStatementNode(_context);
                            node.Run(statement as AstReturnStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.IfStatement:
                        {
                            var node = new AstIfStatementNode(_context);
                            node.Run(statement as AstIfStatement, loopCompilingContext);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.WhileStatement:
                        {
                            var node = new AstWhileStatementNode(_context);
                            node.Run(statement as AstWhileStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.RepeatStatement:
                        {
                            var node = new AstRepeatStatementNode(_context);
                            node.Run(statement as AstRepeatStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.SetDefaultState:
                        {
                            var node = new AstSetDefaultStateStatementNode(_context);
                            node.Run(statement as AstSetDefaultStateStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.SetState:
                        {
                            var node = new AstSetStateStatementNode(_context);
                            node.Run(statement as AstSetStateStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.RejectStatement:
                        {
                            var node = new AstRejectStatementNode(_context);
                            node.Run(statement as AstRejectStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.ExecStatement:
                        {
                            var node = new AstExecStatementNode(_context);
                            node.Run(statement as AstExecStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.EventDeclStatement:
                        {
                            var node = new AstEventDeclStatementNode(_context);
                            node.Run(statement as AstEventDeclStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.WeakCancelActionStatement:
                        {
                            var node = new AstWeakCancelActionStatementNode(_context);
                            node.Run(statement as AstWeakCancelActionStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    case KindOfAstStatement.CancelActionStatement:
                        {
                            var node = new AstCancelActionStatementNode(_context);
                            node.Run(statement as AstCancelActionStatement);
                            AddCommands(node.Result);
                        }
                        break;

                    default: 
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
        }
    }
}
