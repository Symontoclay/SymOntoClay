using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SymOntoClay.SourceGenerator;
using System.Linq;

namespace SymOntoClay.SourceGenerator
{
    public class PlainObjectsSearcher
    {
        public PlainObjectsSearcher(GeneratorExecutionContext context)
        {
            _context = context;
        }

        private readonly GeneratorExecutionContext _context;

        public void Run(TargetCompilationUnit targetCompilationUnit, PlainObjectsRegistry plainObjectsRegistry)
        {
            foreach (var targetClassItem in targetCompilationUnit.ClassItems)
            {
                ProcessTargetClassItem(targetClassItem, plainObjectsRegistry);
            }
        }

        private void ProcessTargetClassItem(TargetClassItem targetClassItem, PlainObjectsRegistry plainObjectsRegistry)
        {
            var className = targetClassItem.SyntaxNode.Identifier.Text;

            var classFullName = $"{targetClassItem.Namespace}.{className}";

            var plainObjectClassName = GeneratorsHelper.GetPlainObjectClassIdentifier(targetClassItem.SyntaxNode);

            var plainObjectNamespace = GeneratorsHelper.GetPlainObjectNamespace(targetClassItem.Namespace);

            var plainObjectClassFullName = $"{plainObjectNamespace}.{plainObjectClassName}";

            var genericParamsCount = GetGenericParamsCount(targetClassItem);

            plainObjectsRegistry.Add(classFullName, genericParamsCount, plainObjectClassFullName);
        }

        private int GetGenericParamsCount(TargetClassItem targetClassItem)
        {
            var typeParameterList = targetClassItem.SyntaxNode?.ChildNodes().OfType<TypeParameterListSyntax>().FirstOrDefault();

            if (typeParameterList == null)
            {
                return 0;
            }

            return typeParameterList.Parameters.Count;
        }
    }
}
