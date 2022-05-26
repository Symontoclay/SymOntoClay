using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP.ATN
{
    public class ConverterInternalCGToPhraseStructure_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            _testEngineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext();
            _engineContext = _testEngineContext.EngineContext;
            _wordsDict = DictionaryInstance.Instance;
        }

        [TearDown]
        public void Cleanup()
        {
            _testEngineContext.Dispose();
        }

        private IEntityLogger _logger;
        private ComplexTestEngineContext _testEngineContext;
        private IEngineContext _engineContext;
        private IWordsDict _wordsDict;

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";

            var nlpContext = UnityTestEngineContextFactory.CreateNLPConverterContext(_engineContext);

            var ruleInstance = Parse(factStr);

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            Assert.AreEqual(@"S
    Subject:
        NP
            N:
                i
    Predicate:
        VP
            V:
                like
            Object:
                NP
                    N:
                        cat
                    D:
                        my", sentenceItem.ToDbgString().Trim());
        }
    }
}
