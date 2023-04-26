using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstWeakCancelActionStatementNode : BaseNode
    {
        public AstWeakCancelActionStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstWeakCancelActionStatement statement)
        {
            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.WeakCancelAction
            });
        }
    }
}
