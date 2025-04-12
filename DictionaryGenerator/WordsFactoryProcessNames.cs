/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
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



#if DEBUG
            _logger.Info($"ProcessName logicalMeaning.Count = {logicalMeaning.Count}");

            foreach (var className in logicalMeaning)
            {
                _logger.Info($"ProcessName className = {className}");
            }
#endif

            AddGrammaticalWordFrameOfName(rootWord, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var possessiveSingular = mNounAntiStemmer.GetPossesiveForSingular(rootWord);

#if DEBUG
            _logger.Info($"ProcessName possessiveSingular = {possessiveSingular}");
#endif

            AddGrammaticalWordFrame(possessiveSingular, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList(),
                IsPossessive = true
            });

            var multipleForms = mNounAntiStemmer.GetPluralForm(rootWord);

            _logger.Info($"ProcessName multipleForms = {multipleForms}");

            AddGrammaticalWordFrameOfName(multipleForms, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = true,
                IsName = true,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var possessivePlural = mNounAntiStemmer.GetPossesiveForPlural(multipleForms);

#if DEBUG
            _logger.Info($"ProcessName possessivePlural = {possessivePlural}");
#endif

            AddGrammaticalWordFrame(possessivePlural, new NounGrammaticalWordFrame()
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
