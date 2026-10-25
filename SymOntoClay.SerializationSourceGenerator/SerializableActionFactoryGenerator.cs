using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SymOntoClay.SerializationSourceGenerator
{
    [Generator]
    public class SerializableActionFactoryGenerator : IIncrementalGenerator
    {
        private static HashSet<string> _targetConstructors = new HashSet<string>()
        {
            "LoggedAltFunctorWithoutResult"
        };

        private static HashSet<(string TypeName, string MethodName)> _targetInvocations = new HashSet<(string TypeName, string MethodName)>()
        {
            ("LoggedAltFunctorWithoutResult", "Run")
        };

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context) 
        {
            var targetNodes = context.SyntaxProvider
                        .CreateSyntaxProvider(
                            // 1. Syntactic filter: look for either method calls (Run) or object creation (new)
                            predicate: static (node, _) => IsCandidateNode(node),

                            // 2. Semantic analysis: check whether it is a constructor or a static method
                            transform: static (ctx, _) => GetTargetInfo(ctx)
                        )
                        .Where(static data => data is not null);

            context.RegisterSourceOutput(targetNodes, static (spc, source) =>
            {
                ExecuteGeneration(spc, source);
            });
        }

        private static bool IsCandidateNode(SyntaxNode node)
        {
            if (node is InvocationExpressionSyntax invocation &&
                        invocation.ArgumentList.Arguments.Count >= 4 &&
                        invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name.Identifier.Text == "Run")
            {
                return true;
            }

            if (node is ObjectCreationExpressionSyntax creation &&
                creation.ArgumentList != null &&
                creation.ArgumentList.Arguments.Count >= 4)
            {
                return true;
            }

            return false;
        }

        private static TargetInfo GetTargetInfo(GeneratorSyntaxContext context)
        {
            var node = context.Node;
            var semanticModel = context.SemanticModel;

            if (node is InvocationExpressionSyntax invocation)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(invocation);
                if (symbolInfo.Symbol is not IMethodSymbol methodSymbol) { return null; }

                // We are testing the static Run method of the target class.
                if (!methodSymbol.IsStatic) 
                { 
                    return null; 
                }

                // ||  != "Run"
                var typeName = methodSymbol.ContainingType.Name;

                if (_targetInvocations.Any(p => p.TypeName == typeName && p.MethodName == methodSymbol.Name)) 
                { 
                    return null; 
                }

                return new TargetInfo(invocation, semanticModel, typeName, isConstructor: false);
            }
            else if (node is ObjectCreationExpressionSyntax creation)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(creation);
                if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol) { return null; }

                // We verify that this is indeed a construction set
                if (constructorSymbol.MethodKind != MethodKind.Constructor) { return null; }

                var typeName = constructorSymbol.ContainingType.Name;

                if (!_targetConstructors.Contains(typeName)) 
                { 
                    return null; 
                }

                // Note: For ObjectCreationExpressionSyntax, the arguments are located in creation.ArgumentList.
                return new TargetInfo(creation, semanticModel, typeName, isConstructor: true);
            }

            return null;
        }

        private static void ExecuteGeneration(SourceProductionContext context, TargetInfo source)
        {
#if DEBUG
            //FileLogger.WriteLn($"Target element processed: {source.TypeName} (Constructor: {source.IsConstructor})");
#endif

            // Safe retrieval of arguments based on node type
            ArgumentListSyntax argList = source.Node switch
            {
                InvocationExpressionSyntax inv => inv.ArgumentList,
                ObjectCreationExpressionSyntax obj => obj.ArgumentList,
                _ => null
            };

            if (argList != null && argList.Arguments.Count >= 4)
            {
                // Second parameter (index 1)
                var guidExpr = argList.Arguments[1].Expression;
                var guid = source.SemanticModel.GetConstantValue(guidExpr).Value?.ToString() ?? "";

                var lambdas = GetAllLambdas(3, argList.Arguments);

#if DEBUG
                //FileLogger.WriteLn($"lambdas.Count = {lambdas.Count}");
#endif

                // Fourth parameter (index 3 — lambda)
                var lambdaExpr = argList.Arguments[3].Expression;
                var lambdaText = lambdaExpr.ToString();

#if DEBUG
                //FileLogger.WriteLn($"guid = {guid}");
                //FileLogger.WriteLn($"lambdaText = {lambdaText}");
#endif

                var safeGuidName = guid.Replace("-", "");

                var usings = GetUsingsForNode(source.Node, source.SemanticModel);

                var serializableActionNamespace = "SymOntoClay.CoreHelper.SerializationToImage.Attributes";
                var serializableActionFactoryNamespace = "SymOntoClay.CoreHelper.SerializationToImage";

                var identationStep = 4;
                var baseIdentation = 0;
                var classDeclIdentation = baseIdentation + identationStep;
                var classDeclSpaces = GeneratorsHelper.Spaces(classDeclIdentation);
                var classContentDeclIdentation = classDeclIdentation + identationStep;
                var classContentDeclSpaces = GeneratorsHelper.Spaces(classContentDeclIdentation);
                var classContentIdentation = classContentDeclIdentation + identationStep;
                var classContentSpaces = GeneratorsHelper.Spaces(classContentIdentation);
                var classContentIdentation_2 = classContentIdentation + identationStep;
                var classContentSpaces_2 = GeneratorsHelper.Spaces(classContentIdentation_2);

                var sb = new StringBuilder();
                sb.AppendLine($"// Auto-generated for GUID: {guid}");
                foreach (var u in usings) 
                {
#if DEBUG
                    //FileLogger.WriteLn($"u = {u}");
#endif
                    sb.AppendLine($"using {u};");
                }

                if(!usings.Contains(serializableActionNamespace))
                {
                    sb.AppendLine($"using {serializableActionNamespace};");
                }

                if (!usings.Contains(serializableActionFactoryNamespace))
                {
                    sb.AppendLine($"using {serializableActionFactoryNamespace};");
                }

                var namespaceForNode = GetNamespaceForNode(source.Node, source.SemanticModel);

#if DEBUG
                //FileLogger.WriteLn($"namespaceForNode = {namespaceForNode}");
#endif

                if(!string.IsNullOrWhiteSpace(namespaceForNode) && !usings.Contains(serializableActionFactoryNamespace))
                {
                    sb.AppendLine($"using {namespaceForNode};");
                }

                sb.AppendLine();
                sb.AppendLine("namespace SymOntoClay.GeneratedFunctors");
                sb.AppendLine("{");
                sb.AppendLine($"{classDeclSpaces}[SerializableAction(Id = \"{guid}\")]");
                sb.AppendLine($"{classDeclSpaces}public class Functor_{source.TypeName}_{safeGuidName}: ISerializableActionFactory");
                sb.AppendLine($"{classDeclSpaces}{{");
                sb.AppendLine($"{classContentDeclSpaces}public string Id => \"{guid}\";");
                sb.AppendLine();
                
                var n = 0;

                foreach (var lambda in lambdas)
                {
                    sb.AppendLine($"{classContentDeclSpaces}private object _action_{n} = {lambda.ToString()};");
                    sb.AppendLine();

                    n++;
                }
                sb.AppendLine($"{classContentDeclSpaces}public object GetAction(int index)");
                sb.AppendLine($"{classContentDeclSpaces}{{");
                sb.AppendLine($"{classContentSpaces}switch(index)");
                sb.AppendLine($"{classContentSpaces}{{");
                for (var i = 0; i < n; i++)
                {
                    sb.AppendLine($"{classContentSpaces_2}case {i}: return _action_{i};");
                }
                sb.AppendLine($"{classContentSpaces_2}default: throw new System.NotSupportedException($\"{guid}: {{index}}\");");
                sb.AppendLine($"{classContentSpaces}}}");
                sb.AppendLine($"{classContentDeclSpaces}}}");
                sb.AppendLine($"{classDeclSpaces}}}");
                sb.AppendLine("}");

#if DEBUG
                //FileLogger.WriteLn($"sb.ToString() = {sb.ToString()}");
#endif

                context.AddSource($"Functor_{safeGuidName}.g.cs", sb.ToString());
            }
        }

        private static List<ExpressionSyntax> GetAllLambdas(int startIndex, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var lambdas = new List<ExpressionSyntax>();

            for(var i = startIndex; i < arguments.Count; i++)
            {
                var expr = arguments[i].Expression;

                if(IsLambda(expr))
                {
                    lambdas.Add(expr);
                }
                else
                {
                    return lambdas;
                }
            }

            throw new NotImplementedException("E551B64C-940C-4AA8-B66C-D074B23BF4F7");
        }

        private static bool IsLambda(ExpressionSyntax expr)
        {
            return expr is ParenthesizedLambdaExpressionSyntax
                || expr is SimpleLambdaExpressionSyntax
                || expr is AnonymousMethodExpressionSyntax;
        }

        private static List<string> GetUsingsForNode(SyntaxNode node, SemanticModel semanticModel)
        {
            var usings = new List<string>();

            // 1. Obtain a semantic model for a specific location in the code (taking local `using` directives into account)
            //var enclosingSymbol = semanticModel.GetEnclosingSymbol(node.SpanStart);

            // Alternatively, you can collect all `using` directives from the file's syntax tree:
            var root = node.SyntaxTree.GetRoot();

            // Find all using-directive nodes in the file
            var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

            foreach (var usingDirective in usingDirectives)
            {
                // Namespace name (e.g., System.Collections.Generic)
                var usingName = usingDirective.Name?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(usingName))
                {
                    usings.Add(usingName);
                }
            }

            // You can also add namespaces that are included globally (`global using`) via `SemanticModel`.
            // But for text generation, it is usually sufficient to collect the string directives from a file.

            return usings;
        }

        private static string GetNamespaceForNode(SyntaxNode node, SemanticModel semanticModel)
        {
            // We retrieve the symbol that encloses the start of the lambda (a method, class, etc.)
            var enclosingSymbol = semanticModel.GetEnclosingSymbol(node.SpanStart);

            if (enclosingSymbol == null)
            { 
                return string.Empty; 
            }

            // We obtain the contained namespace
            var namespaceSymbol = enclosingSymbol.ContainingNamespace;

            // If it is the global namespace (no namespace), then ToDisplayString() will return an empty string or "<global namespace>".
            if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace)
            {
                return string.Empty;
            }

            // Returns the full name, for example: "MyCompany.Game.Systems"
            return namespaceSymbol.ToDisplayString();
        }
    }

    public class TargetInfo
    {
        public TargetInfo(SyntaxNode node, SemanticModel semanticModel, string typeName, bool isConstructor)
        {
            Node = node;
            SemanticModel = semanticModel;
            TypeName = typeName;
            IsConstructor = isConstructor;
        }

        public SyntaxNode Node;
        public SemanticModel SemanticModel;
        public string TypeName;
        public bool IsConstructor;
    }
}
