﻿using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System.Text.RegularExpressions;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessNames(List<string> totalNamesList)
        {
            foreach (var rootName in totalNamesList)
            {
                ProcessName(rootName);
            }
        }

        private void ProcessName(string rootWord)
        {
#if DEBUG
            //if (rootWord.ToLower() == "britain")
            //{
            //    LogInstance.Log($"rootWord = {rootWord}");
            //    throw new NotImplementedException();
            //}
            //LogInstance.Log($"rootWord = {rootWord}");
#endif

            if (mTargetWordsList != null)
            {
                if (!mTargetWordsList.Contains(rootWord))
                {
                    return;
                }
            }

#if DEBUG
            _logger.Info($"rootWord = {rootWord}");
#endif

            var rez = Regex.Match(rootWord, @"\(\w+\)");
            var rezStr = rez.ToString();

            if (!string.IsNullOrWhiteSpace(rezStr))
            {
                rootWord = rootWord.Replace(rezStr, string.Empty);
            }

            var logicalMeaning = new List<string>() { "entity" };

            //List<string> logicalMeaning = null;

            //if (mNounClassesDict.ContainsKey(rootWord))
            //{
            //    logicalMeaning = mNounClassesDict[rootWord];
            //}
            //else
            //{
            //    logicalMeaning = new List<string>();
            //}

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessName logicalMeaning.Count = {logicalMeaning.Count}");

            foreach (var className in logicalMeaning)
            {
                NLog.LogManager.GetCurrentClassLogger().Info($"ProcessName className = {className}");
            }
#endif

            AddGrammaticalWordFrameOfName(rootWord, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var possesiveSingular = mNounAntiStemmer.GetPossesiveForSingular(rootWord);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessName possesiveSingular = {possesiveSingular}");
#endif

            AddGrammaticalWordFrame(possesiveSingular, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList(),
                IsPossessive = true
            });

            var multipleForms = mNounAntiStemmer.GetPluralForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessName multipleForms = {multipleForms}");

            AddGrammaticalWordFrameOfName(multipleForms, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var possesivePlural = mNounAntiStemmer.GetPossesiveForPlural(multipleForms);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessName possesivePlural = {possesivePlural}");
#endif

            AddGrammaticalWordFrame(possesivePlural, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = true,
                LogicalMeaning = logicalMeaning.ToList(),
                IsName = true,
                IsPossessive = true
            });
        }

        private WordFrame GetWordFrameOfName(string word)
        {
            if (mWordsDictData.WordsDict.ContainsKey(word))
            {
                return mWordsDictData.WordsDict[word];
            }

            var wordFrame = new WordFrame();
            mWordsDictDataOfName.WordsDict[word] = wordFrame;
            wordFrame.Word = word;
            wordFrame.GrammaticalWordFrames = new List<BaseGrammaticalWordFrame>();

            return wordFrame;
        }

        private void AddGrammaticalWordFrameOfName(string word, BaseGrammaticalWordFrame grammaticalWordFrame)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentNullException(nameof(word));
            }

            if (grammaticalWordFrame == null)
            {
                throw new ArgumentNullException(nameof(grammaticalWordFrame));
            }

            var wordFrame = GetWordFrameOfName(word);

            if(grammaticalWordFrame.LogicalMeaning.IsNullOrEmpty())
            {
                grammaticalWordFrame.LogicalMeaning = new List<string>() { "entity" };
            }

            grammaticalWordFrame.FullLogicalMeaning = grammaticalWordFrame.LogicalMeaning.ToList();

            if (string.IsNullOrWhiteSpace(grammaticalWordFrame.RootWord))
            {
                grammaticalWordFrame.RootWord = word;
            }

            wordFrame.GrammaticalWordFrames.Add(grammaticalWordFrame);
        }
    }
}