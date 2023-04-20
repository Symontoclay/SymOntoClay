using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstEventDeclStatementNode : BaseNode
    {
        public AstEventDeclStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstEventDeclStatement statement)
        {
#if DEBUG
            Log($"statement = {statement.ToHumanizedString()}");
#endif

            var node = new ExpressionNode(_context);
            node.Run(statement.Expression);

            AddCommands(node.Result);

#if DEBUG
            DbgPrintCommands();
#endif

            throw new NotImplementedException();
        }
    }
}
