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
    public class AdjectiveWordNode
    {
        public AdjectiveWordNode(string word, IMonitorLogger logger, IWordsDict wordsDict)
        {
            _logger = logger;
            _wordsDict = wordsDict;
            _word = word;
        }

        private readonly IMonitorLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly string _word;

        public Word GetWord()
        {
            var wordFramesList = _wordsDict.GetWordFramesByWord(_word);

            var adjectivesList = wordFramesList.Where(p => p.IsAdjective).Select(p => p.AsAdjective).ToList();

            if(adjectivesList.Any())
            {
                var result = new Word();

                result.Content = _word;

                result.WordFrame = adjectivesList.Single();

                return result;
            }

            throw new NotImplementedException();
        }
    }
}
