using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class TargetCodeChunksCompilationUnit
    {
        public string FilePath { get; set; }

        public List<string> Usings { get; set; }
        public List<CodeChunkItem> CodeChunkItems { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(FilePath)} = '{FilePath}'");
            sb.AppendLine($"{nameof(Usings)}.{nameof(Usings.Count)} = {Usings.Count}");
            sb.AppendLine($"{nameof(CodeChunkItems)}.{nameof(CodeChunkItems.Count)} = {CodeChunkItems.Count}");
            return sb.ToString();
        }
    }
}
