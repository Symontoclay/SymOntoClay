using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstReturnStatementNode : BaseNode
    {
        public AstReturnStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstReturnStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            if(statement.Expression != null)
            {
                throw new NotImplementedException();
            }

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.Return
            });
        }
    }
}
