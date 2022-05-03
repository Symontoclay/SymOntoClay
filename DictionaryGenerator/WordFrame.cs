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
        public IList<BaseGrammaticalWordFrame> GrammaticalWordFrames { get; set; }

        public WordFrame Fork()
        {
            var result = new WordFrame();
            result.Word = Word;
            if (GrammaticalWordFrames == null)
            {
                result.GrammaticalWordFrames = null;
            }
            else
            {
                result.GrammaticalWordFrames = GrammaticalWordFrames.Select(p => p.Fork()).Distinct(new ComparerOfBaseGrammaticalWordFrame()).ToList();
            }
            return result;
        }

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
            if (GrammaticalWordFrames == null)
            {
                sb.AppendLine($"{spaces}{nameof(GrammaticalWordFrames)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(GrammaticalWordFrames)}");
                foreach (var grammaticalWordFrame in GrammaticalWordFrames)
                {
#if DEBUG
                    //LogInstance.Log($"grammaticalWordFrame == null = {grammaticalWordFrame == null}");
#endif

                    sb.Append(grammaticalWordFrame.ToString(nextN));
                }
                sb.AppendLine($"{spaces}End {nameof(GrammaticalWordFrames)}");
            }
            return sb.ToString();
        }
    }
}
