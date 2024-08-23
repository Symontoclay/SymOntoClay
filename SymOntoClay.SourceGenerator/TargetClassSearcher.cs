using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.SourceGenerator
{
    public class TargetClassSearcher
    {
        public TargetClassSearcher(IEnumerable<SyntaxTree> syntaxTrees)
        {
            _syntaxTrees = syntaxTrees;
        }

        private readonly IEnumerable<SyntaxTree> _syntaxTrees;

        public List<TargetCompilationUnit> Run(string attributeName)
        {
            return Run(new List<string> { attributeName });
        }

        public List<TargetCompilationUnit> Run(List<string> attributeNames)
        {
            var result = new List<TargetCompilationUnit>();

            var context = new TargetClassSearcherContext();

            foreach (var syntaxTree in _syntaxTrees)
            {
                if (syntaxTree.FilePath.EndsWith(".g.cs"))
                {
                    continue;
                }

                var classItemsResult = new List<TargetClassItem>();
                var usings = new List<string>();

                ProcessSyntaxTree(syntaxTree, attributeNames, context, ref classItemsResult, ref usings);

                if (classItemsResult.Count > 0)
                {
                    var item = new TargetCompilationUnit()
                    {
                        FilePath = syntaxTree.FilePath,
                        ClassItems = classItemsResult,
                        Usings = usings
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result, ref List<string> usings)
        {
            var root = syntaxTree.GetRoot();

            var childNodes = root?.ChildNodes();

            if (childNodes == null)
            {
                return;
            }

            var namespaceDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.NamespaceDeclaration));

            if (namespaceDeclarations.Count() == 0)
            {
                return;
            }

            var localUsings = childNodes.Where(p => p.IsKind(SyntaxKind.UsingDirective)).Select(p => GeneratorsHelper.ToString(p.GetText()));

            if (localUsings.Any())
            {
                usings.AddRange(localUsings);
            }

            context.FilePath = syntaxTree.FilePath;

            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                ProcessNamespaceDeclaration(namespaceDeclaration, attributeNames, context, ref result);
            }
        }

        private void ProcessNamespaceDeclaration(SyntaxNode namespaceDeclaration, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result)
        {
            var childNodes = namespaceDeclaration?.ChildNodes();

            var classDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.ClassDeclaration));

            if (classDeclarations.Count() == 0)
            {
                return;
            }

            var namespaceIdentifierNode = childNodes.Single(p => p.IsKind(SyntaxKind.QualifiedName) || p.IsKind(SyntaxKind.IdentifierName));

            var namespaceIdentifier = GeneratorsHelper.ToString(namespaceIdentifierNode?.GetText());

            context.Namespace = namespaceIdentifier;

            foreach (var classDeclaration in classDeclarations)
            {
                ProcessClassDeclaration(classDeclaration, attributeNames, context, ref result);
            }
        }

        private void ProcessClassDeclaration(SyntaxNode classDeclaration, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result)
        {
            var attributesList = GeneratorsHelper.GetAtributeNamesOfClass(classDeclaration);

            if (!attributesList.Any(p => attributeNames.Contains(p)))
            {
                return;
            }

            var cSharpClassDeclarationSyntax = (ClassDeclarationSyntax)classDeclaration;

            var resultItem = new TargetClassItem
            {
                FilePath = context.FilePath,
                Namespace = context.Namespace,
                Identifier = cSharpClassDeclarationSyntax.Identifier.ToString(),
                SyntaxNode = cSharpClassDeclarationSyntax
            };

            result.Add(resultItem);
        }
    }
}
