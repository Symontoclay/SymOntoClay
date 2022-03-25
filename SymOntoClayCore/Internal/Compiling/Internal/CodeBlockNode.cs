/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class CodeBlockNode: BaseNode
    {
        public CodeBlockNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<AstStatement> statements, LoopCompilingContext loopCompilingContext)
        {
#if DEBUG
            //Log($"statements = {statements.WriteListToString()}");
#endif

            foreach (var statement in statements)
            {
#if DEBUG
                //Log($"statement = {statement}");
#endif

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

                    default: 
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
        }
    }
}
