/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Compiling.Internal;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

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
            var node = new CodeBlockNode(_context);
            node.Run(statements);

            var resultCommandsList = node.Result;

#if DEBUG
            Log($"resultCommandsList = {resultCommandsList.WriteListToString()}");
#endif

            NumerateSequence(resultCommandsList);

#if DEBUG
            Log($"resultCommandsList (2) = {resultCommandsList.WriteListToString()}");
#endif

            var result = new CompiledFunctionBody();

            foreach (var command in resultCommandsList)
            {
                result.Commands.Add(command.Position, ConvertIntermediateScriptCommandToScriptCommand(command));

#if DEBUG
                if (command.SEHGroup != null)
                {
                    throw new NotImplementedException();
                }
#endif
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

        private ScriptCommand ConvertIntermediateScriptCommandToScriptCommand(IntermediateScriptCommand initialCommand)
        {
#if DEBUG
            Log($"initialCommand = {initialCommand}");
#endif

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

#if DEBUG
            Log($"result = {result}");
#endif

            return result;
        }
    }
}
