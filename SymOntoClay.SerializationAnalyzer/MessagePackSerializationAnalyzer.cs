using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SymOntoClay.SerializationAnalyzer
{
    [Generator]
    public class MessagePackSerializationAnalyzer : ISourceGenerator
    {
        private static readonly string DiagnosticDescriptorCategory = "MessagePack";

        private static readonly DiagnosticDescriptor MissedAnnotationRule = new DiagnosticDescriptor(
            id: "MP001",
            title: "MessagePack Key Attribute",
            messageFormat: "Property '{0}' is not annotated with [Key] or [IgnoreMember]",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All properties in [MessagePackObject] must be annotated with [Key] or [IgnoreMember]."
        );

        private static readonly DiagnosticDescriptor KeyNoArgumentsAnnotationRule = new DiagnosticDescriptor(
            id: "MP002",
            title: "MessagePack Key Attribute",
            messageFormat: "Property '{0}' has no arguments in the annotation [Key]",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All properties in [MessagePackObject] must have one argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyManyArgumentsAnnotationRule = new DiagnosticDescriptor(
            id: "MP003",
            title: "MessagePack Key Attribute",
            messageFormat: "Property '{0}' has many arguments in the annotation [Key]",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All properties in [MessagePackObject] must have one argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyNotIntArgumentAnnotationRule = new DiagnosticDescriptor(
            id: "MP004",
            title: "MessagePack Key Attribute",
            messageFormat: "Property '{0}' has non int argument in the annotation [Key]",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All properties in [MessagePackObject] must have int argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyNotSequentialArgumentAnnotationRule = new DiagnosticDescriptor(
            id: "MP005",
            title: "Key Attribute is not sequential",
            messageFormat: "Property '{0}' violates the required consecutive ordering of [Key] attributes.",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Key indices must be sequential without gaps."
        );

        private static readonly DiagnosticDescriptor BaseClassWithoutMessagePackObjectAttributeRule = new DiagnosticDescriptor(
            id: "MP006",
            title: "Base class missing [MessagePackObject]",
            messageFormat: "Base class '{0}' must be annotated with [MessagePackObject] attribute",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All base classes used in MessagePack serialization must be annotated with [MessagePackObject]."
        );

        private static readonly DiagnosticDescriptor ClassIsNotRegisteredInUnionAttributeOfBaseClassRule = new DiagnosticDescriptor(
            id: "MP007",
            title: "Class is not registered in Union attribute of base class",
            messageFormat: "Class '{0}' must be registered in Union attribute of its base class",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "All derived classes participating in Union serialization must be explicitly registered in the Union attribute of their base class."
        );

        public static readonly DiagnosticDescriptor UnionAttributeHasInvalidArgumentCountRule = new DiagnosticDescriptor(
            id: "MP008",
            title: "Union attribute has invalid number of arguments",
            messageFormat: "Union attribute on '{0}' must have exactly 2 arguments (tag and type), but found {1}",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "MessagePack's Union attribute requires exactly two arguments: an integer tag and a Type. Any other number of arguments is invalid and should be corrected."
        );

        public static readonly DiagnosticDescriptor UnionAttributeIndexIsNotIntRule = new DiagnosticDescriptor(
            id: "MP009",
            title: "Union attribute index is not an int",
            messageFormat: "Union attribute on '{0}' must use an integer index as the first argument, but found {1}",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "MessagePack's Union attribute requires the first argument to be an integer tag. Any other type is invalid and should be corrected."
        );

        public static readonly DiagnosticDescriptor UnionAttributeIndicesAreNotSequentialRule = new DiagnosticDescriptor(
            id: "MP010",
            title: "Union attribute indices are not sequential",
            messageFormat: "Union attributes on '{0}' must have sequential integer indices starting from 0, but found a gap or mismatch",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "MessagePack's Union attribute requires indices to be sequential integers starting from 0. Any gaps or non-sequential ordering may cause serialization errors."
        );

        public static readonly DiagnosticDescriptor DerivedClassWithoutMessagePackObjectAttributeRule = new DiagnosticDescriptor(
            id: "MP011",
            title: "Derived class missing [MessagePackObject] attribute",
            messageFormat: "Derived class '{0}' of base class '{1}' must be annotated with [MessagePackObject] attribute",
            category: DiagnosticDescriptorCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "When a base class is annotated with [MessagePackObject] and [Union], all derived classes participating in serialization must also be annotated with [MessagePackObject]."
        );

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var compilation = context.Compilation;

                var allTypes = GetAllTypes(compilation.GlobalNamespace).ToList();

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

                        if (symbol == null)
                        {
                            continue;
                        }

                        if(!HasBaseClassMessagePackObjectAttribute(symbol))
                        {
                            context.ReportDiagnostic(
                            Diagnostic.Create(
                                BaseClassWithoutMessagePackObjectAttributeRule,
                                cls.GetLocation(),
                                cls.Identifier.Text));

                            continue;
                        }

                        if (!IsRegistredInBaseClass(symbol, cls.Identifier.Text))
                        {
                            context.ReportDiagnostic(
                            Diagnostic.Create(
                                ClassIsNotRegisteredInUnionAttributeOfBaseClassRule,
                                cls.GetLocation(),
                                cls.Identifier.Text));

                            continue;
                        }

                        var unionAttributes = cls.AttributeLists
                            .SelectMany(p => p.Attributes)
                            .Where(p => p.Name.ToString() == "Union")
                            .ToList();

                        if(unionAttributes.Count > 0)
                        {
                            var previousUnionKeyIndex = -1;

                            foreach (var attr in unionAttributes)
                            {
                                var attrArgs = attr.ArgumentList.Arguments;


                                if (attrArgs.Count != 2)
                                {
                                    var diagnostic = Diagnostic.Create(
                                        UnionAttributeHasInvalidArgumentCountRule,
                                        attr.GetLocation(),
                                        cls.Identifier.Text,
                                        attrArgs.Count);
                                    context.ReportDiagnostic(diagnostic);

                                    continue;
                                }

                                var firstArg = attrArgs.First();

                                if(!int.TryParse(firstArg.ToString(), out var unionKeyIndex))
                                {
                                    var diagnostic = Diagnostic.Create(
                                        UnionAttributeIndexIsNotIntRule,
                                        attr.GetLocation(),
                                        cls.Identifier.Text,
                                        firstArg.ToString());
                                    context.ReportDiagnostic(diagnostic);

                                    continue;
                                }

                                if(unionKeyIndex != previousUnionKeyIndex +1)
                                {
                                    var diagnostic = Diagnostic.Create(
                                        UnionAttributeIndicesAreNotSequentialRule,
                                        cls.GetLocation(),
                                        cls.Identifier.Text);
                                    context.ReportDiagnostic(diagnostic);
                                }

                                previousUnionKeyIndex = unionKeyIndex;
                            }
                        }

                        var derivedTypes = allTypes
                            .Where(t => t.BaseType?.Equals(symbol, SymbolEqualityComparer.Default) == true)
                            .ToList();

                        if(derivedTypes.Count > 0)
                        {
                            var derivedTypesWithoutAttr = derivedTypes.Where(p => !p.GetAttributes().Any(x => x.AttributeClass?.Name == "MessagePackObjectAttribute")).ToList();

                            foreach (var derivedType in derivedTypesWithoutAttr)
                            {
                                var diagnostic = Diagnostic.Create(
                                    DerivedClassWithoutMessagePackObjectAttributeRule,
                                    derivedType.Locations.First(),
                                    derivedType.Name,
                                    cls.Identifier.Text);

                                context.ReportDiagnostic(diagnostic);
                            }
                        }

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

                            markedPropsNames.Add(ignoredProp.Identifier.Text);
                        }

                        var previousKeyIndex = GetLastKeyIndexFromBaseClass(symbol);

                        var serializedProps = allFilteredProps.Select(p => p.Item1)
                            .Where(c => c.AttributeLists
                            .Any(a => a.Attributes.Any(p => p.Name.ToString() == "Key")))
                            .ToList();

                        for (var i = 0; i < serializedProps.Count; i++)
                        {
                            var serializedProp = serializedProps[i];

                            var propName = serializedProp.Identifier.Text;

                            markedPropsNames.Add(propName);

                            var keyAttribute = serializedProp.AttributeLists.SelectMany(p => p.Attributes).Single(p => p.Name.ToString() == "Key");

                            var keyAttributeArgumentsCount = keyAttribute.ArgumentList.Arguments.Count;

                            if (keyAttributeArgumentsCount == 0)
                            {
                                var diagnostic = Diagnostic.Create(KeyNoArgumentsAnnotationRule, serializedProp.GetLocation(),
                                           propName);
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            if(keyAttributeArgumentsCount > 1)
                            {
                                var diagnostic = Diagnostic.Create(KeyManyArgumentsAnnotationRule, serializedProp.GetLocation(),
                                    propName);
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            var attributeArg = keyAttribute.ArgumentList.Arguments.Single();

                            if(!int.TryParse(attributeArg.ToString(), out var keyIndex))
                            {
                                var diagnostic = Diagnostic.Create(KeyNotIntArgumentAnnotationRule, serializedProp.GetLocation(),
                                    propName);
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            if(keyIndex != previousKeyIndex + 1)
                            {
                                var diagnostic = Diagnostic.Create(KeyNotSequentialArgumentAnnotationRule, serializedProp.GetLocation(),
                                    propName);
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            previousKeyIndex = keyIndex;
                        }

                        for (var i = 0; i < allFilteredProps.Count; i++)
                        {
                            var allProp = allFilteredProps[i];

                            var propName = allProp.Item1.Identifier.Text;

                            if (!markedPropsNames.Contains(propName))
                            {
                                var diagnostic = Diagnostic.Create(MissedAnnotationRule, allProp.Item1.GetLocation(),
                                           propName);
                                context.ReportDiagnostic(diagnostic);

                                continue;
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

        private Dictionary<INamedTypeSymbol, int> _getLastKeyIndexFromBaseClassCache = new Dictionary<INamedTypeSymbol, int>();

        private int GetLastKeyIndexFromBaseClass(INamedTypeSymbol symbol)
        {
            if(_getLastKeyIndexFromBaseClassCache.TryGetValue(symbol, out int chachedIndex))
            {
                return chachedIndex;
            }

            var baseType = symbol.BaseType;

            while (baseType != null && baseType.Name != "Object")
            {
                var lastKeyIndex = ProcessGetLastKeyIndexFromBaseClass(baseType);

                if(lastKeyIndex != -1)
                {
                    _getLastKeyIndexFromBaseClassCache[symbol] = lastKeyIndex;

                    return lastKeyIndex;
                }

                baseType = baseType.BaseType;
            }

            _getLastKeyIndexFromBaseClassCache[symbol] = -1;

            return -1;
        }

        private int ProcessGetLastKeyIndexFromBaseClass(INamedTypeSymbol baseType)
        {
            var propsBeforeFiltering = baseType.GetMembers().OfType<IPropertySymbol>().Select(p => ((PropertyDeclarationSyntax)null, p));

            var properties = FilterAllTargetProperties(propsBeforeFiltering).Where(p => p.Item2.GetAttributes().Any(y => y.AttributeClass?.Name == "KeyAttribute"));

            var keysAttributes = properties.SelectMany(p => p.Item2.GetAttributes()).Where(p => p.AttributeClass?.Name == "KeyAttribute");

            var keyValues = keysAttributes.SelectMany(p => p.ConstructorArguments).Where(p => int.TryParse(p.Value.ToString(), out var intVal)).Select(p => int.Parse(p.Value.ToString())).ToList();

            if(keyValues.Count == 0)
            {
                return -1;
            }

            return keyValues.Max();
        }

        private bool HasBaseClassMessagePackObjectAttribute(INamedTypeSymbol symbol)
        {
            var baseType = symbol.BaseType;

            if(baseType == null)
            {
                return true;
            }
            
            if(baseType.Name == "Object")
            {
                return true;
            }
 
            return baseType.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == "MessagePackObjectAttribute");
        }

        private bool IsRegistredInBaseClass(INamedTypeSymbol symbol, string className)
        {
            var baseType = symbol.BaseType;

            if (baseType == null)
            {
                return true;
            }

            if (baseType.Name == "Object")
            {
                return true;
            }

            var unionAttrs = baseType.GetAttributes().Where(p => p.AttributeClass?.Name == "UnionAttribute");
 
            var hasClass = unionAttrs.Any(attr =>
                    attr.ConstructorArguments.Any(arg =>
                        arg.Value is INamedTypeSymbol typeSymbol &&
                        typeSymbol.Name == className));

            return hasClass;
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol ns)
        {
            foreach (var type in ns.GetTypeMembers())
            {
                yield return type;
            }

            foreach (var nestedNs in ns.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypes(nestedNs))
                {
                    yield return type;
                }
            }
        }
    }
}
