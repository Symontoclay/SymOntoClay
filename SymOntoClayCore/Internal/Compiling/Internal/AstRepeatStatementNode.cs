using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstRepeatStatementNode : BaseNode
    {
        public AstRepeatStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstRepeatStatement statement)
        {
#if DEBUG
            Log($"statement = {statement}");
#endif

            throw new NotImplementedException();
        }
    }
}
