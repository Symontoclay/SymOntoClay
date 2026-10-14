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
            // 1. Syntax-level quick filter
            var methodCalls = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsCandidateNode(node),
                    transform: static (ctx, _) => (
                        Invocation: (InvocationExpressionSyntax)ctx.Node,
                        SemanticModel: ctx.SemanticModel
                    )
                )
                .Where(static data => IsTargetMethod(data.Invocation, data.SemanticModel));

            // 2. Code generation
            context.RegisterSourceOutput(methodCalls, static (spc, source) =>
            {
                ExecuteGeneration(spc, source.Invocation, source.SemanticModel);
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

        private static bool IsTargetMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            { 
                return false; 
            }

            var methodName = memberAccess.Name.Identifier.Text;

            return false;//tmp

            //if (!TargetMethods.Contains(methodName)) 
            //{ 
            //    return false; 
            //}

            //// Obtaining information about the symbol via the semantic model.
            //var symbolInfo = semanticModel.GetSymbolInfo(invocation);

            //if (symbolInfo.Symbol is not IMethodSymbol methodSymbol) 
            //{ 
            //    return false;
            //}

            //var containingTypeName = methodSymbol.ContainingType.Name;

            //// We check whether the class is in our list of target classes.
            //return TargetTypes.Contains(containingTypeName);
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
}
