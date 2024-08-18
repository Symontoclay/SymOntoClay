using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SourceGenerator
{
    public class CodeChunkItem
    {
        public string Identifier { get; set; }
        public string Namespace { get; set; }
        public List<ParenthesizedLambdaExpressionSyntax> Lambdas { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(Identifier)} = '{Identifier}'");
            sb.AppendLine($"{nameof(Namespace)} = '{Namespace}'");
            sb.AppendLine($"{nameof(Lambdas)}.{nameof(Lambdas.Count)} = {Lambdas.Count}");
            //sb.AppendLine($"{nameof(Identifier)} = '{Identifier}'");
            return sb.ToString();
        }
    }
}
