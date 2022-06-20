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

using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.CommonDict.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Dicts
{
    public class JsonDictionary : IWordsDict
    {
        public JsonDictionary(string fileName)
        {
            var serializer = new WordsDictJSONSerializationEngine();

            _wordFramesByWordDict = serializer.LoadFromFile(fileName);
            var tmpList = _wordFramesByWordDict.SelectMany(p => p.Value.ToList());
            _wordFramesByByRootWordDict = tmpList.GroupBy(p => p.RootWord).ToDictionary(p => p.Key, p => p.ToList());
        }

        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesByWordDict;
        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _wordFramesByByRootWordDict;

        /// <inheritdoc/>
        public IList<BaseGrammaticalWordFrame> GetWordFramesByWord(string word)
        {
            if (_wordFramesByWordDict.ContainsKey(word))
            {
                return _wordFramesByWordDict[word];
            }

            return null;
        }

        /// <inheritdoc/>
        public IList<BaseGrammaticalWordFrame> GetWordFramesByRootWord(string word)
        {
            if (_wordFramesByByRootWordDict.ContainsKey(word))
            {
                return _wordFramesByByRootWordDict[word];
            }

            return null;
        }
    }
}
