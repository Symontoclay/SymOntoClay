using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.SerializationGenerator
{
    [Generator]
    public class MessagePackSerializationGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var identationStep = 4;
                var baseIdentation = 0;
                var classDeclIdentation = baseIdentation + identationStep;
                var classContentDeclIdentation = classDeclIdentation + identationStep;

                var classDeclSpaces = GeneratorsHelper.Spaces(classDeclIdentation);
                var classContentDeclSpaces = GeneratorsHelper.Spaces(classContentDeclIdentation);

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
                        FileLogger.WriteLn($"tree.FilePath = {cls.Identifier.Text}");
#endif

                        var symbol = semanticModel.GetDeclaredSymbol(cls) as INamedTypeSymbol;

                        if (symbol == null) 
                        { 
                            continue;
                        }

                        var sb = new StringBuilder();

                        var root = cls.SyntaxTree.GetCompilationUnitRoot();
                        var usings = root.Usings;

                        foreach (var u in root.Usings)
                        {
                            sb.AppendLine($"using {u.Name.ToString()};");
                        }

                        var ns = GetNamespace(cls);

                        var propsBeforeFiltering = cls.Members
                                .OfType<PropertyDeclarationSyntax>()
                                .Select(p => semanticModel.GetDeclaredSymbol(p) as IPropertySymbol);

                        var props = FilterProperties(propsBeforeFiltering)
                             .ToList();

                        var baseCount = CountBaseProperties(symbol);

                        sb.AppendLine($"namespace {ns}");
                        sb.AppendLine("{");
                        sb.AppendLine($"{classDeclSpaces}public partial class {cls.Identifier.Text}");
                        sb.AppendLine($"{classDeclSpaces}{{");

                        for (var i = 0; i < props.Count; i++)
                        {
                            var prop = props[i];
                            var keyIndex = baseCount + i;
                            var propName = prop.Name;
                            var shadowFieldName = $"_{char.ToLowerInvariant(propName[0])}{propName.Substring(1)}";

                            sb.AppendLine($"{classContentDeclSpaces}private {prop.Type} {shadowFieldName};");
                            sb.AppendLine($"{classContentDeclSpaces}[Key({keyIndex})] public partial {prop.Type} {propName} {{ get => {shadowFieldName}; set => {shadowFieldName} = value; }}");
                        }

                        sb.AppendLine($"{classDeclSpaces}}}");
                        sb.AppendLine("}");

                        context.AddSource($"{cls.Identifier.Text}_Keys.g.cs", sb.ToString());
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

        private static string GetNamespace(ClassDeclarationSyntax cls)
        {
            var ns = cls.Ancestors()
                        .OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault();
            return ns?.Name.ToString() ?? "";
        }

        private IEnumerable<IPropertySymbol> FilterProperties(IEnumerable<IPropertySymbol> source)
        {
            return source.Where(p =>
                            !p.IsAbstract &&
                            !p.IsStatic &&
                            p.SetMethod != null &&
                            (p.SetMethod.DeclaredAccessibility == Accessibility.Public || p.SetMethod.IsInitOnly)
                        );
        }

        private int CountBaseProperties(INamedTypeSymbol symbol)
        {
            var count = 0;
            var baseType = symbol.BaseType;

            while (baseType != null && baseType.Name != "Object")
            {
                var hasAttr = baseType.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == "MessagePackObjectAttribute");

                if (hasAttr)
                {
                    var properties = FilterProperties(baseType.GetMembers()
                        .OfType<IPropertySymbol>());

                    count += properties.Count();
                }

                baseType = baseType.BaseType;
            }

            return count;
        }
    }
}
/*
using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class MessagePackKeyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;

        foreach (var tree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var classes = tree.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(c => c.AttributeLists
                    .Any(a => a.ToString().Contains("MessagePackObject")));

            foreach (var cls in classes)
            {
                var symbol = semanticModel.GetDeclaredSymbol(cls) as INamedTypeSymbol;
                if (symbol == null) continue;

                var props = cls.Members.OfType<PropertyDeclarationSyntax>().ToList();
                var sb = new StringBuilder();

                sb.AppendLine("using MessagePack;");
                sb.AppendLine($"public partial class {cls.Identifier.Text}");
                sb.AppendLine("{");

                // count the number of properties in base classes
                int baseCount = CountBaseProperties(symbol);

                for (int i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    int keyIndex = baseCount + i;
                    sb.AppendLine($"    [Key({keyIndex})] public {prop.Type} {prop.Identifier} {{ get; set; }}");
                }

                sb.AppendLine("}");

                context.AddSource($"{cls.Identifier.Text}_Keys.g.cs", sb.ToString());

                // if the class is abstract — generate Union
                if (symbol.IsAbstract)
                {
                    var derivedTypes = compilation.GlobalNamespace
                        .GetNamespaceMembers()
                        .SelectMany(ns => ns.GetTypeMembers())
                        .Where(t => t.BaseType?.Equals(symbol, SymbolEqualityComparer.Default) == true)
                        .ToList();

                    if (derivedTypes.Any())
                    {
                        var unionSb = new StringBuilder();
                        unionSb.AppendLine("using MessagePack;");
                        unionSb.AppendLine($"[MessagePackObject]");

                        int index = 0;
                        foreach (var d in derivedTypes)
                        {
                            unionSb.AppendLine($"[Union({index}, typeof({d.Name}))]");
                            index++;
                        }

                        unionSb.AppendLine($"public abstract partial class {cls.Identifier.Text} {{ }}");

                        context.AddSource($"{cls.Identifier.Text}_Union.g.cs", unionSb.ToString());
                    }
                }
            }
        }
    }

    private int CountBaseProperties(INamedTypeSymbol symbol)
    {
        int count = 0;
        var baseType = symbol.BaseType;

        while (baseType != null && baseType.Name != "Object")
        {
            var hasAttr = baseType.GetAttributes()
                .Any(a => a.AttributeClass?.Name == "MessagePackObjectAttribute");

            if (hasAttr)
            {
                count += baseType.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Count();
            }

            baseType = baseType.BaseType;
        }

        return count;
    }
}
 */