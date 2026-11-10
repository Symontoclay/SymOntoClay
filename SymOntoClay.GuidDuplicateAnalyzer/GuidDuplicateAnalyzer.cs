using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            try
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
                                return new GuidLocation(text.ToUpperInvariant(), literal.GetLocation(), ctx.SemanticModel.SyntaxTree);
                            }
                            return null;
                        })
                    .Where(static g => g != null)!;

                // 2. Combine the GUID stream with the .editorconfig configuration provider.
                IncrementalValueProvider<(ImmutableArray<GuidLocation> Guids, AnalyzerConfigOptionsProvider OptionsProvider)> combined =
                guidValues.Collect().Combine(context.AnalyzerConfigOptionsProvider);

                // 3. Generation/analysis step: look for duplicates and provide diagnostics
                context.RegisterSourceOutput(combined, static (spc, source) =>
                {
                    try
                    {
                        var guids = source.Guids;
                        var optionsProvider = source.OptionsProvider;

#if DEBUG
                        //FileLogger.WriteLn($"guids.IsDefaultOrEmpty = {guids.IsDefaultOrEmpty}");
#endif

                        if (guids.IsDefaultOrEmpty)
                        {
                            return;
                        }

                        var firstTree = guids[0].SyntaxTree;
                        var fileOptions = optionsProvider.GetOptions(firstTree);

#if DEBUG
                        //foreach(var key in fileOptions.Keys)
                        //{
                        //    FileLogger.WriteLn($"key = {key}");
                        //}
#endif

                        // We read our custom parameter (returns true/false if it is set)
                        if (fileOptions.TryGetValue("symontoclay.guid_analyzer.ignore_duplicates", out var ignoreVal) &&
                            bool.TryParse(ignoreVal, out bool shouldIgnore) && shouldIgnore)
                        {
#if DEBUG
                            //FileLogger.WriteLn($"ignoreVal = {ignoreVal}");
                            //FileLogger.WriteLn($"shouldIgnore = {shouldIgnore}");
#endif

                            // If the ignore flag is enabled in the config, we abort the analysis.
                            return;
                        }

#if DEBUG
                        //FileLogger.WriteLn($"ignoreVal = {ignoreVal}");
#endif

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
                    }
#if DEBUG
                    catch (Exception e)
#else
                    catch
#endif
                    {
#if DEBUG
                        FileLogger.WriteLn(e.ToString());
#endif
                    }
                });
            }
#if DEBUG
            catch (Exception ex)
#else
            catch
#endif
            {
#if DEBUG
                FileLogger.WriteLn(ex.ToString());
#endif
            }

        }

        private record GuidLocation(string Value, Location Location, SyntaxTree SyntaxTree);
    }
}
