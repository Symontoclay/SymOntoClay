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

using SymOntoClay.CoreHelper.DebugHelpers;
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
        public NounWordNode(string word, RoleOfNoun roleOfNoun, IEntityLogger logger, IWordsDict wordsDict, GrammaticalMood mood)
        {
            _logger = logger;
            _wordsDict = wordsDict;
            _word = word;
            _roleOfNoun = roleOfNoun;
            _mood = mood;
        }

        private readonly IEntityLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly string _word;
        private readonly RoleOfNoun _roleOfNoun;
        private readonly GrammaticalMood _mood;

        public Word GetWord()
        {
#if DEBUG
            //_logger.Log($"_word = '{_word}'");
            //_logger.Log($"_roleOfNoun = {_roleOfNoun}");
#endif

            switch (_roleOfNoun)
            {
                case RoleOfNoun.Subject:
                    {
                        var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

#if DEBUG
                        //_logger.Log($"wordFramesList = {wordFramesList.WriteListToString()}");
#endif

                        var pronounsList = wordFramesList.Where(p => p.IsPronoun).Select(p => p.AsPronoun).Where(p => p.Case == CaseOfPersonalPronoun.Subject).ToList();

                        if (pronounsList.Any())
                        {
#if DEBUG
                            //_logger.Log($"pronounsList = {pronounsList.WriteListToString()}");
#endif

                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = pronounsList.Single();

#if DEBUG
                            //_logger.Log($"result = {result}");
#endif

                            return result;
                        }

                        var nounsList = wordFramesList.Where(p => p.IsNoun).Select(p => p.AsNoun).ToList();

                        if (nounsList.Any())
                        {
#if DEBUG
                            //_logger.Log($"nounsList = {nounsList.WriteListToString()}");
#endif

                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = nounsList.Single();

#if DEBUG
                            //_logger.Log($"result = {result}");
#endif

                            return result;
                        }

                        throw new NotImplementedException();
                    }

                case RoleOfNoun.Object:
                    {
                        if(_mood == GrammaticalMood.Imperative && _word == "self")
                        {
                            return null;
                        }

                        throw new NotImplementedException();
                    }

                case RoleOfNoun.PossessDeterminer:
                    {
                        var wordFramesList = _wordsDict.GetWordFramesByRootWord(_word);

#if DEBUG
                        //_logger.Log($"wordFramesList = {wordFramesList.WriteListToString()}");
#endif

                        var articlesList = wordFramesList.Where(p => p.IsArticle).Select(p => p.AsArticle).ToList();

                        if(articlesList.Any())
                        {
#if DEBUG
                            //_logger.Log($"articlesList = {articlesList.WriteListToString()}");
#endif

                            var targetWordFrame = articlesList.Single();

                            var result = new Word();

                            result.Content = targetWordFrame.Word;

                            result.WordFrame = targetWordFrame;

#if DEBUG
                            //_logger.Log($"result = {result}");
#endif

                            return result;
                        }

                        throw new NotImplementedException();
                    }

                case RoleOfNoun.AnotherRole:
                    {
                        var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

#if DEBUG
                        //_logger.Log($"wordFramesList = {wordFramesList.WriteListToString()}");
#endif

                        var nounsList = wordFramesList.Where(p => p.IsNoun).Select(p => p.AsNoun).ToList();

                        if (nounsList.Any())
                        {
#if DEBUG
                            //_logger.Log($"nounsList = {nounsList.WriteListToString()}");
#endif

                            var result = new Word();

                            result.Content = _word;

                            result.WordFrame = nounsList.Single();

#if DEBUG
                            //_logger.Log($"result = {result}");
#endif

                            return result;
                        }

                        throw new NotImplementedException();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(_roleOfNoun), _roleOfNoun, null);
            }
        }

        public NounPhrase GetNounPhrase()
        {
            var nounWord = GetWord();

#if DEBUG
            //_logger.Log($"nounWord = {nounWord}");
#endif

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
