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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class NounWordNode
    {
        public NounWordNode(string word, RoleOfNoun roleOfNoun, IMonitorLogger logger, IWordsDict wordsDict, GrammaticalMood mood)
        {
            _logger = logger;
            _wordsDict = wordsDict;
            _word = word;
            _roleOfNoun = roleOfNoun;
            _mood = mood;
        }

        private readonly IMonitorLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly string _word;
        private readonly RoleOfNoun _roleOfNoun;
        private readonly GrammaticalMood _mood;

        public Word GetWord()
        {
#if DEBUG
            //_logger.Info("06CE6419-E709-4055-BD8E-8B40F3400DC6", $"_roleOfNoun = {_roleOfNoun}");
#endif

            switch (_roleOfNoun)
            {
                case RoleOfNoun.Subject:
                    {
#if DEBUG
                        //_logger.Info("662ADA38-0558-4988-B431-6FDD7C414D05", $"_word = '{_word}'");
#endif

                        var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

#if DEBUG
                        //_logger.Info("527BC61B-8A94-4FD8-97DF-BD996F9F9343", $"wordFramesList?.Count = {wordFramesList?.Count}");
#endif

                        var pronounsList = wordFramesList.Where(p => p.IsPronoun).Select(p => p.AsPronoun).Where(p => p.Case == CaseOfPersonalPronoun.Subject).ToList();

                        if (pronounsList.Any())
                        {
                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = pronounsList.Single();

                            return result;
                        }

                        pronounsList = wordFramesList.Where(p => p.IsPronoun).Select(p => p.AsPronoun).Where(p => p.Case == CaseOfPersonalPronoun.Undefined).ToList();

                        if (pronounsList.Any())
                        {
                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = pronounsList.Single();

                            return result;
                        }

                        var nounsList = wordFramesList.Where(p => p.IsNoun).Select(p => p.AsNoun).ToList();

                        if (nounsList.Any())
                        {
                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = nounsList.Single();

                            return result;
                        }

                        throw new NotImplementedException("B17D6D8B-9487-466D-BF83-6CF0FCA4243D");
                    }

                case RoleOfNoun.Object:
                    {
                        if(_mood == GrammaticalMood.Imperative && _word == "self")
                        {
                            return null;
                        }

                        var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

                        var nounsList = wordFramesList.Where(p => p.IsNoun).Select(p => p.AsNoun).ToList();

                        if (nounsList.Any())
                        {
                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = nounsList.Single();

                            return result;
                        }

                        throw new NotImplementedException("600065E6-9721-41A7-97CE-6F77F23EF6AB");
                    }

                case RoleOfNoun.PossessDeterminer:
                    {
                        var wordFramesList = _wordsDict.GetWordFramesByRootWord(_word);

                        var articlesList = wordFramesList.Where(p => p.IsArticle).Select(p => p.AsArticle).ToList();

                        if(articlesList.Any())
                        {
                            var targetWordFrame = articlesList.Single();

                            var result = new Word();

                            result.Content = targetWordFrame.Word;

                            result.WordFrame = targetWordFrame;

                            return result;
                        }

                        throw new NotImplementedException("45249671-3DC9-4F4E-B669-01BDA6F21797");
                    }

                case RoleOfNoun.AnotherRole:
                    {
                        var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

                        var nounsList = wordFramesList.Where(p => p.IsNoun).Select(p => p.AsNoun).ToList();

                        if (nounsList.Any())
                        {
                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = nounsList.Single();

                            return result;
                        }

                        throw new NotImplementedException("2FEA1D14-2777-49FF-B592-DED05F075786");
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(_roleOfNoun), _roleOfNoun, null);
            }
        }

        public NounPhrase GetNounPhrase()
        {
            var nounWord = GetWord();

            if(nounWord == null)
            {
                return null;
            }

            var nounPhrase = new NounPhrase();
            nounPhrase.N = nounWord;

            return nounPhrase;
        }
    }
}
