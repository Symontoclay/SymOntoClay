using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
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
            throw new NotImplementedException();
        }
    }
}
