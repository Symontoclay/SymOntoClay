using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SymOntoClay.GuidDuplicateAnalyzer
{
    [Generator]
    public class GuidDuplicateAnalyzer : IIncrementalGenerator
    {
        // Regular expression for finding a GUID (standard format with hyphens)
        private static readonly Regex GuidRegex = new(
            @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$",
            RegexOptions.Compiled);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 1. Selection step: find all string literals in the syntax tree
            IncrementalValuesProvider<GuidLocation> guidValues = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is LiteralExpressionSyntax,
                    transform: static (ctx, _) =>
                    {
                        var literal = (LiteralExpressionSyntax)ctx.Node;
                        if (literal.Token.Value is string text && Guid.TryParse(text, out var _))
                        {
                            return new GuidLocation(text.ToUpperInvariant(), literal.GetLocation());
                        }
                        return null;
                    })
                .Where(static g => g != null)!;

            // 2. Aggregation step: collect ALL GUIDs from the entire project into a single collection
            IncrementalValueProvider<ImmutableArray<GuidLocation>> allGuids = guidValues.Collect();

            // 3. Generation/analysis step: look for duplicates and provide diagnostics
            context.RegisterSourceOutput(allGuids, static (spc, guids) =>
            {
                if (guids.IsDefaultOrEmpty) return;

                // Group by GUID value to find those that appear more than once
                var duplicates = guids
                    .GroupBy(g => g.Value)
                    .Where(g => g.Count() > 1);

                foreach (var duplicateGroup in duplicates)
                {
                    foreach (var item in duplicateGroup)
                    {
                        var descriptor = new DiagnosticDescriptor(
                            id: "GUID001",
                            title: "Duplicate GUID",
                            messageFormat: "GUID '{0}' found in several places in the project.",
                            category: "CodeQuality",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true);

                        spc.ReportDiagnostic(Diagnostic.Create(descriptor, item.Location, duplicateGroup.Key));
                    }
                }
            });
        }

        private record GuidLocation(string Value, Location Location);
    }
}
