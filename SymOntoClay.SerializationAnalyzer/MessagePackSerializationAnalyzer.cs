using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SymOntoClay.SerializationAnalyzer
{
    [Generator]
    public class MessagePackSerializationAnalyzer : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "MP001",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' is not annotated with [Key] or [IgnoreMember]", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "All properties in [MessagePackObject] must be annotated with [Key] or [IgnoreMember]."
        );

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var compilation = context.Compilation;

                foreach (var tree in compilation.SyntaxTrees)
                {
#if DEBUG
                    //FileLogger.WriteLn($"tree.FilePath = {tree.FilePath}");
#endif

                    var semanticModel = compilation.GetSemanticModel(tree);
                    var classes = tree.GetRoot()
                        .DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(c => c.AttributeLists
                            .Any(a => a.ToString().Contains("MessagePackObject")));

                    foreach (var cls in classes)
                    {
#if DEBUG
                        FileLogger.WriteLn($"cls.Identifier.Text = {cls.Identifier.Text}");
#endif

                        var propsBeforeFiltering = cls.Members
                            .OfType<PropertyDeclarationSyntax>()
                            .Select(p => (p, semanticModel.GetDeclaredSymbol(p) as IPropertySymbol));

                        var allFilteredProps = FilterAllTargetProperties(propsBeforeFiltering)
                            .ToList();

                        var markedPropsNames = new List<string>();

                        var ignoredProps = allFilteredProps.Select(p => p.Item1)
                            .Where(c => c.AttributeLists
                            .Any(a => a.Attributes.Any(p => p.Name.ToString() == "IgnoreMember")))
                            .ToList();

                        for (var i = 0; i < ignoredProps.Count; i++)
                        {
                            var ignoredProp = ignoredProps[i];

#if DEBUG
                            FileLogger.WriteLn($"ignoredProp.Identifier.Text = {ignoredProp.Identifier.Text}");
#endif

                            markedPropsNames.Add(ignoredProp.Identifier.Text);
                        }

                        var serializedProps = allFilteredProps.Select(p => p.Item1)
                            .Where(c => c.AttributeLists
                            .Any(a => a.Attributes.Any(p => p.Name.ToString() == "Key")))
                            .ToList();

                        for (var i = 0; i < serializedProps.Count; i++)
                        {
                            var serializedProp = serializedProps[i];

#if DEBUG
                            FileLogger.WriteLn($"serializedProp.Identifier.Text = {serializedProp.Identifier.Text}");
#endif

                            markedPropsNames.Add(serializedProp.Identifier.Text);
                        }

                        for (var i = 0; i < allFilteredProps.Count; i++)
                        {
                            var allProp = allFilteredProps[i];

#if DEBUG
                            FileLogger.WriteLn($"allProp.Name = {allProp.Item1.Identifier.Text}");
#endif

                            var propName = allProp.Item1.Identifier.Text;

                            if (!markedPropsNames.Contains(propName))
                            {
#if DEBUG
                                FileLogger.WriteLn("Spkipped!!!!!");
#endif

                                var diagnostic = Diagnostic.Create(Rule, allProp.Item1.GetLocation(),
                                           $"Property '{propName}' is not annotated with the Key/IgnoreMember attribute");
                                context.ReportDiagnostic(diagnostic);
                            }

                            foreach (var a in allProp.Item1.AttributeLists)
                            {
#if DEBUG
                                FileLogger.WriteLn($"a = {a}");
#endif
                            }


                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                FileLogger.WriteLn(e.ToString());
#endif

                throw;
            }
        }

        private IEnumerable<(PropertyDeclarationSyntax, IPropertySymbol)> FilterAllTargetProperties(IEnumerable<(PropertyDeclarationSyntax, IPropertySymbol)> source)
        {
            return source.Where(p =>
                            !p.Item2.IsAbstract &&
                            !p.Item2.IsStatic &&
                            p.Item2.SetMethod != null &&
                            (p.Item2.SetMethod.DeclaredAccessibility == Accessibility.Public || p.Item2.SetMethod.IsInitOnly)
                        );
        }
    }
}
