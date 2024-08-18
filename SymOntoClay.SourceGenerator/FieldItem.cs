using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class FieldItem : BaseFieldItem
    {
        public FieldDeclarationSyntax SyntaxNode { get; set; }

        public override string Identifier => GeneratorsHelper.GetFieldIdentifier(SyntaxNode);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(KindFieldType)} = {KindFieldType}");
            sb.AppendLine($"{nameof(Identifier)} = {Identifier}");
            sb.AppendLine($"{nameof(IsActionKey)} = {IsActionKey}");
            sb.AppendLine($"{nameof(IsActionOrFunc)} = {IsActionOrFunc}");
            //sb.AppendLine($"{nameof()} = {}");
            return sb.ToString();
        }
    }
}
