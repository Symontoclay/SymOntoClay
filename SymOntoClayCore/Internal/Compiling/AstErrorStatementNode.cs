using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class AstErrorStatementNode : BaseNode
    {
        public AstErrorStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstErrorStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            CompileValue(statement.RuleInstanceValue);

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.Error;
            AddCommand(command);

            //throw new NotImplementedException();
        }
    }
}
