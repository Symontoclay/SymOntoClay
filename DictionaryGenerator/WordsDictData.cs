using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryGenerator
{
    public class WordsDictData : IObjectToString
    {
        public IDictionary<string, WordFrame> WordsDict { get; set; }
        //public IList<string> NamesList { get; set; }

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
            if (WordsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(WordsDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(WordsDict)}");
                foreach (var wordsDictKVPItem in WordsDict)
                {
                    sb.AppendLine($"{nextNSpaces}word = {wordsDictKVPItem.Key}");
                    sb.Append(wordsDictKVPItem.Value.ToString(nextN));
                }
                sb.AppendLine($"{spaces}End {nameof(WordsDict)}");
            }

            //if (NamesList == null)
            //{
            //    sb.AppendLine($"{spaces}{nameof(NamesList)} = null");
            //}
            //else
            //{
            //    sb.AppendLine($"{spaces}Begin {nameof(NamesList)}");
            //    foreach (var name in NamesList)
            //    {
            //        sb.AppendLine($"{nextNSpaces}{nameof(name)} = {name}");
            //    }
            //    sb.AppendLine($"{spaces}End {nameof(NamesList)}");
            //}
            return sb.ToString();
        }
    }
}
