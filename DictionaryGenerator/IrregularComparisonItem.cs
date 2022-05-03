using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class IrregularComparisonItem : IObjectToString
    {
        public string BaseWord { get; set; }
        public string ComparativeForm { get; set; }
        public string SuperlativeForm { get; set; }

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
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(BaseWord)} = {BaseWord}");
            sb.AppendLine($"{spaces}{nameof(ComparativeForm)} = {ComparativeForm}");
            sb.AppendLine($"{spaces}{nameof(SuperlativeForm)} = {SuperlativeForm}");
            return sb.ToString();
        }
    }
}
