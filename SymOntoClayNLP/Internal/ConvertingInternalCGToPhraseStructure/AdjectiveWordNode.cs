using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class AdjectiveWordNode
    {
        public AdjectiveWordNode(string word, IEntityLogger logger, IWordsDict wordsDict)
        {
            _logger = logger;
            _wordsDict = wordsDict;
            _word = word;
        }

        private readonly IEntityLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly string _word;

        public Word GetWord()
        {
#if DEBUG
            //_logger.Log($"_word = '{_word}'");
#endif

            var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

#if DEBUG
            //_logger.Log($"wordFramesList = {wordFramesList.WriteListToString()}");
#endif

            var adjectivesList = wordFramesList.Where(p => p.IsAdjective).Select(p => p.AsAdjective).ToList();

            if(adjectivesList.Any())
            {
#if DEBUG
                //_logger.Log($"adjectivesList = {adjectivesList.WriteListToString()}");
#endif

                var result = new Word();

                result.Content = _word;

                result.WordFrame = adjectivesList.Single();

#if DEBUG
                //_logger.Log($"result = {result}");
#endif

                return result;
            }

            throw new NotImplementedException();
        }
    }
}
