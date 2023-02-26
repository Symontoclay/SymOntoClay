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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SymOntoClay.NLP.CommonDict.Implementations
{
    public class WordsDictJSONSerializationEngine
    {
        private DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, List<BaseGrammaticalWordFrame>>), new List<Type> {
                typeof(PronounGrammaticalWordFrame),
                typeof(AdjectiveGrammaticalWordFrame),
                typeof(AdverbGrammaticalWordFrame),
                typeof(ArticleGrammaticalWordFrame),
                typeof(ConjunctionGrammaticalWordFrame),
                typeof(NounGrammaticalWordFrame),
                typeof(NumeralGrammaticalWordFrame),
                typeof(PostpositionGrammaticalWordFrame),
                typeof(PrepositionGrammaticalWordFrame),
                typeof(PronounGrammaticalWordFrame),
                typeof(VerbGrammaticalWordFrame)
    });

        public void SaveToFile(Dictionary<string, List<BaseGrammaticalWordFrame>> item, string fileName)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fs = File.OpenWrite(fileName))
            {
                _jsonSerializer.WriteObject(fs, item);
                fs.Flush();
            }
        }

        public Dictionary<string, List<BaseGrammaticalWordFrame>> LoadFromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using (var fs = File.OpenRead(fileName))
            {
                return (Dictionary<string, List<BaseGrammaticalWordFrame>>)_jsonSerializer.ReadObject(fs);
            }
        }
    }
}
