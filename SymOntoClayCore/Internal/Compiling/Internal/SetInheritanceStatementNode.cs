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

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class SetInheritanceStatementNode : BaseNode
    {
        public SetInheritanceStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstSetInheritanceStatement statement)
        {
            CompileValue(statement.SubName);
            CompileValue(statement.SuperName);
            CompileValue(statement.Rank);

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.GetAnnotationValue();

            AddCommand(command);

            command = new IntermediateScriptCommand();

            if (statement.HasNot)
            {
                command.OperationCode = OperationCode.SetNotInheritance;
            }
            else
            {
                command.OperationCode = OperationCode.SetInheritance;
            }
            
            AddCommand(command);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}
