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
using SymOntoClay.Core.Internal.TasksExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class Compiler: BaseContextComponent, ICompiler
    {
        public Compiler(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }
        
        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(AstStatement statement)
        {
            return Compile(new List<AstStatement> { statement}, null, KindOfCompilation.Usual);
        }

        /// <inheritdoc/>
        public List<IntermediateScriptCommand> CompileToIntermediateCommands(AstStatement statement)
        {
            return CompileToIntermediateCommands(new List<AstStatement> { statement }, null, KindOfCompilation.Usual);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<AstStatement> statements)
        {
            return Compile(statements, null, KindOfCompilation.Usual);
        }

        /// <inheritdoc/>
        public List<IntermediateScriptCommand> CompileToIntermediateCommands(List<AstStatement> statements)
        {
            return CompileToIntermediateCommands(statements, null, KindOfCompilation.Usual);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<AstStatement> statements, List<AstExpression> callSuperClassConstructorsExpressions, KindOfCompilation kindOfCompilation)
        {
            return ConvertToCompiledFunctionBody(CompileToIntermediateCommands(statements, callSuperClassConstructorsExpressions, kindOfCompilation));
        }

        /// <inheritdoc/>
        public List<IntermediateScriptCommand> CompileToIntermediateCommands(List<AstStatement> statements, List<AstExpression> callSuperClassConstructorsExpressions, KindOfCompilation kindOfCompilation)
        {
            var node = new CodeBlockNode(_context);
            node.Run(statements, null, callSuperClassConstructorsExpressions, kindOfCompilation);

            return node.Result;
        }

        /// <inheritdoc/>
        public CompiledFunctionBody CompileLambda(AstStatement statement)
        {
            var node = new LambdaNode(_context);
            node.Run(statement);

            return ConvertToCompiledFunctionBody(node.Result);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(List<Field> fields)
        {
            return ConvertToCompiledFunctionBody(CompileToIntermediateCommands(fields));
        }

        /// <inheritdoc/>
        public List<IntermediateScriptCommand> CompileToIntermediateCommands(List<Field> fields)
        {
            var node = new FieldsNode(_context);
            node.Run(fields);

            return node.Result;
        }

        /// <inheritdoc/>
        public List<IntermediateScriptCommand> CompileToIntermediateCommands(List<Property> properties)
        {
            var node = new PropertiesNode(_context);
            node.Run(properties);

            return node.Result;
        }

        /// <inheritdoc/>
        public CompiledFunctionBody Compile(TasksPlan plan)
        {
            var node = new TasksPlanNode(_context);
            node.Run(plan);

            return ConvertToCompiledFunctionBody(node.Result);
        }

        /// <inheritdoc/>
        public CompiledFunctionBody ConvertToCompiledFunctionBody(List<IntermediateScriptCommand> intermediateCommandsList)
        {
            NumerateSequence(intermediateCommandsList);

            var sehIndex = 0;

            var result = new CompiledFunctionBody();

            foreach (var command in intermediateCommandsList)
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
            result.CompoundTask = initialCommand.CompoundTask;

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
