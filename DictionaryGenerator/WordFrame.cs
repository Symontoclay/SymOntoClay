using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryGenerator
{
    public class WordFrame : IObjectToString
    {
        public string Word { get; set; }
        public List<BaseGrammaticalWordFrame> GrammaticalWordFrames { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(Word)} = {Word}");
            sb.PrintObjListProp(n, nameof(GrammaticalWordFrames), GrammaticalWordFrames);
            return sb.ToString();
        }
    }
}
