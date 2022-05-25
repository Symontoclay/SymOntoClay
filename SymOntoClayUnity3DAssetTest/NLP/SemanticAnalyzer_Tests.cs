using NUnit.Framework;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
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
    }
}
