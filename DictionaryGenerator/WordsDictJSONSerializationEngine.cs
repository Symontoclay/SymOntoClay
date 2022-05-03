using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryGenerator
{
    public class WordsDictJSONSerializationEngine
    {
        private DataContractJsonSerializer mJSONSerializer = new DataContractJsonSerializer(typeof(WordsDictData), new List<Type> {
                typeof(PronounGrammaticalWordFrame),
                typeof(AdjectiveGrammaticalWordFrame),
                typeof(AdverbGrammaticalWordFrame),
                typeof(ArticleGrammaticalWordFrame),
                typeof(ConjunctionGrammaticalWordFrame),
                typeof(NounGrammaticalWordFrame),
                typeof(NumeralGrammaticalWordFrame),
                typeof(PostpositionGrammaticalWordFrame),
                typeof(PrepositionGrammaticalWordFrame),
                typeof(PronounGrammaticalWordFrame),
                typeof(VerbGrammaticalWordFrame)
    });

        public void SaveToFile(WordsDictData item, string fileName)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fs = File.OpenWrite(fileName))
            {
                mJSONSerializer.WriteObject(fs, item);
                fs.Flush();
            }
        }

        public WordsDictData LoadFromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using (var fs = File.OpenRead(fileName))
            {
                return (WordsDictData)mJSONSerializer.ReadObject(fs);
            }
        }
    }
}
