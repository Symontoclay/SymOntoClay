using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.CommonDict.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Dicts
{
    public class JsonDictionary : IWordsDict
    {
        public JsonDictionary(string fileName)
        {
            var serializer = new WordsDictJSONSerializationEngine();

            _wordFramesByWordDict = serializer.LoadFromFile(fileName);
            var tmpList = _wordFramesByWordDict.SelectMany(p => p.Value.ToList());
            _wordFramesByByRootWordDict = tmpList.GroupBy(p => p.RootWord).ToDictionary(p => p.Key, p => p.ToList());
        }

        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesByWordDict;
        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesByByRootWordDict;

        /// <inheritdoc/>
        public IList<BaseGrammaticalWordFrame> GetWordFramesByWord(string word)
        {
            if (_wordFramesByWordDict.ContainsKey(word))
            {
                return _wordFramesByWordDict[word];
            }

            return null;
        }

        /// <inheritdoc/>
        public IList<BaseGrammaticalWordFrame> GetWordFramesByRootWord(string word)
        {
            if (_wordFramesByByRootWordDict.ContainsKey(word))
            {
                return _wordFramesByByRootWordDict[word];
            }

            return null;
        }
    }
}
