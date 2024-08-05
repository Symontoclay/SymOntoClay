using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class TargetCompilationUnit
    {
        public string FilePath { get; set; }

        public List<TargetClassItem> ClassItems { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(FilePath)} = '{FilePath}'");
            sb.AppendLine($"{nameof(ClassItems)}.{nameof(ClassItems.Count)} = '{ClassItems.Count}'");
            return sb.ToString();
        }
    }
}
