using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SymOntoClay.SourceGenerator
{
    public abstract class BaseFieldItem
    {
        public ClassDeclarationSyntax ClassDeclarationSyntaxNode { get; set; }
        public KindFieldType KindFieldType { get; set; } = KindFieldType.Unknown;
        public SyntaxNode FieldTypeSyntaxNode { get; set; }
    }
}
