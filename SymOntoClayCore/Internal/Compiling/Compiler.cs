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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Compiling.Internal;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class Compiler: BaseComponent, ICompiler
    {
        public Compiler(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<AstStatement> statements)
        {
            return Compile(statements, null, KindOfCompilation.Usual);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<AstStatement> statements, List<AstExpression> callSuperClassContructorsExpressions, KindOfCompilation kindOfCompilation)
        {
            var node = new CodeBlockNode(_context);
            node.Run(statements, null, callSuperClassContructorsExpressions, kindOfCompilation);

            return ConvertToCompiledFunctionBody(node.Result);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<Field> fields)
        {
            var node = new FieldsNode(_context);
            node.Run(fields);

            return ConvertToCompiledFunctionBody(node.Result);
        }

        private CompiledFunctionBody ConvertToCompiledFunctionBody(List<IntermediateScriptCommand> resultCommandsList)
        {
            NumerateSequence(resultCommandsList);

            var sehIndex = 0;

            var result = new CompiledFunctionBody();

            foreach (var command in resultCommandsList)
            {
                result.Commands.Add(command.Position, ConvertIntermediateScriptCommandToScriptCommand(command, result.SEH, ref sehIndex));
            }

            return result;
        }

        private void NumerateSequence(List<IntermediateScriptCommand> commandsList)
        {
            var n = 0;

            foreach (var command in commandsList)
            {
                command.Position = n;
                n++;
            }
        }

        private ScriptCommand ConvertIntermediateScriptCommandToScriptCommand(IntermediateScriptCommand initialCommand, Dictionary<int, SEHGroup> sehDict, ref int sehIndex)
        {
            var result = new ScriptCommand();

            result.OperationCode = initialCommand.OperationCode;
            result.Position = initialCommand.Position;
            result.Value = initialCommand.Value;

            if(initialCommand.JumpToMe != null)
            {
                result.TargetPosition = initialCommand.JumpToMe.Position;
            }
            result.KindOfOperator = initialCommand.KindOfOperator;
            result.CountParams = initialCommand.CountParams;

            if (initialCommand.SEHGroup != null)
            {
                var resultSEHGroup = new SEHGroup();
                sehDict[sehIndex] = resultSEHGroup;
                resultSEHGroup.AfterPosition = initialCommand.SEHGroup.AfterPosition.Position;
                result.TargetPosition = sehIndex;
                sehIndex++;

                var sehItemsList = new List<SEHItem>();

                foreach (var initialSEHItem in initialCommand.SEHGroup.Items)
                {
                    var sehItem = new SEHItem();
                    sehItemsList.Add(sehItem);

                    sehItem.VariableName = initialSEHItem.VariableName;
                    sehItem.Condition = initialSEHItem.Condition;
                    sehItem.TargetPosition = initialSEHItem.JumpToMe.Position;
                }

                resultSEHGroup.Items = sehItemsList.OrderByDescending(p => p.Condition != null).ThenByDescending(p => p.VariableName != null && !p.VariableName.IsEmpty).ToList();

            }

            return result;
        }
    }
}
