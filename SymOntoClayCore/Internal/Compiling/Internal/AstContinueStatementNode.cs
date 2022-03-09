using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstContinueStatementNode : BaseNode
    {
        public AstContinueStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstContinueStatement statement, LoopCompilingContext loopCompilingContext)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            var jumpCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.JumpTo, JumpToMe = loopCompilingContext.FirstCommand };

            AddCommand(jumpCommand);

            //throw new NotImplementedException();
        }
    }
}
