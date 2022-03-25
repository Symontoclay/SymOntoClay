using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstSetDefaultStateStatementNode : BaseNode
    {
        public AstSetDefaultStateStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstSetDefaultStateStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            CompileValue(statement.StateName);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.SetDefaultState
            });

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}
