using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForAstStatement
    {
        public static string ToHumanizedString(this IEnumerable<AstStatement> statements, DebugHelperOptions options)
        {
            if(statements.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach(var statement in statements)
            {
                sb.AppendLine(statement.ToHumanizedString(options));
            }

            return sb.ToString();
        }
    }
}
