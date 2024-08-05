using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class TargetClassSearcherContext
    {
        public string FilePath { get; set; }
        public string Namespace { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(FilePath)} = {FilePath}");
            sb.AppendLine($"{nameof(Namespace)} = {Namespace}");
            return sb.ToString();
        }
    }
}
