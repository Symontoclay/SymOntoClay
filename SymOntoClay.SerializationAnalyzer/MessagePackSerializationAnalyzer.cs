using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SerializationAnalyzer
{
    [Generator]
    public class MessagePackSerializationAnalyzer : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var compilation = context.Compilation;

                foreach (var tree in compilation.SyntaxTrees)
                {
#if DEBUG
                    //FileLogger.WriteLn($"tree.FilePath = {tree.FilePath}");
#endif

                    var semanticModel = compilation.GetSemanticModel(tree);
                    var classes = tree.GetRoot()
                        .DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(c => c.AttributeLists
                            .Any(a => a.ToString().Contains("MessagePackObject")));

                    foreach (var cls in classes)
                    {
#if DEBUG
                        FileLogger.WriteLn($"tree.FilePath = {cls.Identifier.Text}");
#endif
                    }
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
    }
}
