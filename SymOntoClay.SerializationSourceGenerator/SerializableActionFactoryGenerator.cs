using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
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
                FileLogger.WriteLn($"Target element processed: {source.TypeName} (Constructor: {source.IsConstructor})");
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

        private static void ExecuteGeneration(SourceProductionContext context, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
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
