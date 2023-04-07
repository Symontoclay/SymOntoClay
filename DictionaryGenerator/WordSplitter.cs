/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class WordSplitter
    {
        public WordSplitter(Dictionary<string, List<RootNounSourceWordItem>> rootNounDict, Dictionary<string, List<RootVerbSourceWordItem>> rootVerbsDict,
            Dictionary<string, List<RootAdjSourceWordItem>> rootAdjsDict, Dictionary<string, List<RootAdvSourceWordItem>> rootAdvsDict)
        {
            mRootNounDict = rootNounDict;
            mRootVerbsDict = rootVerbsDict;
            mRootAdjsDict = rootAdjsDict;
            mRootAdvsDict = rootAdvsDict;
        }

        private Dictionary<string, List<RootNounSourceWordItem>> mRootNounDict;
        private Dictionary<string, List<RootVerbSourceWordItem>> mRootVerbsDict;
        private Dictionary<string, List<RootAdjSourceWordItem>> mRootAdjsDict;
        private Dictionary<string, List<RootAdvSourceWordItem>> mRootAdvsDict;

        public SplitWordModel Split(string word, GrammaticalPartOfSpeech targetPartOfSpeech)
        {
            var result = new SplitWordModel();
            result.InitialWord = word;

            var wordsList = word.Split('_').ToList();

            result.SplittedWords = wordsList;

            var n = 0;

            var litsOfTargetWords = new List<KeyValuePair<string, int>>();

            foreach(var splittedWord in wordsList)
            {
                var partOfSpeechesList = GetPartOfSpeechesList(splittedWord);

#if DEBUG

#endif

                if (partOfSpeechesList.Contains(targetPartOfSpeech))
                {
                    litsOfTargetWords.Add(new KeyValuePair<string, int>(splittedWord, n));
                }

                n++;
            }

            if(litsOfTargetWords.Any())
            {
                var targetItem = litsOfTargetWords.Last();

                result.TargetWord = targetItem.Key;
                result.IndexOfTargetWord = targetItem.Value;
            }

            return result;
        }

        private List<GrammaticalPartOfSpeech> GetPartOfSpeechesList(string word)
        {
            var result = new List<GrammaticalPartOfSpeech>();

            if(mRootNounDict.ContainsKey(word))
            {
                result.Add(GrammaticalPartOfSpeech.Noun);
            }

            if (mRootVerbsDict.ContainsKey(word))
            {
                result.Add(GrammaticalPartOfSpeech.Verb);
            }

            if (mRootAdjsDict.ContainsKey(word))
            {
                result.Add(GrammaticalPartOfSpeech.Adjective);
            }

            if (mRootAdvsDict.ContainsKey(word))
            {
                result.Add(GrammaticalPartOfSpeech.Adverb);
            }

            return result;
        }

        public string Join(string word, SplitWordModel splitModel)
        {
            var wordsList = splitModel.SplittedWords.ToList();
            wordsList[splitModel.IndexOfTargetWord] = word;
            return string.Join("_", wordsList);
        }
    }
}
