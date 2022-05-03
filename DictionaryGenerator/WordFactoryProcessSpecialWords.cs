using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void AddUsualSpecialWords(ref List<string> usualWordsList)
        {
            var word = "ought";
            AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "advertisement";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
			
			word = "almost";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootAdjsDict(word);
			AddSpecialWordToRootAdvsDict(word);
			
			word = "beseech";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "bestride";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "cloth";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			AddSpecialWordToRootAdjsDict(word);
			
			word = "cruel";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootAdjsDict(word);
			
			word = "existence";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
			
			word = "favour";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "foresee";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "foretell";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "forsake";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "forswear";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "gainsay";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "interweave";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "knowledge";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
			
			word = "manager";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootNounDict(word);

			word = "mishear";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "mishit";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "mislay";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "misunderstand";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "mix";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "mixed";
			AddSpecialWordToUsualWords(word, ref usualWordsList);			
			AddSpecialWordToRootAdjsDict(word);
			
			word = "overeat";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "overfly";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "overhear";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);
			
			word = "plough";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "quit";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			AddSpecialWordToRootAdjsDict(word);
			
			word = "redo";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);

			word = "repay";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootVerbsDict(word);

			word = "resit";
			AddSpecialWordToUsualWords(word, ref usualWordsList);	
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "retell";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "slay";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
			AddSpecialWordToRootNounDict(word);
            AddSpecialWordToRootVerbsDict(word);
			
			word = "trousers";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootNounDict(word);

			word = "undergo";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "waylay";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "weep";
			AddSpecialWordToUsualWords(word, ref usualWordsList);
            AddSpecialWordToRootVerbsDict(word);

			word = "withstand";
			AddSpecialWordToUsualWords(word, ref usualWordsList);	
            AddSpecialWordToRootVerbsDict(word);
        }

        private void ProcessSpecialWords()
        {
        }
    }
}
