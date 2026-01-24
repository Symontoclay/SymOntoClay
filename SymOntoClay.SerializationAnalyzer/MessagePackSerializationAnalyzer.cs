using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SymOntoClay.SerializationAnalyzer
{
    [Generator]
    public class MessagePackSerializationAnalyzer : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor MissedAnnotationRule = new DiagnosticDescriptor(
            id: "MP001",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' is not annotated with [Key] or [IgnoreMember]", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "All properties in [MessagePackObject] must be annotated with [Key] or [IgnoreMember]."
        );

        private static readonly DiagnosticDescriptor KeyNoArgumentsAnnotationRule = new DiagnosticDescriptor(
            id: "MP002",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' has no arguments in the annotation [Key]", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "All properties in [MessagePackObject] must have one argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyManyArgumentsAnnotationRule = new DiagnosticDescriptor(
            id: "MP003",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' has many arguments in the annotation [Key]", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "All properties in [MessagePackObject] must have one argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyNotIntArgumentAnnotationRule = new DiagnosticDescriptor(
            id: "MP004",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' has non int argument in the annotation [Key]", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "All properties in [MessagePackObject] must have int argument in the annotation [Key]."
        );

        private static readonly DiagnosticDescriptor KeyNotSequentialArgumentAnnotationRule = new DiagnosticDescriptor(
            id: "MP005",                          // unique diagnostic identifier
            title: "MessagePack Key Attribute",   // short title
            messageFormat: "Property '{0}' violates the required consecutive ordering of [Key] attributes.", // message format
            category: "MessagePack",              // category (any string)
            defaultSeverity: DiagnosticSeverity.Error, // severity level (Error, Warning, Info, Hidden)
            isEnabledByDefault: true,             // whether the diagnostic is enabled by default
            description: "Key indices must be sequential without gaps."
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

                        var symbol = semanticModel.GetDeclaredSymbol(cls) as INamedTypeSymbol;

                        if (symbol == null)
                        {
                            continue;
                        }

                        if(!HasBaseClassMessagePackObjectAttribute(symbol))
                        {
                            //TODO: check this

                            throw new NotImplementedException();
                        }

                        if (!IsRegistredInBaseClass(symbol, cls.Identifier.Text))
                        {
                            //TODO: check this

                            throw new NotImplementedException();
                        }

                        var unionAttributes = cls.AttributeLists
                            .SelectMany(p => p.Attributes)
                            .Where(p => p.Name.ToString() == "Union")
                            .ToList();

#if DEBUG
                        FileLogger.WriteLn($"unionAttributes.Count = {unionAttributes.Count}");
#endif

                        if(unionAttributes.Count > 0)
                        {
                            //TODO: check order of union attributes

                            throw new NotImplementedException();
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

#if DEBUG
                            FileLogger.WriteLn($"ignoredProp.Identifier.Text = {ignoredProp.Identifier.Text}");
#endif

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

#if DEBUG
                            FileLogger.WriteLn($"serializedProp.Identifier.Text = {serializedProp.Identifier.Text}");
#endif

                            var propName = serializedProp.Identifier.Text;

                            markedPropsNames.Add(propName);

                            var keyAttribute = serializedProp.AttributeLists.SelectMany(p => p.Attributes).Single(p => p.Name.ToString() == "Key");

#if DEBUG
                            FileLogger.WriteLn($"keyAttribute.ArgumentList.Arguments.Count = {keyAttribute.ArgumentList.Arguments.Count}");
#endif

                            var keyAttributeArgumentsCount = keyAttribute.ArgumentList.Arguments.Count;

                            if (keyAttributeArgumentsCount == 0)
                            {
                                var diagnostic = Diagnostic.Create(KeyNoArgumentsAnnotationRule, serializedProp.GetLocation(),
                                           $"Property '{propName}' has no arguments in the annotation [Key]");
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            if(keyAttributeArgumentsCount > 1)
                            {
                                var diagnostic = Diagnostic.Create(KeyManyArgumentsAnnotationRule, serializedProp.GetLocation(),
                                    $"Property '{propName}' has many arguments in the annotation [Key]");
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            var attributeArg = keyAttribute.ArgumentList.Arguments.Single();

                            if(!int.TryParse(attributeArg.ToString(), out var keyIndex))
                            {
                                var diagnostic = Diagnostic.Create(KeyNotIntArgumentAnnotationRule, serializedProp.GetLocation(),
                                    $"Property '{propName}' has non int argument in the annotation [Key]");
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

#if DEBUG
                            FileLogger.WriteLn($"keyIndex = {keyIndex}");
#endif

                            if(keyIndex != previousKeyIndex + 1)
                            {
                                var diagnostic = Diagnostic.Create(KeyNotSequentialArgumentAnnotationRule, serializedProp.GetLocation(),
                                    $"Property '{propName}' violates the required consecutive ordering of [Key] attributes.");
                                context.ReportDiagnostic(diagnostic);

                                continue;
                            }

                            previousKeyIndex = keyIndex;

                            foreach (var a in keyAttribute.ArgumentList.Arguments)
                            {
#if DEBUG
                                FileLogger.WriteLn($"a = {a}");
#endif
                            }
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

                                var diagnostic = Diagnostic.Create(MissedAnnotationRule, allProp.Item1.GetLocation(),
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

#if DEBUG
            FileLogger.WriteLn($"properties.Count() = {properties.Count()}");
#endif

            var keysAttributes = properties.SelectMany(p => p.Item2.GetAttributes()).Where(p => p.AttributeClass?.Name == "KeyAttribute");

#if DEBUG
            FileLogger.WriteLn($"keysAttributes.Count() = {keysAttributes.Count()}");
#endif

            var keyValues = keysAttributes.SelectMany(p => p.ConstructorArguments).Where(p => int.TryParse(p.Value.ToString(), out var intVal)).Select(p => int.Parse(p.Value.ToString())).ToList();

            if(keyValues.Count == 0)
            {
                return -1;
            }

#if DEBUG
            foreach (var a in keyValues)
            {

                FileLogger.WriteLn($"a = {a}");

            }
#endif

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
#if DEBUG
            FileLogger.WriteLn($"className = {className}");
#endif

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

#if DEBUG
            FileLogger.WriteLn($"hasClass = {hasClass}");
#endif

            return hasClass;
        }
    }
}
