using Microsoft.CodeAnalysis;

namespace SymOntoClay.SourceGenerator
{
    [Generator]
    public class SerializationSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            ProcessSocSerialization(context);
            ProcessSerializableActions(context);
        }

        private void ProcessSocSerialization(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var searcher = new TargetClassSearcher(syntaxTrees);

            var items = searcher.Run("SocSerialization");

            var socSerializationGeneration = new SocSerializationGeneration(context);

            foreach (var item in items)
            {
                socSerializationGeneration.Run(item);
            }
        }

        private void ProcessSerializableActions(GeneratorExecutionContext context)
        {
            var codeChunksSearcher = new CodeChunksSearcher(context);

            var codeChunkCodeGenerator = new CodeChunkCodeGenerator(context);

            var items = codeChunksSearcher.Run();

            foreach (var item in items)
            {
                codeChunkCodeGenerator.Run(item);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
