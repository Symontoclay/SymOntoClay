using Microsoft.CodeAnalysis;
using System;

namespace SymOntoClay.SourceGenerator
{
    [Generator]
    public class CodeChunksSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var codeChunksSearcher = new CodeChunksSearcher(context);

                var codeChunkCodeGenerator = new CodeChunkCodeGenerator(context);

                var items = codeChunksSearcher.Run();

                foreach (var item in items)
                {
                    codeChunkCodeGenerator.Run(item);
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

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
