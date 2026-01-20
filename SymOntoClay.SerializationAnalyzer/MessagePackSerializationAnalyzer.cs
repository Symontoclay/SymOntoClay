using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SerializationAnalyzer
{
    [Generator]
    public class MessagePackSerializationAnalyzer : ISourceGenerator
    {
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

                        for (var i = 0; i < allFilteredProps.Count; i++)
                        {
                            var allProp = allFilteredProps[i];

#if DEBUG
                            FileLogger.WriteLn($"allProp.Name = {allProp.Item1.Identifier.Text}");
#endif

                            foreach(var a in allProp.Item1.AttributeLists)
                            {
#if DEBUG
                                FileLogger.WriteLn($"a = {a}");
#endif
                            }
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
