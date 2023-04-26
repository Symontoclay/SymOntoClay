using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstCancelActionStatementNode : BaseNode
    {
        public AstCancelActionStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstCancelActionStatement statement)
        {
            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.CancelAction
            });
        }
    }
}
