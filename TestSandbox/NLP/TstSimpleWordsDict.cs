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
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestSandbox.NLP
{
    public class TstSimpleWordsDict: IWordsDict
    {
        public TstSimpleWordsDict()
        {
            DefineCommonClasses();
            DefineWords();

            CalculateFullMeaningsDict();
            PrepareRootWords();

            _wordFramesDict = _wordFramesList.GroupBy(x => x.Word).ToDictionary(p => p.Key, p => p.ToList());
        }

        public IList<BaseGrammaticalWordFrame> GetWordFramesByWord(string word)
        {
            if (_wordFramesDict.ContainsKey(word))
            {
                return _wordFramesDict[word];
            }

            return null;
        }

        public IList<BaseGrammaticalWordFrame> GetWordFramesByRootWord(string word)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, List<string>> mTmpMeaningsDict = new Dictionary<string, List<string>>();

        private List<BaseGrammaticalWordFrame> _wordFramesList = new List<BaseGrammaticalWordFrame>();
        private Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesDict;

        private void DefineCommonClasses()
        {
            DefineMeaning("act", "event");
            DefineMeaning("animate", "entity");
            DefineMeaning("phisobj", "entity");
            DefineMeanings("animal", new List<string>() { "animate", "phisobj" });
            DefineMeaning("moving", "act");
            DefineMeaning("place", "phisobj");
        }

        private void DefineMeaning(string word, string meaning)
        {
            DefineMeanings(word, new List<string>() { meaning });
        }

        private void DefineMeanings(string word, List<string> listOfMeanings)
        {
            if (mTmpMeaningsDict.ContainsKey(word))
            {
                var tmplistOfMeanings = mTmpMeaningsDict[word];
                tmplistOfMeanings.AddRange(listOfMeanings);
                tmplistOfMeanings = tmplistOfMeanings.Distinct().ToList();
                mTmpMeaningsDict[word] = tmplistOfMeanings;
            }
            else
            {
                mTmpMeaningsDict[word] = listOfMeanings;
            }
        }

        private void AddGrammaticalWordFrame(string word, BaseGrammaticalWordFrame grammaticalWordFrame)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentNullException(nameof(word));
            }

            if (grammaticalWordFrame == null)
            {
                throw new ArgumentNullException(nameof(grammaticalWordFrame));
            }

            grammaticalWordFrame.Word = word;

            _wordFramesList.Add(grammaticalWordFrame);
        }

        private void CalculateFullMeaningsDict()
        {
            var mMeaningsDict = new Dictionary<string, IList<string>>();

            foreach (var tmpMeaningsDictKVPItem in mTmpMeaningsDict)
            {
                var word = tmpMeaningsDictKVPItem.Key;
                var wasVisited = new List<string>();
                wasVisited.Add(word);
                var tmplistOfMeanings = tmpMeaningsDictKVPItem.Value;

                NCalculateFullMeaningsDict(word, ref tmplistOfMeanings, wasVisited);

                tmplistOfMeanings = tmplistOfMeanings.Distinct().ToList();
                mMeaningsDict[word] = tmplistOfMeanings;
            }

            foreach (var grammaticalWordFrame in _wordFramesList)
            {
                var logicalMeaningsList = grammaticalWordFrame.LogicalMeaning;

                if (logicalMeaningsList.IsNullOrEmpty())
                {
                    continue;
                }

                var completeLogicalMeaningsList = new List<string>();

                foreach (var logicalMeaning in logicalMeaningsList)
                {
                    completeLogicalMeaningsList.Add(logicalMeaning);

                    if (mMeaningsDict.ContainsKey(logicalMeaning))
                    {
                        var targetLogicalMeaningsList = mMeaningsDict[logicalMeaning];
                        completeLogicalMeaningsList.AddRange(targetLogicalMeaningsList);
                    }
                }

                completeLogicalMeaningsList = completeLogicalMeaningsList.Distinct().ToList();
                grammaticalWordFrame.FullLogicalMeaning = completeLogicalMeaningsList;
            }
        }

        private void NCalculateFullMeaningsDict(string word, ref List<string> listOfMeanings, List<string> wasVisited)
        {
            var tmpSourceListOfMeanings = listOfMeanings.ToList();
            foreach (var meaning in tmpSourceListOfMeanings)
            {
                if (wasVisited.Contains(meaning))
                {
                    continue;
                }

                if (mTmpMeaningsDict.ContainsKey(meaning))
                {
                    var tmplistOfMeanings = mTmpMeaningsDict[meaning];
                    listOfMeanings.AddRange(tmplistOfMeanings);
                    wasVisited.Add(meaning);

                    NCalculateFullMeaningsDict(meaning, ref listOfMeanings, wasVisited);
                }
            }
        }

        private void PrepareRootWords()
        {
            foreach (var grammaticalWordFrame in _wordFramesList)
            {
                if (string.IsNullOrWhiteSpace(grammaticalWordFrame.RootWord))
                {
                    grammaticalWordFrame.RootWord = grammaticalWordFrame.Word;
                }
            }
        }

        private void DefineWords()
        {
            DefineSpecialWords();
            DefineUsualWords();
        }

        private void DefineSpecialWords()
        {
            DefineToBeWords();
            DefineToDoWords();
            DefineToHaveWords();
            DefineModalWerbs();
            DefinePronouns();
            DefineArticles();
            DefineAdverbs();
            ProcessAllPrepositions();
            ProcessAllConjunctions();
            ProcessAllNumerals();
        }

        private void DefineToBeWords()
        {
            var word = "be";
            var rootWord = word;

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToBe = true
            });

            word = "been";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "am";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Singular,
                Person = GrammaticalPerson.First,
                RootWord = rootWord
            });

            word = "is";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Singular,
                Person = GrammaticalPerson.Second,
                RootWord = rootWord
            });

            word = "are";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Plural,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "was";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Past,
                Number = GrammaticalNumberOfWord.Singular,
                RootWord = rootWord
            });

            word = "were";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Past,
                Number = GrammaticalNumberOfWord.Plural,
                RootWord = rootWord
            });

            word = "being";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });

            word = "will";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Future
            });
        }

        private void DefineToDoWords()
        {
            var word = "do";
            var rootWord = word;

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToDo = true,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "does";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "did";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord
            });

            word = "done";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "doing";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });
        }

        private void DefineToHaveWords()
        {
            var word = "have";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToHave = true
            });

            word = "has";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "had";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "having";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });
        }

        private void DefineModalWerbs()
        {
            var word = "can";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });

            word = "could";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = rootWord
            });

            word = "may";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });

            word = "might";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = rootWord
            });

            word = "must";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });

            word = "would";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = "will"
            });

            word = "shell";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Future,
                IsModal = false,
                IsRare = true
            });

            word = "should";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = rootWord
            });

            word = "ought";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });
        }

        private void DefinePronouns()
        {
            var wordName = "i";
            var rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "me";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "my";
            AddGrammaticalWordFrame(wordName, new ArticleGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfArticle.Definite
            });

            wordName = "mine";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
            });

            wordName = "myself";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
            });

            wordName = "you";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.Second,
                Number = GrammaticalNumberOfWord.Neuter,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.Second,
                Number = GrammaticalNumberOfWord.Neuter,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "your";
            AddGrammaticalWordFrame(wordName, new ArticleGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfArticle.Definite
            });

            wordName = "yours";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Second,
                Number = GrammaticalNumberOfWord.Neuter,
                LogicalMeaning = new List<string>()
            });

            wordName = "yourself";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Second,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
            });

            wordName = "yourselves";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Second,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
            });

            wordName = "he";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "him";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "his";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
            });

            wordName = "himself";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
            });

            wordName = "she";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "her";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            AddGrammaticalWordFrame(wordName, new ArticleGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfArticle.Definite
            });

            wordName = "hers";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                LogicalMeaning = new List<string>()
            });

            wordName = "herself";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                LogicalMeaning = new List<string>()
            });

            wordName = "it";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "its";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
            });

            wordName = "itself";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
            });

            wordName = "we";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "us";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "our";
            AddGrammaticalWordFrame(wordName, new ArticleGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfArticle.Definite
            });

            wordName = "ours";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
            });

            wordName = "ourselves";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.First,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
            });

            wordName = "they";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Subject,
                Person = GrammaticalPerson.Third,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "them";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Personal,
                Case = CaseOfPersonalPronoun.Object,
                Person = GrammaticalPerson.Third,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "their";
            AddGrammaticalWordFrame(wordName, new ArticleGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfArticle.Definite
            });

            wordName = "theirs";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Possessive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                Number = GrammaticalNumberOfWord.Neuter,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "themselves";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Reflexive,
                Case = CaseOfPersonalPronoun.Undefined,
                Person = GrammaticalPerson.Third,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "this";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "near_object"
                }
            });

            wordName = "these";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "near_object"
                }
            });

            wordName = "that";
            rootWord = wordName;
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "far_object"
                }
            });

            wordName = "those";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                Number = GrammaticalNumberOfWord.Plural,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "far_object"
                }
            });

            wordName = "former";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative
            });

            wordName = "latter";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative
            });

            wordName = "who";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            wordName = "whom";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "as_possesive"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "as_possesive"
                }
            });

            wordName = "which";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object"
                }
            });

            wordName = "when";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            wordName = "where";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Demonstrative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            wordName = "something";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "thing"
                }
            });

            wordName = "anything";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "thing"
                }
            });

            wordName = "nothing";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                IsNegation = true,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "thing"
                }
            });

            wordName = "somewhere";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "about_place"
                }
            });

            wordName = "anywhere";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "about_place"
                }
            });

            wordName = "nowhere";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                IsNegation = true,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "about_place"
                }
            });

            wordName = "someone";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "people"
                }
            });

            wordName = "anyone";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Indefinite,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "people"
                }
            });

            wordName = "what";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object"
                }
            });

            wordName = "whose";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "object",
                    "as_possesive"
                }
            });

            wordName = "why";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "reason"
                }
            });

            wordName = "how";
            AddGrammaticalWordFrame(wordName, new PronounGrammaticalWordFrame()
            {
                TypeOfPronoun = TypeOfPronoun.Interrogative,
                IsQuestionWord = true,
                LogicalMeaning = new List<string>()
                {
                    "way"
                }
            });
        }

        private void DefineArticles()
        {
            var word = "the";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Kind = KindOfArticle.Definite
            });

            word = "a";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                Kind = KindOfArticle.Indefinite
            });

            word = "an";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                Kind = KindOfArticle.Indefinite
            });

            word = "no";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Kind = KindOfArticle.Negative
            });
        }

        private void ProcessAllPrepositions()
        {
            var word = "aboard";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "about";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "above";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "absent";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "across";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "cross";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "after";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "against";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'gainst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "gainst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsPoetic = true
            });

            word = "again";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "gain";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "along";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'long";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "alongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "alongside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amid";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amidst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "mid";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "midst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsPoetic = true
            });

            word = "among";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "'mong";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "mong";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "'mongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "apropos";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                IsRare = true
            });

            word = "apud";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "around";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'round";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "round";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "as";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "astride";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "at";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "@";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "atop";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ontop";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "bar";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "before";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "afore";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "tofore";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "B4";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "behind";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ahind";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "below";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ablow";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "allow";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsDialectal = true
            });

            word = "beneath";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'neath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "neath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true
            });

            word = "beside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "besides";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "between";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "atween";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "beyond";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ayond ";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "but";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "by";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "chez";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                IsRare = true
            });

            word = "circa";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "c.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "ca.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "come";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "dehors";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "despite";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "spite";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "down";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "during";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "except";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "for";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "4";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "from";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "in";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "inside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "into";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "less";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "like";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "minus";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "near";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "nearer";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Comparison = GrammaticalComparison.Comparative
            });

            word = "nearest ";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Comparison = GrammaticalComparison.Superlative
            });

            word = "anear";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "notwithstanding";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "of";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "o'";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true,
                IsDialectal = true
            });

            word = "off";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "on";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "onto";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "opposite";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "out";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "outen";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "outside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "over";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "o'er";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true
            });

            word = "pace";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "past";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "per";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "plus";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "post";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "pre";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "pro";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "qua";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "re";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sans";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "save";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sauf";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "short";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "since";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sithence";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "than";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "through";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "thru";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "throughout";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "thruout";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "till";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "to";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                ConditionalLogicalMeaning = new Dictionary<string, IList<string>>()
                {
                    {
                        "go", new List<string>() {
                            "direction"
                        }
                    }
                }
            });

            word = "2";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "toward";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "towards";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "under";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "underneath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "unlike";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "until";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'til";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "til";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "unto";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "up";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "upon";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'pon";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "pon";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "upside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "versus";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vs.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "v.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "via";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vice";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vis-à-vis";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "with";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "wi'";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "c̄";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "within";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/i";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "without";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/o";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "worth";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "next";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ago";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "apart";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "aside";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "aslant";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "away";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "hence";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "withal";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
                IsArchaic = true
            });
        }

        private void ProcessAllConjunctions()
        {
            var word = "and";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "but";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "for";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "nor";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "or";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "so";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "yet";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "though";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "although";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "while";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "unless";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "until";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "lest";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "than";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "whether";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "whereas";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "after";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "before";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "once";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "since";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "till";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "until";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "when";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "whenever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "while";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "because";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "since";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "why";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective
            });

            word = "what";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "whatever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "which";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "whichever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "who";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whoever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whom";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whomever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whose";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "how";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Manner,
                IsQuestionWord = true
            });

            word = "where";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Place,
                IsQuestionWord = true
            });

            word = "wherever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Place,
                IsQuestionWord = true
            });
        }

        private void ProcessAllNumerals()
        {
            var word = "zero";
            var value = 0f;
            var rootWord = word;

            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "naught";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nought";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "aught";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "oh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nil";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nothing";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "null";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
            });

            word = "love";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "zilch";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nada";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "zip";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nix";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "cypher";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "duck";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "blank";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "zeroth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "noughth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "0th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "noughth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "one";
            value = 1f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "solo";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "unit";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "unity";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "once";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "solitary";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "singular";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "1st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "two";
            value = 2f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "couple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "brace";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "pair";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "deuce";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "duo";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quadratic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twice";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "double";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "twofold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "duplicate";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "2nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "three";
            value = 3f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "trio";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "cubic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thrice";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "triple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "threefold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "triplicate";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "3rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "four";
            value = 4f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "quartet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quad";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quadruple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "fourfold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "4th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quarter";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quarters";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "five";
            value = 5f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "quintet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quintic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quint";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quintuple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "fivefold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "5th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "six";
            value = 6f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "sextet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sextic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "hectic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sextuple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "hextuple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "sixfold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "6th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seven";
            value = 7f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "septet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "septic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "heptic";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "septuple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "heptuple";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "sevenfold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "7th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eight";
            value = 8f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "octet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "8th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nine";
            value = 9f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "nonet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "9th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ten";
            value = 10f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "dime";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "decet";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "10th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "tenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "tenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eleven";
            value = 11f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "11th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eleventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "elevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twelve";
            value = 12f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "dozen";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "12th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twelfth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twelfths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirteen";
            value = 13f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "13th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourteen";
            value = 14f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "14th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fourteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifteen";
            value = 15f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "15th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixteen";
            value = 16f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "16th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventeen";
            value = 17f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "17th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventeenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventeenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighteen";
            value = 18f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "18th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nineteen";
            value = 19f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "19th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nineteenth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "nineteenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty";
            value = 20f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "20th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twentieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twentieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty";
            value = 30f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "30th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirtieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirtieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty";
            value = 40f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "40th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fortieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fortieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty";
            value = 50f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "50th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fiftieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fiftieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty";
            value = 60f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "60th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixtieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixtieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy";
            value = 70f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "70th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty";
            value = 80f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "80th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eightieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eightieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety";
            value = 90f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "90th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninetieth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninetieths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "hundred";
            value = 100f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "hundredfold";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "100th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "hundredth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "hundredths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thousand";
            value = 1000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "1000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thousandth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thousandths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "million";
            value = 1000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "1000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "millionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "millionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "milliard";
            value = 1000000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "billion";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "1000000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "milliardth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "milliardths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trillion";
            value = 1000000000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "billion";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "1000000000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trillionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trillionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quadrillion";
            value = 1000000000000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "billiard";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "1000000000000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quadrillionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quadrillionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billiardth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "billiardths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quintillion";
            value = 1000000000000000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "trillion";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "1000000000000000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quintillionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "quintillionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trillionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trillionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trilliard";
            value = 1000000000000000000000f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "sextillion";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "1000000000000000000000th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sextillionth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sextillionths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trilliardth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "trilliardths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "half";
            value = 0.5f;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value
            });
        }

        private void DefineAdverbs()
        {
            var word = "not";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                IsDeterminer = true,
                IsNegation = true
            });

            word = "there";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "place",
                    "as_far"
                }
            });

            word = "here";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "place",
                    "as_near"
                }
            });

            word = "some";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                IsDeterminer = true
            });

            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame()
            {
                IsDeterminer = true
            });

            word = "soon";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "time",
                    "as_near"
                }
            });

            word = "tomorrow";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "yesterday";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "yesterday's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "only";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
            });

            word = "already";
            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
            });


        }

        private void DefineUsualWords()
        {
            DefineUsualNouns();
            DefineUsualVerbs();
            DefineUsualAdjectives();
            DefineUsualPrepositions();
            DefineNames();
        }

        private void DefineUsualNouns()
        {
            var word = "dog";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "cat";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "waypoint";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            word = "work";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "pleasure";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "meeting";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "event"
                }
            });

            word = "o'clock";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
            });

            word = "moment";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame()
            {
            });

            word = "week";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "week's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "year";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "holiday";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "space";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            word = "time";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "noise";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "office";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            word = "book";
            rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

            word = "books";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

            word = "dictionary";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "place"
                }
            });

            word = "gel";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "honor";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "case";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "favour";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "adventurer";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "home";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "soup";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "bag";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "coastline";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

            word = "father";
            rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "father's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "funeral";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "event"
                }
            });

            word = "paper";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                LogicalMeaning = new List<string>()
                {
                    "event"
                }
            });

            word = "sister-in-law";
            rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "sister-in-law's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "friend";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "ticket";
            rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

            word = "tickets";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

            word = "cousin";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                Gender = GrammaticalGender.Feminine,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "station";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = true,
                LogicalMeaning = new List<string>()
                {
                    "phisobj"
                }
            });

        }

        private void DefineUsualVerbs()
        {
            var word = "know";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "state"
                }
            });

            word = "go";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act",
                    "moving"
                }
            });

            word = "went";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act",
                    "moving"
                }
            });

            word = "gone";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "play";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "played";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "playing";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "start";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "state"
                }
            });

            word = "starts";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.First,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "state"
                }
            });

            word = "stop";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "state"
                }
            });

            word = "explore";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "explored";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "say";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "says";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.First,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "said";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "read";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "talk";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "visit";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "get";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "got";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "gotten";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "forget";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "state"
                }
            });

            word = "conclude";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "concludes";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.First,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "ask";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "speak";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "speaking";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "fly";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular
            });

            word = "scream";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "screamed";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "come";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "came";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "tell";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "told";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "sell";
            rootWord = word;
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "sold";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_3,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            word = "pick";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
            });

            word = "like";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                LogicalMeaning = new List<string>()
                {
                    "act"
                }
            });
        }

        private void DefineUsualAdjectives()
        {
            var word = "sorry";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "green";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "color"
                }
            });

            word = "new";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame()
            {
                LogicalMeaning = new List<string>()
                {
                    "time"
                }
            });

            word = "able";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "german";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "every";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "shower";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "little";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame());

            word = "beautiful";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            word = "very";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame());

            word = "far";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame());

            word = "all";
            AddGrammaticalWordFrame(word, new AdjectiveGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new AdverbGrammaticalWordFrame());

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Plural
            });

            AddGrammaticalWordFrame(word, new PronounGrammaticalWordFrame());


        }

        private void DefineUsualPrepositions()
        {
            /*var word = "to";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                ConditionalLogicalMeaning = new Dictionary<string, IList<string>>()
                {
                    {
                        "go", new List<string>() {
                            "direction"
                        }
                    }
                }
            });*/
        }

        private void DefineNames()
        {
            DefineHumanNames();
        }

        private void DefineHumanNames()
        {
            var word = "Tom";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Winnie-the-Pooh";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Stephen";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Jonathan";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Kev";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Tim";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Simone";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Olivia";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Olivia's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Feminine,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Britain";
            rootWord = word;

            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Neuter,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Britain's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Neuter,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Jake";
            rootWord = word;
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });

            word = "Jake's";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                IsName = true,
                IsCountable = true,
                Number = GrammaticalNumberOfWord.Singular,
                Gender = GrammaticalGender.Masculine,
                IsPossessive = true,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "animate"
                }
            });


        }
    }
}
