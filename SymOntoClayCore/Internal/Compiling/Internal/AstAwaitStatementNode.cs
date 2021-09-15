using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstAwaitStatementNode : BaseNode
    {
        public AstAwaitStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstAwaitStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            AddCommand(new IntermediateScriptCommand() 
            {
                OperationCode = OperationCode.Await
            });
        }
    }
}
