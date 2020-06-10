using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstExpressionStatement: AstStatement
    {
        public override KindOfAstStatement Kind => KindOfAstStatement.Expression;


    }
}
