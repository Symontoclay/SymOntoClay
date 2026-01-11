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

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class LambdaNode : BaseNode
    {
        public LambdaNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstStatement statement)
        {
#if DEBUG
            //Info("D86974B2-295C-4F9C-AD8B-29B48AF85CB7", $"statement.GetType().Name = {statement.GetType().Name}");
#endif

            var astExpressionStatement = statement as AstExpressionStatement;

            Run(astExpressionStatement.Expression);

#if DEBUG
            //DbgPrintCommands();
#endif
        }

        public void Run(AstExpression expression)
        {
            var node = new ExpressionNode(_context);
            node.Run(expression);

            AddCommands(node.Result);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.ReturnVal
            });
        }
    }
}
