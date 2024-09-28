using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace SymOntoClay.SourceGenerator
{
    public static class GeneratorsHelper
    {
        public static string GetPlainObjectNamespace(string classNamespace)
        {
            return $"{classNamespace}.PlainObjects";
        }

        public static string GetPlainObjectClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);
            sb.Append("Po");

            var typeParameterList = syntaxNode?.ChildNodes().OfType<TypeParameterListSyntax>().FirstOrDefault();

            if (typeParameterList != null)
            {
                sb.Append(ToString(typeParameterList.GetText()).Replace("<", "_").Replace(",", "_").Replace(" ", string.Empty).Replace(">", string.Empty));
            }

            return sb.ToString();
        }

        public static string ExtractNamespaceNameFromUsing(string usingContent)
        {
            if (string.IsNullOrWhiteSpace(usingContent))
            {
                return string.Empty;
            }

            if (usingContent.Contains("=") ||
                usingContent.Contains(" static "))
            {
                return string.Empty;
            }

            if (!usingContent.StartsWith("using "))
            {
                return string.Empty;
            }

            return usingContent.Trim().Substring(6).Replace(";", string.Empty).Trim();
        }

        public static IEnumerable<string> GetAtributeNamesOfClass(SyntaxNode syntaxNode)
        {
            return syntaxNode?.ChildNodes()
                .Where(p => p.IsKind(SyntaxKind.AttributeList))
                .SelectMany(p => p.ChildNodes().Where(x => x.IsKind(SyntaxKind.Attribute)).SelectMany(y => y.ChildNodes().Where(u => u.IsKind(SyntaxKind.IdentifierName))))
                .Select(p => ToString(p.GetText())) ?? Enumerable.Empty<string>();
        }

        public static string GetPropertyIdentifier(PropertyItem propertyItem)
        {
            return GetPropertyIdentifier(propertyItem.SyntaxNode);
        }

        public static string GetPropertyIdentifier(PropertyDeclarationSyntax syntaxNode)
        {
            return syntaxNode.Identifier.Text;
        }

        public static string GetFieldIdentifier(FieldItem fieldItem)
        {
            return GetFieldIdentifier(fieldItem.SyntaxNode);
        }

        public static string GetFieldIdentifier(FieldDeclarationSyntax syntaxNode)
        {
            var variableDeclarator = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclaration))?.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclarator)) as VariableDeclaratorSyntax;

            return variableDeclarator.Identifier.Text;
        }

        public static string ToString(SourceText sourceText)
        {
            var sb = new StringBuilder();

            var inMultilineComment = false;

            foreach (var line in sourceText.Lines)
            {
                var lineStr = line.ToString();

                if (string.IsNullOrWhiteSpace(lineStr))
                {
                    continue;
                }

                lineStr = lineStr.Trim();

#if DEBUG
                //FileLogger.WriteLn($"lineStr = '{lineStr}'");
#endif

                if (!inMultilineComment && lineStr.StartsWith("/*"))
                {
                    inMultilineComment = true;
                    continue;
                }

                if (inMultilineComment && lineStr.EndsWith("*/"))
                {
                    inMultilineComment = false;
                    continue;
                }

                if(inMultilineComment)
                {
                    continue;
                }

#if DEBUG
                //FileLogger.WriteLn($"lineStr = '{lineStr}'");
#endif

                if(lineStr.StartsWith("#"))
                {
                    continue;
                }

#if DEBUG
                //FileLogger.WriteLn($"lineStr = '{lineStr}'");
#endif

                sb.Append(lineStr);
            }

#if DEBUG
            //FileLogger.WriteLn($"sb.ToString().Trim() = '{sb.ToString().Trim()}'");
#endif

            return sb.ToString().Trim();
        }

#if DEBUG
        public static void ShowSyntaxNode(int n, SyntaxNode syntaxNode)
        {
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.GetType().Name = {syntaxNode?.GetType().Name}");
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.Kind() = {syntaxNode?.Kind()}");
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.GetText() = {syntaxNode?.GetText()}");

            var childNodes = syntaxNode?.ChildNodes();

            FileLogger.WriteLn($"{Spaces(n)}childNodes = {childNodes == null}");

            if (childNodes != null)
            {
                FileLogger.WriteLn($"{Spaces(n)}childNodes.Count() = {childNodes.Count()}");

                foreach (var childNode in childNodes)
                {
                    ShowSyntaxNode(n + 4, childNode);
                }
            }
        }
#endif

        public static string Spaces(int n)
        {
            if (n == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < n; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}
