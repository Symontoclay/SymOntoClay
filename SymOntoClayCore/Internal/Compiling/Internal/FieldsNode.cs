/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class FieldsNode : BaseNode
    {
        public FieldsNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<Field> fields)
        {
            foreach(var field in fields)
            {
                CompileVarDecl(field);

                if(field.Value == null)
                {
                    continue;
                }

                var node = new ExpressionNode(_context);
                node.Run(field.Value);

                AddCommands(node.Result);

                CompilePushVal(field.Name);

                CompilePushAnnotation(field);

                var command = new IntermediateScriptCommand();
                command.OperationCode = OperationCode.CallBinOp;
                command.KindOfOperator = KindOfOperator.Assign;

                AddCommand(command);

            }
        }
    }
}
