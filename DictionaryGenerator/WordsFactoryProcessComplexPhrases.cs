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
using System.Text.RegularExpressions;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessComplexPhrases(List<string> totalNamesList)
        {
            ProcessAllComplexPronouns();
            ProcessAllComplexPrepositions();
            ProcessAllComplexConjunctions();
            ProcessAllComplexNumerals();

            //foreach (var rootName in totalNamesList)
            //{
            //    ProcessComplexRootWordName(rootName);
            //}
        }

        private void ProcessComplexRootWordName(string rootWord)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessRootWordName rootWord = {rootWord}");
#endif

            var rez = Regex.Match(rootWord, @"\(\w+\)");
            var rezStr = rez.ToString();

            if (!string.IsNullOrWhiteSpace(rezStr))
            {
                rootWord = rootWord.Replace(rezStr, string.Empty);
            }

            if (mRootNounDict.ContainsKey(rootWord))
            {
                ProcessComplexNoun(rootWord);
            }

            if (mRootVerbsDict.ContainsKey(rootWord))
            {
                ProcessComplexVerb(rootWord);
            }

            if (mRootAdjsDict.ContainsKey(rootWord))
            {
                ProcessComplexAdj(rootWord);
            }

            if (mRootAdvsDict.ContainsKey(rootWord))
            {
                ProcessComplexAdv(rootWord);
            }
        }

        private void ProcessComplexNoun(string rootWord)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexNoun rootWord = {rootWord}");
#endif

            if (IsNumeral(rootWord))
            {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info("ProcessComplexNoun IsNumeral(rootWord) return; !!!!!");
#endif
                return;
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
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun logicalMeaning.Count = {logicalMeaning.Count}");

            //foreach (var className in logicalMeaning)
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun className = {className}");
            //}
#endif

            var isCountable = logicalMeaning.Contains("object") || logicalMeaning.Contains("causal_agent");

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun isCountable = {isCountable}");
#endif

            mTotalCount++;

            AddGrammaticalWordFrame(rootWord, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var splitModel = mWordSplitter.Split(rootWord, GrammaticalPartOfSpeech.Noun);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexNoun splitModel = {splitModel}");
#endif

            if(splitModel.IndexOfTargetWord > -1)
            {
                var targetWord = splitModel.TargetWord;

                var multipleForms = mNounAntiStemmer.GetPluralForm(targetWord);

                //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexNoun multipleForms = {multipleForms}");

                multipleForms = mWordSplitter.Join(multipleForms, splitModel);

                //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexNoun after multipleForms = {multipleForms}");
            }
        }

        private bool DetectWordInComplexPhrase(string word, string phrase)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"DetectWordInComplexPhrase word = {word} phrase = {phrase}");
#endif

            if(phrase.StartsWith($"{word}_") || phrase.Contains($"_{word}_") || phrase.Contains($"_{word}"))
            {
                return true;
            }

            return false;
        }

        private void ProcessComplexVerb(string rootWord)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexVerb rootWord = {rootWord}");
#endif

            if(DetectWordInComplexPhrase("be", rootWord))
            {
#if DEBUG
                NLog.LogManager.GetCurrentClassLogger().Info("ProcessComplexVerb Detect!!!!!");
#endif
            }
        }

        private void ProcessComplexAdj(string rootWord)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexAdj rootWord = {rootWord}");
#endif
        }

        private void ProcessComplexAdv(string rootWord)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"ProcessComplexAdv rootWord = {rootWord}");
#endif

            if (IsNumeral(rootWord))
            {
#if DEBUG
                NLog.LogManager.GetCurrentClassLogger().Info("ProcessComplexAdv IsNumeral(rootWord) return; !!!!!");
#endif
                return;
            }


        }
    }
}
