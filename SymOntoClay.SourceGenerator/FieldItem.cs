using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class FieldItem : BaseFieldItem
    {
        public FieldDeclarationSyntax SyntaxNode { get; set; }

        public string Identifier => GeneratorsHelper.GetFieldIdentifier(SyntaxNode);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(KindFieldType)} = {KindFieldType}");
            sb.AppendLine($"{nameof(Identifier)} = {Identifier}");
            //sb.AppendLine($"{nameof()} = {}");
            return sb.ToString();
        }
    }
}
