using Microsoft.CodeAnalysis;

namespace SymOntoClay.SourceGenerator
{
    [Generator]
    public class SerializationSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var searcher = new TargetClassSearcher(syntaxTrees);

            var items = searcher.Run(Constants.SerializationAttributeName);

            var plainObjectsRegistry = new PlainObjectsRegistry();

            var plainObjectsSearcher = new PlainObjectsSearcher(context);

            foreach (var item in items)
            {
                plainObjectsSearcher.Run(item, plainObjectsRegistry);
            }

            var socSerializationGeneration = new SocSerializationGeneration(context);

            foreach (var item in items)
            {
                socSerializationGeneration.Run(item, plainObjectsRegistry);
            }
        }
        
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
