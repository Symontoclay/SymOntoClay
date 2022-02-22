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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public abstract class BaseNode: BaseLoggedComponent
    {
        protected BaseNode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        public List<IntermediateScriptCommand> Result => _result;

        private readonly List<IntermediateScriptCommand> _result = new List<IntermediateScriptCommand>();

        protected void AddCommand(IntermediateScriptCommand command)
        {
            _result.Add(command);
        }

        protected void AddCommands(List<IntermediateScriptCommand> commands)
        {
            _result.AddRange(commands);
        }

        protected void CompileValue(Value value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            if(value.IsStrongIdentifierValue)
            {
                var name = value.AsStrongIdentifierValue;

                var kindOfName = name.KindOfName;

                switch(kindOfName)
                {
                    case KindOfName.Concept:
                    case KindOfName.Channel:
                    case KindOfName.Entity:
                        CompilePushVal(value);
                        break;

                    case KindOfName.SystemVar:
                    case KindOfName.Var:
                        CompilePushValFromVar(value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                }

                return;
            }

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value;

            AddCommand(command);
        }

        protected void CompileVarDecl(VarDeclAstExpression varDeclAstExpression)
        {
            CompilePushVal(varDeclAstExpression.Name);

            foreach (var typeItem in varDeclAstExpression.TypesList)
            {
                CompilePushVal(typeItem);
            }

            CompilePushAnnotation(varDeclAstExpression);

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.VarDecl;
            command.CountParams = varDeclAstExpression.TypesList.Count;

            AddCommand(command);
        }

        protected void CompilePushVal(Value value)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value;

            AddCommand(command);
        }

        protected void CompilePushValFromVar(Value value)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushValFromVar;
            command.Value = value;

            AddCommand(command);
        }

        protected void CompilePushAnnotation(AnnotatedItem annotatedItem)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = annotatedItem.GetAnnotationValue();

            AddCommand(command);
        }
    }
}
