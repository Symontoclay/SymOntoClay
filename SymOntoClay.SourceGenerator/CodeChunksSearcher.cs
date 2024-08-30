using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.SourceGenerator
{
    public class CodeChunksSearcher
    {
        public CodeChunksSearcher(GeneratorExecutionContext context)
        {
            _context = context;
        }

        private readonly GeneratorExecutionContext _context;

        private List<string> _targetConstructors = new List<string>()
        {
            "LoggedCodeChunkFunctorWithoutResult",
            "LoggedCodeChunkFunctorWithResult",
            "LoggedCodeChunkSyncFunctorWithoutResult",
            "LoggedCodeChunkSyncFunctorWithResult"
        };

        private List<(string FirstIdenfifier, string SecondIdenfifier)> _targetInvocations = new List<(string FirstIdenfifier, string SecondIdenfifier)>()
        {
            ("LoggedCodeChunkFunctorWithoutResult", "Run"),
            ("LoggedCodeChunkFunctorWithResult", "Run"),
            ("LoggedCodeChunkSyncFunctorWithoutResult", "Run"),
            ("LoggedCodeChunkSyncFunctorWithResult", "Run"),
            (null, "CreateCodeChunk"),
            (null, "CreateSyncCall")
        };

        public List<TargetCodeChunksCompilationUnit> Run()
        {
            var result = new List<TargetCodeChunksCompilationUnit>();

            var syntaxTrees = _context.Compilation.SyntaxTrees;

            var context = new CodeChunkSearchingContext();

            foreach (var syntaxTree in syntaxTrees)
            {
                if (syntaxTree.FilePath.EndsWith(".g.cs"))
                {
                    continue;
                }

                var codeChunkItemsResult = new List<CodeChunkItem>();
                var usings = new List<string>();

                ProcessSyntaxTree(syntaxTree, context, ref codeChunkItemsResult, ref usings);

                if (codeChunkItemsResult.Count > 0)
                {
                    var item = new TargetCodeChunksCompilationUnit()
                    {
                        FilePath = syntaxTree.FilePath,
                        CodeChunkItems = codeChunkItemsResult,
                        Usings = usings
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, CodeChunkSearchingContext context, ref List<CodeChunkItem> result, ref List<string> usings)
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
                ProcessNamespaceDeclaration(namespaceDeclaration, context, ref result);
            }
        }

        private void ProcessNamespaceDeclaration(SyntaxNode namespaceDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
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
                ProcessClassDeclaration(classDeclaration, context, ref result);
            }
        }

        private void ProcessClassDeclaration(SyntaxNode classDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            var childNodes = classDeclaration?.ChildNodes();

            var methodDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.MethodDeclaration));

            if (methodDeclarations.Count() == 0)
            {
                return;
            }

            foreach (var methodDeclaration in methodDeclarations)
            {
                ProcessMethodDeclaration(methodDeclaration, context, ref result);
            }
        }

        private void ProcessMethodDeclaration(SyntaxNode methodDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            var childNodes = methodDeclaration?.ChildNodes();

            var block = childNodes.FirstOrDefault(p => p.IsKind(SyntaxKind.Block));

            ProcessMethodBlock(block, context, ref result);
        }

        private void ProcessMethodBlock(SyntaxNode block, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            var childNodes = block?.ChildNodes();

            if (childNodes.Count() == 0)
            {
                return;
            }

            foreach (var childNode in childNodes)
            {
                ProcessMethodBlockChildNodes(childNode, context, ref result);
            }
        }

        private void ProcessMethodBlockChildNodes(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            if (node.IsKind(SyntaxKind.InvocationExpression))
            {
                if (ShouldCatchInvocationExpression(node))
                {
                    ProcessTargetInvocationExpression(node, context, ref result);
                }
            }
            else
            {
                if (node.IsKind(SyntaxKind.ObjectCreationExpression) && ShouldCatchObjectCreationExpression(node))
                {
                    ProcessTargetObjectCreationExpression(node, context, ref result);
                }
            }

            var childNodes = node?.ChildNodes();

            foreach (var childNode in childNodes)
            {
                ProcessMethodBlockChildNodes(childNode, context, ref result);
            }
        }

        private void ProcessTargetInvocationExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            ProcessTargetExpression(node, context, ref result);
        }

        private void ProcessTargetObjectCreationExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            ProcessTargetExpression(node, context, ref result);
        }

        private void ProcessTargetExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
            var argumentList = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.ArgumentList));

            if (argumentList == null)
            {
                return;
            }

            var identifierArgument = argumentList.ChildNodes().FirstOrDefault(p => p.ChildNodes().Any(x => x.IsKind(SyntaxKind.StringLiteralExpression)));

            if (identifierArgument == null)
            {
                return;
            }

            var identifier = GeneratorsHelper.ToString(identifierArgument.GetText()).Replace('"', ' ').Trim();

            var lambdas = argumentList.ChildNodes().Where(p => p.ChildNodes().Any(x => x.IsKind(SyntaxKind.ParenthesizedLambdaExpression)))
                .SelectMany(p => p.ChildNodes().Where(x => x.IsKind(SyntaxKind.ParenthesizedLambdaExpression))).Select(p => p as ParenthesizedLambdaExpressionSyntax).ToList();

            if (lambdas.Count() == 0)
            {
                return;
            }

            var resultItem = new CodeChunkItem()
            {
                Identifier = identifier,
                Namespace = context.Namespace,
                Lambdas = lambdas
            };

            result.Add(resultItem);
        }

        private bool ShouldCatchInvocationExpression(SyntaxNode node)
        {
            var simpleMemberAccessExpression = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.SimpleMemberAccessExpression));

            if (simpleMemberAccessExpression == null)
            {
                return false;
            }

            var childNodes = simpleMemberAccessExpression?.ChildNodes();

            if (childNodes.Count() != 2)
            {
                return false;
            }

            var firstIdentifier = string.Empty;
            var secondIdentifier = string.Empty;

            var firstSyntaxNode = childNodes.ElementAt(0);

            if (firstSyntaxNode.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = firstSyntaxNode as IdentifierNameSyntax;

                if (identifierNameSyntax == null)
                {
                    return false;
                }

                firstIdentifier = identifierNameSyntax.Identifier.Text;
            }
            else
            {
                if (firstSyntaxNode.IsKind(SyntaxKind.GenericName))
                {
                    var genericNameSyntax = firstSyntaxNode as GenericNameSyntax;

                    if (genericNameSyntax == null)
                    {
                        return false;
                    }

                    firstIdentifier = genericNameSyntax.Identifier.Text;
                }
                else
                {
                    return false;
                }
            }

            var secondSyntaxNode = childNodes.ElementAt(1);

            if (secondSyntaxNode.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = secondSyntaxNode as IdentifierNameSyntax;

                if (identifierNameSyntax == null)
                {
                    return false;
                }

                secondIdentifier = identifierNameSyntax.Identifier.Text;
            }
            else
            {
                if (secondSyntaxNode.IsKind(SyntaxKind.GenericName))
                {
                    var genericNameSyntax = secondSyntaxNode as GenericNameSyntax;

                    if (genericNameSyntax == null)
                    {
                        return false;
                    }

                    secondIdentifier = genericNameSyntax.Identifier.Text;
                }
                else
                {
                    return false;
                }
            }

            return _targetInvocations.Any(p => (string.IsNullOrWhiteSpace(p.FirstIdenfifier) || p.FirstIdenfifier == firstIdentifier) &&
                (string.IsNullOrWhiteSpace(p.SecondIdenfifier) || p.SecondIdenfifier == secondIdentifier));
        }

        private bool ShouldCatchObjectCreationExpression(SyntaxNode node)
        {
            var genericName = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.GenericName));

            if (genericName == null)
            {
                var identifierName = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.IdentifierName));

                if (identifierName == null)
                {
                    return false;
                }

                var identifierNameSyntax = identifierName as IdentifierNameSyntax;

                if (identifierNameSyntax != null)
                {
                    return false;
                }

                return _targetConstructors.Contains(identifierNameSyntax.Identifier.Text);
            }

            var genericNameSyntax = genericName as GenericNameSyntax;

            if (genericNameSyntax == null)
            {
                return false;
            }

            return _targetConstructors.Contains(genericNameSyntax.Identifier.Text);
        }
    }
}
