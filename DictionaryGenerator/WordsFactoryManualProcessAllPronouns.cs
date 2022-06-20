/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllPronouns()
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("Begin ProcessAllPronouns");
#endif

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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
                    "object"
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
    }
}
