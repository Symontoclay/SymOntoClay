using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstSetStateStatementNode : BaseNode
    {
        public AstSetStateStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstSetStateStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            CompileValue(statement.StateName);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.SetState
            });

#if DEBUG
            DbgPrintCommands();
#endif
        }
    }
}
