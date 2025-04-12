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

using SymOntoClay.NLP.CommonDict;
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

        }

        private void ProcessComplexRootWordName(string rootWord)
        {
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
            if (IsNumeral(rootWord))
            {
                return;
            }

            var logicalMeaning = new List<string>() { "entity" };



#if DEBUG

#endif

            var isCountable = logicalMeaning.Contains("object") || logicalMeaning.Contains("causal_agent");

            mTotalCount++;

            AddGrammaticalWordFrame(rootWord, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var splitModel = mWordSplitter.Split(rootWord, GrammaticalPartOfSpeech.Noun);

            if(splitModel.IndexOfTargetWord > -1)
            {
                var targetWord = splitModel.TargetWord;

                var multipleForms = mNounAntiStemmer.GetPluralForm(targetWord);


                multipleForms = mWordSplitter.Join(multipleForms, splitModel);

            }
        }

        private bool DetectWordInComplexPhrase(string word, string phrase)
        {
#if DEBUG
            _logger.Info($"DetectWordInComplexPhrase word = {word} phrase = {phrase}");
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
            _logger.Info($"ProcessComplexVerb rootWord = {rootWord}");
#endif

            if(DetectWordInComplexPhrase("be", rootWord))
            {
#if DEBUG
                _logger.Info("ProcessComplexVerb Detect!!!!!");
#endif
            }
        }

        private void ProcessComplexAdj(string rootWord)
        {
        }

        private void ProcessComplexAdv(string rootWord)
        {
            if (IsNumeral(rootWord))
            {
#if DEBUG
                _logger.Info("ProcessComplexAdv IsNumeral(rootWord) return; !!!!!");
#endif
                return;
            }


        }
    }
}
