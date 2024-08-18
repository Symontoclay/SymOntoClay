using Microsoft.CodeAnalysis;

namespace SymOntoClay.SourceGenerator
{
    [Generator]
    public class CodeChunksSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
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
