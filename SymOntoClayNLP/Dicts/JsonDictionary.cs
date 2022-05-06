using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.CommonDict.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Dicts
{
    public class JsonDictionary : IWordsDict
    {
        public JsonDictionary(string fileName)
        {
            var serializer = new WordsDictJSONSerializationEngine();

            _wordFramesDict = serializer.LoadFromFile(fileName);
        }

        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesDict;

        public IList<BaseGrammaticalWordFrame> GetWordFrames(string word)
        {
            if (_wordFramesDict.ContainsKey(word))
            {
                return _wordFramesDict[word];
            }

            return null;
        }
    }
}
