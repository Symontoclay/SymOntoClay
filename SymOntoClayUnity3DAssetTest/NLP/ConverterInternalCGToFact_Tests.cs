using NUnit.Framework;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.ConvertingCGToInternal;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToFact;
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
    public class ConverterInternalCGToFact_Tests
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

            var convertorCGToInternal = new ConvertorCGToInternal(_logger);

            var internalCG = convertorCGToInternal.Convert(conceptualGraph);

            var converterInternalCGToFact = new ConverterInternalCGToFact(_logger);

            var ruleInstancesList = converterInternalCGToFact.ConvertConceptualGraph(internalCG);

            Assert.AreEqual(1, ruleInstancesList.Count);

            var ruleInstance = ruleInstancesList[0];

            Assert.AreEqual(KindOfRuleInstance.Fact, ruleInstance.KindOfRuleInstance);

            Assert.AreEqual(@"{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }
    }
}
