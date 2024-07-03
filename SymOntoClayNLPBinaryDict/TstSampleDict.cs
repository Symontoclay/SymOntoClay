using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;

namespace SymOntoClay.NLP.BinaryDict
{
    public class TstSampleDict: IWordsDict
    {
        public TstSampleDict()
        {
            _wordFramesDict = new Dictionary<string, List<BaseGrammaticalWordFrame>>();

            List<BaseGrammaticalWordFrame> framesList = null;

            framesList = new List<BaseGrammaticalWordFrame>();

            _wordFramesDict["dog"] = framesList;

            framesList.Add(new ArticleGrammaticalWordFrame() { });
        }

        private Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesDict;

        public IList<BaseGrammaticalWordFrame> GetWordFramesByWord(string word)
        {
            if(_wordFramesDict.ContainsKey(word))
            {
                return _wordFramesDict[word];
            }

            return null;
        }

        public IList<BaseGrammaticalWordFrame> GetWordFramesByRootWord(string word)
        {
            throw new NotImplementedException("9FB537A5-C34F-46B5-9F32-6C932C3F8841");
        }
    }
}
