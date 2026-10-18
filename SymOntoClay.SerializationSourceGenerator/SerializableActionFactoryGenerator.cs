using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.SerializationSourceGenerator
{
    [Generator]
    public class SerializableActionFactoryGenerator : IIncrementalGenerator
    {
        private static readonly HashSet<string> TargetTypes = new()
        {
            "LoggedAltFunctorWithoutResult"
        };

        private List<string> _targetConstructors = new List<string>()
        {
            "LoggedAltFunctorWithoutResult"
        };

        private List<(string FirstIdenfifier, string SecondIdenfifier)> _targetInvocations = new List<(string FirstIdenfifier, string SecondIdenfifier)>()
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
                if (!methodSymbol.IsStatic || methodSymbol.Name != "Run") { return null; }

                var typeName = methodSymbol.ContainingType.Name;
                if (!TargetTypes.Contains(typeName)) { return null; }

                return new TargetInfo(invocation, semanticModel, typeName, isConstructor: false);
            }
            else if (node is ObjectCreationExpressionSyntax creation)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(creation);
                if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol) { return null; }

                // We verify that this is indeed a construction set
                if (constructorSymbol.MethodKind != MethodKind.Constructor) { return null; }

                var typeName = constructorSymbol.ContainingType.Name;
                if (!TargetTypes.Contains(typeName)) return null;

                // Note: For ObjectCreationExpressionSyntax, the arguments are located in creation.ArgumentList.
                return new TargetInfo(creation, semanticModel, typeName, isConstructor: true);
            }

            return null;
        }

        private static void ExecuteGeneration(SourceProductionContext context, TargetInfo source)
        {
#if DEBUG
            FileLogger.WriteLn($"Target element processed: {source.TypeName} (Constructor: {source.IsConstructor})");
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

                // Fourth parameter (index 3 — lambda)
                var lambdaExpr = argList.Arguments[3].Expression;
                var lambdaText = lambdaExpr.ToString();

#if DEBUG
                FileLogger.WriteLn($"guid = {guid}");
                FileLogger.WriteLn($"lambdaText = {lambdaText}");
#endif

                var safeGuidName = guid.Replace("-", "");

                var usings = GetUsingsForNode(source.Node, source.SemanticModel);

                var identationStep = 4;
                var baseIdentation = 0;

                var sb = new StringBuilder();
                sb.AppendLine($"// Auto-generated for GUID: {guid}");
                foreach (var u in usings) 
                {
#if DEBUG
                    FileLogger.WriteLn($"u = {u}");
#endif
                    sb.AppendLine($"using {u};");
                }
                sb.AppendLine("namespace SymOntoClay.GeneratedFunctors");
                sb.AppendLine("{");
                sb.AppendLine("}");

#if DEBUG
                FileLogger.WriteLn($"sb.ToString() = {sb.ToString()}");
#endif

                context.AddSource($"Functor_{safeGuidName}.g.cs", sb.ToString());
            }

            //var arguments = invocation.ArgumentList.Arguments;

            //// The second parameter (index 1) is a string literal (GUID)
            //var guidValue = semanticModel.GetConstantValue(arguments[1].Expression).Value?.ToString() ?? "UnknownGuid";

            //// The fourth parameter (index 3) is a lambda expression.
            //var lambdaText = arguments[3].Expression.ToString();

            //var safeGuidName = guidValue.Replace("-", "");
            //var generatedCode = $@"
            //// Auto-generated for GUID: {guidValue}
            //namespace GeneratedFunctors
            //{{
            //    public static class Handler_{safeGuidName}
            //    {{
            //        public static void Execute()
            //        {{
            //            // Logic of the extracted lambda:
            //            // {lambdaText}
            //        }}
            //    }}
            //}}";

            //context.AddSource($"Functor_{safeGuidName}.g.cs", generatedCode);
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
                string usingName = usingDirective.Name?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(usingName))
                {
                    usings.Add(usingName);
                }
            }

            // You can also add namespaces that are included globally (`global using`) via `SemanticModel`.
            // But for text generation, it is usually sufficient to collect the string directives from a file.

            return usings;
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
