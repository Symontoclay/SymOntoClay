using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class SplitWordModel : IObjectToString
    {
        public string InitialWord { get; set; }
        public List<string> SplittedWords { get; set; }
        public string TargetWord { get; set; }
        public int IndexOfTargetWord { get; set; } = -1;

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(InitialWord)} = {InitialWord}");
            if (SplittedWords == null)
            {
                sb.AppendLine($"{spaces}{nameof(SplittedWords)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(SplittedWords)}");
                foreach (var splittedWord in SplittedWords)
                {
                    sb.AppendLine($"{nextNSpaces}{splittedWord}");
                }
                sb.AppendLine($"{spaces}End {nameof(SplittedWords)}");
            }
            sb.AppendLine($"{spaces}{nameof(TargetWord)} = {TargetWord}");
            sb.AppendLine($"{spaces}{nameof(IndexOfTargetWord)} = {IndexOfTargetWord}");
            return sb.ToString();
        }
    }
}
