using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public static class GeneratorsHelper
    {
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

                if (inMultilineComment)
                {
                    continue;
                }

#if DEBUG
                //FileLogger.WriteLn($"lineStr = '{lineStr}'");
#endif

                if (lineStr.StartsWith("#"))
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
