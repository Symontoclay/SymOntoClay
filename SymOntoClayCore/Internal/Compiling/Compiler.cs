using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
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
            //Log($"resultCommandsList = {resultCommandsList.WriteListToString()}");
#endif

            var n = 0;
            var result = new CompiledFunctionBody();

            foreach (var command in resultCommandsList)
            {
                command.Position = n;             
                result.Commands.Add(n, command);
                n++;
            }

            return result;
        }
    }
}
