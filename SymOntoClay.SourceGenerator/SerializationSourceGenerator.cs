using Microsoft.CodeAnalysis;
using System;

namespace SymOntoClay.SourceGenerator
{
    [Generator]
    public class SerializationSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            try
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
            catch(Exception e)
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
