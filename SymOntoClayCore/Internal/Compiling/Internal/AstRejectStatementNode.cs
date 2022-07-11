using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstRejectStatementNode : BaseNode
    {
        public AstRejectStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstRejectStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.Reject
            });
        }
    }
}
