using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class IrregularVerbItem : IObjectToString
    {
        public string RootWord { get; set; }
        public string PastForm { get; set; }
        public string ParticleForm { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(RootWord)} = {RootWord}");
            sb.AppendLine($"{spaces}{nameof(PastForm)} = {PastForm}");
            sb.AppendLine($"{spaces}{nameof(ParticleForm)} = {ParticleForm}");
            return sb.ToString();
        }
    }
}
