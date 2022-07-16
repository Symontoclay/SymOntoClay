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

using NUnit.Framework;
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using SymOntoClay.UnityAsset.Core.Tests.NLP.ATN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP
{
    public class SemanticAnalyzer_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            _wordsDict = DictionaryInstance.Instance;
        }

        private IEntityLogger _logger;
        private IWordsDict _wordsDict;

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = "I like my cat.";

            var parser = new ATNParser(_logger, _wordsDict);

            var result = parser.Run(text);

            Assert.AreEqual(1, result.Count);

            var item = result[0];

            var compactizer = new PhraseCompactizer(_logger, _wordsDict);

            compactizer.Run(item);

            var converterToPlainSentences = new ConverterToPlainSentences(_logger);

            var plainSentencesList = converterToPlainSentences.Run(item);

            Assert.AreEqual(1, plainSentencesList.Count);

            var plainSentence = plainSentencesList[0];

            var semanticAnalyzer = new SemanticAnalyzer(_logger, _wordsDict);

            var conceptualGraph = semanticAnalyzer.Run(plainSentence);

            var conceptualGraphDbgStr = DotConverter.ConvertToString(conceptualGraph);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
subgraph cluster_2{
n_1[shape=box,label=""i""];
n_2[shape=box,label=""like""];
n_3[shape=box,label=""cat""];
n_4[shape=box,label=""i""];
n_5[shape=ellipse,label=""possess""];
n_6[shape=ellipse,label=""__entity_condition""];
n_7[shape=ellipse,label=""object""];
n_8[shape=ellipse,label=""experiencer""];
n_9[shape=ellipse,label=""state""];
n_1 -> n_9;
n_2 -> n_7;
n_2 -> n_8;
n_4 -> n_5;
n_5 -> n_3;
n_6 -> n_5;
n_7 -> n_3;
n_8 -> n_1;
n_9 -> n_2;
}

}".DeepTrim(), conceptualGraphDbgStr.DeepTrim());
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = "Go to green place!";

            var parser = new ATNParser(_logger, _wordsDict);

            var result = parser.Run(text);

            Assert.AreEqual(1, result.Count);

            var item = result[0];

            var compactizer = new PhraseCompactizer(_logger, _wordsDict);

            compactizer.Run(item);

            var converterToPlainSentences = new ConverterToPlainSentences(_logger);

            var plainSentencesList = converterToPlainSentences.Run(item);

            Assert.AreEqual(1, plainSentencesList.Count);

            var plainSentence = plainSentencesList[0];

            var semanticAnalyzer = new SemanticAnalyzer(_logger, _wordsDict);

            var conceptualGraph = semanticAnalyzer.Run(plainSentence);

            var conceptualGraphDbgStr = DotConverter.ConvertToString(conceptualGraph);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
n_1[shape=box,label=""imperative""];
n_2[shape=ellipse,label=""__mood""];
subgraph cluster_2{
n_3[shape=box,label=""go""];
n_5[shape=box,label=""place""];
n_6[shape=box,label=""green""];
n_4[shape=ellipse,label=""action""];
n_7[shape=ellipse,label=""color""];
n_8[shape=ellipse,label=""__entity_condition""];
n_9[shape=ellipse,label=""direction""];
n_10[shape=ellipse,label=""command""];
n_3 -> n_9;
n_4 -> n_3;
n_5 -> n_7;
n_7 -> n_6;
n_8 -> n_7;
n_9 -> n_5;
n_10 -> n_3;
}

n_3 -> n_2[ltail=cluster_2,];
n_2 -> n_1;
}".DeepTrim(), conceptualGraphDbgStr.DeepTrim());
        }
    }
}
