using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ExecutableCodeBlock : CodeItem, IExecutable
    {
        public ExecutableCodeBlock(List<AstStatement> statements, CompiledFunctionBody compiledFunctionBody)
        {
            throw new NotImplementedException("D3132966-2BFB-44E0-B6AA-2F113D7C4555");
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.ExecutableCodeBlock;

        /// <inheritdoc/>
        public override bool IsExecutableCodeBlock => true;

        /// <inheritdoc/>
        public override ExecutableCodeBlock AsExecutableCodeBlock => this;

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iArgumentsList;
        private IList<IFunctionArgument> _iArgumentsList = new List<IFunctionArgument>();


    }
}
