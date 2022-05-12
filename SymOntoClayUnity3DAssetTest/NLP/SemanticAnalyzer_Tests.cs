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
n_1[shape=box,label=""simple""];
n_3[shape = box, label = ""present""];
            n_5[shape = box, label = ""active""];
            n_7[shape = box, label = ""indicative""];
            n_2[shape = ellipse, label = ""__aspect""];
            n_4[shape = ellipse, label = ""__tense""];
            n_6[shape = ellipse, label = ""__voice""];
            n_8[shape = ellipse, label = ""__mood""];
            subgraph cluster_2{
                n_9[shape = box, label = ""i""];
                n_10[shape = box, label = ""like""];
                n_11[shape = box, label = ""cat""];
                n_12[shape = box, label = ""my""];
                n_13[shape = ellipse, label = ""determiner""];
                n_14[shape = ellipse, label = ""__entity_condition""];
                n_15[shape = ellipse, label = ""object""];
                n_16[shape = ellipse, label = ""experiencer""];
                n_17[shape = ellipse, label = ""state""];
                n_9->n_17;
                n_10->n_15;
                n_10->n_16;
                n_11->n_13;
                n_13->n_12;
                n_14->n_13;
                n_15->n_11;
                n_16->n_9;
                n_17->n_10;
            }

            n_9->n_2[ltail = cluster_2,];
            n_9->n_4[ltail = cluster_2,];
            n_9->n_6[ltail = cluster_2,];
            n_9->n_8[ltail = cluster_2,];
            n_2->n_1;
            n_4->n_3;
            n_6->n_5;
            n_8->n_7;
        }", conceptualGraphDbgStr.Trim());
        }
    }
}
