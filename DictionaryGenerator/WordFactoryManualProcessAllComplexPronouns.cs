using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllComplexPronouns()
        {
            var wordName = "no_one";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "people"
                }
            });
        }
    }
}
