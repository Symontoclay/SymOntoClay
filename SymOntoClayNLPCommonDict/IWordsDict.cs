using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public interface IWordsDict
    {
        IList<BaseGrammaticalWordFrame> GetWordFramesByWord(string word);
        IList<BaseGrammaticalWordFrame> GetWordFramesByRootWord(string word);
    }
}
