using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class TargetClassItem
    {
        public string FilePath { get; set; }
        public string Namespace { get; set; }
        public string Identifier { get; set; }
        public ClassDeclarationSyntax SyntaxNode { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(FilePath)} = '{FilePath}'");
            sb.AppendLine($"{nameof(Namespace)} = '{Namespace}'");
            sb.AppendLine($"{nameof(Identifier)} = '{Identifier}'");
            return sb.ToString();
        }
    }
}
