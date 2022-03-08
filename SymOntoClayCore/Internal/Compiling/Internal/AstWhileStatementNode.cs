using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstWhileStatementNode : BaseNode
    {
        public AstWhileStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstWhileStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            var afterCommand = new IntermediateScriptCommand() { OperationCode = OperationCode.Nop };



            throw new NotImplementedException();
        }
    }
}
