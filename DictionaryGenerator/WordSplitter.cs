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
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Split word = {word} targetPartOfSpeech = {targetPartOfSpeech}");
#endif

            var result = new SplitWordModel();
            result.InitialWord = word;

            var wordsList = word.Split('_').ToList();

            result.SplittedWords = wordsList;

            var n = 0;

            var litsOfTargetWords = new List<KeyValuePair<string, int>>();

            foreach(var splittedWord in wordsList)
            {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Split splittedWord = {splittedWord} n = {n}");
#endif

                var partOfSpeechesList = GetPartOfSpeechesList(splittedWord);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun partOfSpeechesList.Count = {partOfSpeechesList.Count}");

                //foreach (var partOfSpeech in partOfSpeechesList)
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun partOfSpeech = {partOfSpeech}");
                //}
#endif

                if (partOfSpeechesList.Contains(targetPartOfSpeech))
                {
#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info($"Split !!!!!!!!!!!!! splittedWord = {splittedWord} n = {n}");
#endif

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
